using Newtonsoft.Json;
using System;

namespace Pl.Crawler.Core.Entities
{
    /// <summary>
    /// Lớp cơ bản của một entity trong hệ thống.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Khóa chính của entity
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Thời điểm tạo mới entity
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời điểm cập nhập entity lần cuối cùng
        /// </summary>
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
