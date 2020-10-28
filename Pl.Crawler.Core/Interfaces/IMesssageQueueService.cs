using System;
using System.Collections.Generic;

namespace Pl.Crawler.Core.Interfaces
{
    public interface IMesssageQueueService
    {

        /// <summary>
        /// Xuất bản một nhiệm vụ
        /// </summary>
        /// <param name="queueName">Tên nhiệm vụ</param>
        /// <param name="message">Nội dung nhiệm vụ</param>
        bool PublishTask(string queueName, string message);

        /// <summary>
        /// Đăng ký thực hiện một nhiệm vụ chỉ dùng cho console app
        /// </summary>
        /// <param name="queueName">Tên nhiệm vụ</param>
        /// <param name="func">Worker sẽ thực hiện khi có sự kiện</param>
        void RegisterConsoleWorker(string queueName, Func<string, bool> func);

        /// <summary>
        /// Xuất bản nhiều nhiệm vụ
        /// </summary>
        /// <param name="queueName">Tên nhiệm vụ</param>
        /// <param name="messages">Danh sách nội dung nghiệm vụ</param>
        bool PublishTasks(string queueName, List<string> messages);

    }
}