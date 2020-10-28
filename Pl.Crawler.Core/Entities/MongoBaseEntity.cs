using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Pl.Crawler.Core.Entities
{
    /// <summary>
    /// Lớp cơ bản của một mongo entity trong hệ thống.
    /// </summary>
    public class MongoBaseEntity
    {
        /// <summary>
        /// Khóa chính của entity
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Thời điểm tạo mới entity
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("CreatedTime")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời điểm cập nhập entity lần cuối cùng
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("UpdatedTime")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Hàm trả về object dạng json
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
