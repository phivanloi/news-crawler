namespace Pl.Crawler.Core.Entities
{
    public class ReplateRule : BaseEntity
    {
        /// <summary>
        /// Id phân trích trường
        /// </summary>
        public long ParseFieldId { get; set; }

        /// <summary>
        /// Chứa data hoặc regex hoặc html paser để remove
        /// </summary>
        public string SelectKey { get; set; }

        /// <summary>
        /// Chứa data thay thế
        /// </summary>
        public string ReplaceData { get; set; }

        /// <summary>
        /// Sử dụng regex hoặt html paser để lấy dữ liệu
        /// 0 là Xpart
        /// 1 là regex
        /// </summary>
        public int ParserType { get; set; }
    }
}
