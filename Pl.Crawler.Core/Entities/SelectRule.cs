namespace Pl.Crawler.Core.Entities
{
    public class SelectRule : BaseEntity
    {
        /// <summary>
        /// Id phân trích trường
        /// </summary>
        public long ParseFieldId { get; set; }

        /// <summary>
        /// Sử dụng giá trị mặc định thì set nguyên DefaultData vào Field cần paser
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// Giá trị được sét mặt định
        /// </summary>
        public string DefaultData { get; set; }

        /// <summary>
        /// chữa chuỗi regex, chuỗi paser html
        /// </summary>
        public string SelectKey { get; set; }

        /// <summary>
        /// Sử dụng regex hoặt html paser để lấy dữ liệu
        /// 0 là Xpart
        /// 1 là regex
        /// </summary>
        public int ParserType { get; set; } = 0;

        /// <summary>
        /// Dữ liệu có là dạng html
        /// </summary>
        public bool IsHtml { get; set; } = false;

        /// <summary>
        /// Có lấy tất cả dữ liệu nếu tìm thấy hay chỉ lấy 1
        /// </summary>
        public bool SelectMultiple { get; set; } = false;

    }
}
