namespace Pl.Crawler.Core.Constants
{
    public class MessageQueueConstants
    {
        /// <summary>
        /// Tên queue download nội dung web
        /// </summary>
        public const string DownloadWebContentTaskQueueName = "DownloadWebContentTask";

        /// <summary>
        /// Tên queue xử lý nội dung web
        /// </summary>
        public const string ProcessWebContentTaskQueueName = "ProcessWebContentTask";

        /// <summary>
        /// Tên queue tìm kiếm link mới của nội dung web
        /// </summary>
        public const string FindlinkWebContentTaskQueueName = "FindlinkWebContentTask";

        /// <summary>
        /// Tên queue xuất dữ liệu về api
        /// </summary>
        public const string DataExportTaskQueueName = "DataExportTask";

        /// <summary>
        /// Tên queue thông báo
        /// </summary>
        public const string NotificationTaskQueueName = "NotificationTask";
    }
}
