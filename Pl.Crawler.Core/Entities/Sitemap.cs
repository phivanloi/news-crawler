using System.ComponentModel.DataAnnotations;

namespace Pl.Crawler.Core.Entities
{
    /// <summary>
    /// Thông tin site mapp
    /// </summary>
    public class Sitemap : BaseEntity
    {

        public long WebsiteId { get; set; }

        /// <summary>
        /// Cấp web site
        /// </summary>
        [Required]
        [StringLength(2048)]
        public string Url { get; set; }

        /// <summary>
        /// Cấp web site
        /// </summary>
        public int DownloadRank { get; set; } = 9;

    }
}
