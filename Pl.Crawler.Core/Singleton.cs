namespace Pl.Crawler.Core
{
    /// <summary>
    /// Đảm bảo cả hệ thống chỉ có một thể hiện của đối tượng được sử dụng
    /// Có thể sử dụng cho nhiều luồng sử lý
    /// </summary>
    /// <typeparam name="T">Kiểu của thực thể</typeparam>
    public static class Singleton<T>
    {
        private static T instance;
        private static readonly object thisLock = new object();

        /// <summary>
        /// Thể hiển của giá trị
        /// </summary>
        public static T Instance
        {
            get => instance;
            set
            {
                lock (thisLock)
                {
                    instance = value;
                }
            }
        }
    }
}