using Microsoft.Extensions.Options;
using Pl.Crawler.Core;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pl.Crawler.MessageQueue
{
    public class RabbitmqService : IMesssageQueueService
    {

        private readonly ConnectionMessageQueue connectionMessageQueue;

        public RabbitmqService(
            IOptions<ConnectionMessageQueue> options)
        {
            connectionMessageQueue = options.Value;
        }

        /// <summary>
        /// Xuất bản một nhiệm vụ
        /// </summary>
        /// <param name="queueName">Tên nhiệm vụ</param>
        /// <param name="message">Nội dung nhiệm vụ</param>
        public bool PublishTask(string queueName, string message)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = connectionMessageQueue.HostName,
                    Port = connectionMessageQueue.Port,
                    UserName = connectionMessageQueue.UserName,
                    Password = connectionMessageQueue.Password,
                    VirtualHost = connectionMessageQueue.VirtualHost,
                    ContinuationTimeout = new TimeSpan(0, 0, 0, connectionMessageQueue.ContinuationTimeout)
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: properties,
                                         body: body);
                    $"Send to queue name: {queueName} with message: {message}".WriteConsole(ConsoleColor.Green);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.Message.WriteConsole(ConsoleColor.Red);
                return false;
            }
        }

        /// <summary>
        /// Xuất bản nhiều nhiệm vụ
        /// </summary>
        /// <param name="queueName">Tên nhiệm vụ</param>
        /// <param name="messages">Danh sách nội dung nghiệm vụ</param>
        public bool PublishTasks(string queueName, List<string> messages)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = connectionMessageQueue.HostName,
                    Port = connectionMessageQueue.Port,
                    UserName = connectionMessageQueue.UserName,
                    Password = connectionMessageQueue.Password,
                    VirtualHost = connectionMessageQueue.VirtualHost,
                    ContinuationTimeout = new TimeSpan(0, 0, 0, connectionMessageQueue.ContinuationTimeout)
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    foreach (var message in messages)
                    {
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: queueName,
                                             basicProperties: properties,
                                             body: body);
                        $"Send to queue name: {queueName} with message: {message}".WriteConsole(ConsoleColor.Green);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.Message.WriteConsole(ConsoleColor.Red);
                return false;
            }
        }

        /// <summary>
        /// Đăng ký thực hiện một nhiệm vụ chỉ dùng cho console app
        /// </summary>
        /// <param name="queueName">Tên nhiệm vụ</param>
        /// <param name="eventingBasicConsumer">Worker sẽ thực hiện khi có sự kiện</param>
        public void RegisterConsoleWorker(string queueName, Func<string, bool> func)
        {
            var factory = new ConnectionFactory()
            {
                HostName = connectionMessageQueue.HostName,
                Port = connectionMessageQueue.Port,
                UserName = connectionMessageQueue.UserName,
                Password = connectionMessageQueue.Password,
                VirtualHost = connectionMessageQueue.VirtualHost,
                ContinuationTimeout = new TimeSpan(0, 0, 0, connectionMessageQueue.ContinuationTimeout)
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body);
                    try
                    {
                        func(message);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString().WriteConsole(ConsoleColor.Red);
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
                while (true)
                {
                    Thread.Sleep(int.MaxValue);
                }

            }
        }
    }
}
