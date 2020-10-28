using System.ComponentModel.DataAnnotations;

namespace Pl.Crawler.Core.Entities
{
    public class CrawlConfig : BaseEntity
    {
        /// <summary>
        /// Id website
        /// </summary>
        public long WebsiteId { get; set; }

        /// <summary>
        /// Tên loại page của cấu hình
        /// Chi tiết tin bài
        /// Chi tiết sản phẩm
        /// ...
        /// </summary>
        [Required]
        [StringLength(128)]
        public string PageTypeName { get; set; }

        /// <summary>
        /// Mẫu url
        /// </summary>
        [Required]
        [StringLength(512)]
        public string UrlPattern { get; set; }

        /// <summary>
        /// Trạng thái xuất tự động cho aip nhận dữ liệu sau khi phân tích
        /// </summary>
        public bool AutoExport { get; set; }

        /// <summary>
        /// Thông tin api export
        /// </summary>
        [StringLength(2048)]
        public string ExportApiUrl { get; set; }

    }
}
