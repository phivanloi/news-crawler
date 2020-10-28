using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Pl.Crawler.Core.Entities
{
    public class Webpage : MongoBaseEntity
    {

        /// <summary>
        /// Id website
        /// </summary>
        [BsonElement("WebsiteId")]
        public long WebsiteId { get; set; }

        /// <summary>
        /// The crawl config id
        /// </summary>
        [BsonElement("ConfigId")]
        public long ConfigId { get; set; }

        /// <summary>
        /// Url trang cần download
        /// </summary>
        [BsonElement("Url")]
        public string Url { get; set; }

        /// <summary>
        /// Chuỗi md5 sinh ra từ url dùng để query where
        /// </summary>
        [BsonElement("UrlMd5")]
        public string UrlMd5 { get; set; }

        /// <summary>
        /// Thời hạn cho lần download tiếp theo
        /// Tối thiểu là từ lúc xử lý phân tích xong cộng với 30 s và tối đa là 1 ngày
        /// có quy tắc sau.Mỗi lần xử lý không thấy page có thay đổi thì page sẽ được tăng thêm 30s và tối đa là 1 ngày
        /// và khi có phát hiện update thì page sẽ được trừ đi 300s tối đa là 30s
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("NextDownloadTime")]
        public DateTime NextDownloadTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Điểm Rank của trang
        /// 0 tương ứng với không được đưa vào xử lý liên tục mà chỉ được xử lý 1 lần
        /// 1 xử lý mỗi 1 lần/ 1 tháng
        /// 2 xử lý mỗi 1 lần/ 1 tuần
        /// 3 xử lý mỗi 1 lần/ 3 ngày
        /// 4 xử lý mỗi 1 lần/ 2 ngày
        /// 5 xử lý mỗi 1 lần/ 1 ngày
        /// 6 xử lý mỗi 1 lần/ 6 giờ
        /// 7 xử lý mỗi 1 lần/ 1 giờ
        /// 8 xử lý mỗi 1 lần/ 30 phút
        /// 9 xử lý mỗi 1 lần/ 10 phút
        /// 10 xử lý mỗi 1 lần/ 1 phút, chỉ được fix
        /// </summary>
        [BsonElement("DownloadRank")]
        public int DownloadRank { get; set; } = 5;

        /// <summary>
        /// Lần download thành công cuối
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LastDownloaded")]
        public DateTime? LastDownloaded { get; set; } = null;

        /// <summary>
        /// Số lần download lỗi 
        /// Cứ 5 lần download lỗi thì tụt 1 rank download và khi tụt rank thì bị reset số lần download lỗi về 0
        /// </summary>
        [BsonElement("ErrorDownloadCount")]
        public int ErrorDownloadCount { get; set; } = 0;

        /// <summary>
        /// mã download lần cuối
        /// </summary>
        [BsonElement("LastStatusCode")]
        public string LastStatusCode { get; set; } = "";

        /// <summary>
        /// Lần xử lý thành công cuối
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LastProcessedTime")]
        public DateTime? LastProcessedTime { get; set; } = null;

        /// <summary>
        /// Số lần xử lý lỗi
        /// </summary>
        [BsonElement("ErrorProcessCount")]
        public int ErrorProcessCount { get; set; } = 0;

        /// <summary>
        /// Số lần xử lý thấy page không có update conent
        /// </summary>
        [BsonElement("ProcessNoUpdateCount")]
        public int ProcessNoUpdateCount { get; set; } = 0;

        /// <summary>
        /// Số lần xử lý tìm link không thấy có link mới, 
        /// Cứ 10 lần không tìm thấy link mới sẽ bị tụt một rank link
        /// Cứ có link mới sẽ được tăng 1 rank tối đã là đến rank 9
        /// </summary>
        [BsonElement("NoNewLinkFoundCount")]
        public int NoNewLinkFoundCount { get; set; } = 0;

        /// <summary>
        /// Represent this page is create by site map
        /// </summary>
        [BsonElement("IsSiteMapPage")]
        public bool IsSiteMapPage { get; set; } = false;

    }
}
