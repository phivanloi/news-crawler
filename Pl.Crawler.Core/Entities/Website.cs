using System.ComponentModel.DataAnnotations;

namespace Pl.Crawler.Core.Entities
{
    public class Website : BaseEntity
    {

        /// <summary>
        /// Domain của website
        /// </summary>
        [Required]
        [StringLength(2048)]
        public string Domain { get; set; }

        /// <summary>
        /// Tên website
        /// </summary>
        [StringLength(512)]
        public string Name { get; set; }

        /// <summary>
        /// Cấp web site
        /// </summary>
        public float Rank { get; set; }

        /// <summary>
        /// Trạng thái kích hoạt nguồn
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Chỉ tìm kiếm link mới ở page site mapp
        /// </summary>
        public bool FindLinkOnlySiteMap { get; set; } = false;

    }
}
