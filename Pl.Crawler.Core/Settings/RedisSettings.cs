namespace Pl.Crawler.Core.Settings
{
    /// <summary>
    /// Tỏng  hợp connection
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// Thông tin kết nối redis
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Tên ứng dụng
        /// </summary>
        public string InstanceName { get; set; }
    }
}
