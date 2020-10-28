using MongoDB.Bson.Serialization.Attributes;

namespace Pl.Crawler.Core.Entities
{
    public class WebpageContent : MongoBaseEntity
    {

        /// <summary>
        /// Id liên kết với webpage
        /// </summary>
        [BsonElement("WebpageId")]
        public string WebpageId { get; set; }

        /// <summary>
        /// Nội dung  trang sau khi download về
        /// </summary>
        [BsonElement("Content")]
        public string Content { get; set; }

    }
}
