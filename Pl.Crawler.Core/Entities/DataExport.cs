using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Pl.Crawler.Core.Entities
{
    public class DataExport : MongoBaseEntity
    {

        /// <summary>
        /// Id trang web
        /// </summary>
        [BsonElement("WebpageId")]
        public string WebpageId { get; set; }

        /// <summary>
        /// Id cấu hình
        /// </summary>
        [BsonElement("CrawlConfigId")]
        public long CrawlConfigId { get; set; }

        /// <summary>
        /// Id website
        /// </summary>
        [BsonElement("WebsiteId")]
        public long WebsiteId { get; set; }

        /// <summary>
        /// Khóa check
        /// </summary>
        [BsonElement("ExistKey")]
        public string ExistKey { get; set; }

        /// <summary>
        /// Đây là một json data dang key value với key là 
        /// </summary>
        [BsonElement("PaserData")]
        public string PaserData { get; set; }

        /// <summary>
        /// Thời gian paser 
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LastPaserTime")]
        public DateTime LastPaserTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian paser 
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LastExportTime")]
        public DateTime? LastExportTime { get; set; } = null;

        /// <summary>
        /// Đây là dữ liệu thêm mới
        /// </summary>
        [BsonElement("IsNew")]
        public bool IsNew { get; set; } = true;

        /// <summary>
        /// Đây là dữ liệu cần update
        /// </summary>
        [BsonElement("IsUpdate")]
        public bool IsUpdate { get; set; } = false;

        /// <summary>
        /// Số lần update
        /// </summary>
        [BsonElement("UpdateCount")]
        public int UpdateCount { get; set; } = 0;

        /// <summary>
        /// Cảnh bảo xử lý khi có 1 hoặc nhiều phân tích trường không có dữ liệu
        /// </summary>
        [BsonElement("IsWarningData")]
        public bool IsWarningData { get; set; } = false;

    }
}
