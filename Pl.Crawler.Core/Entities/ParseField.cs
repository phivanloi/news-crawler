namespace Pl.Crawler.Core.Entities
{
    public class ParseField : BaseEntity
    {

        /// <summary>
        /// Id cấu hình
        /// </summary>
        public long CrawlConfigId { get; set; }

        /// <summary>
        /// Dữ liệu sẽ được set vào field nào
        /// </summary>
        public string FieldName { get; set; }

    }
}
