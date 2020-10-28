namespace Pl.Crawler.Core.Settings
{
    public class ConnectionMessageQueue
    {
        public string HostName { get; set; }

        public int Port { get; set; } = 5672;

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// Thời gian time out khi kết nối tính bằng giây
        /// </summary>
        public int ContinuationTimeout { get; set; } = 10;
    }
}
