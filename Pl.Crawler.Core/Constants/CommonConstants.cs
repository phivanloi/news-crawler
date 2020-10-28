using System;

namespace Pl.Crawler.Core.Constants
{
    public static class CommonConstants
    {
        /// <summary>
        /// Url tìm kiếm địa chỉ trên google map
        /// </summary>
        public const string GoogleMapUrl = "http://maps.google.com/maps?f=q&hl={0}&ie=UTF8&oe=UTF8&geocode=&q={1}";

        /// <summary>
        /// Khóa này dùng để cộng với các cache tag helper. và có thể đổi lúc runtime
        /// </summary>
        public static string ViewCacheDefault = DateTime.UtcNow.ToString("ddMMyyyyhhmmssfff");

        /// <summary>
        /// Thời gian cache mặc định tính theo giây
        /// </summary>
        public const int DefaultCacheTime = 60;

        /// <summary>
        /// Thời gian cache mặc định của view action mặc định tính bằng giây
        /// </summary>
        public const int ResponseCacheTimeDefault = 3600;

        /// <summary>
        /// Các request gửi đi sẽ thêm phần user agent này
        /// </summary>
        public static string RequestUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";

        /// <summary>
        /// thay đổi cache cho phần view
        /// </summary>
        public static void UpdateViewCachekey()
        {
            ViewCacheDefault = DateTime.UtcNow.ToString("ddMMyyyyhhmmssfff");
        }
    }
}