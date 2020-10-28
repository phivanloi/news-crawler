using System.Collections.Generic;

namespace Pl.Crawler.Core.Constants
{
    /// <summary>
    /// Lấy tin tự động
    /// </summary>
    public static class CrawlConstants
    {
        /// <summary>
        /// Thời gian cho lần download tiếp theo được cộng thêm vào nếu task download bị lỗi
        /// </summary>
        public static int ErrorDownloadTimeAppend = 3600;

        /// <summary>
        /// Số lần lỗi download tối đa để tụt rank
        /// </summary>
        public static int MaxErrorDownloadCount = 5;

        /// <summary>
        /// Số lần xử lý lỗi tối đã để page bị tụt rank
        /// </summary>
        public static int MaxProcessNoUpdateCount = 2;

        /// <summary>
        /// Số lần tìm không thấy có link mới tối đa để bị tụt rank
        /// </summary>
        public static int MaxNoNewLinkFindCount = 2;

        /// <summary>
        /// khóa kiểm tra không export tự động khi tiêu đề hoặc nội dung có từ khóa này
        /// </summary>
        public static string NotExprotKey = "[NotAutoExport]";

        public static class PaserFieldName
        {

            /// <summary>
            /// Khóa lấy ngôn ngữ của loại parser
            /// </summary>
            public const string LanguageStringParserType = "constants.parsertype.";

            /// <summary>
            /// Loại paser theo xpart
            /// </summary>
            public const int ParserTypeXpath = 0;

            /// <summary>
            /// Loại paser theo regex
            /// </summary>
            public const int ParserTypeRegex = 1;

            private static List<int> _listParserType;

            /// <summary>
            /// Danh sách các trường hỗ trợ crawl trong hệ thống
            /// </summary>
            public static List<int> ListParserType => _listParserType ?? (_listParserType = new List<int>() {
                ParserTypeXpath,//constants.parsertype.0
                ParserTypeRegex,//constants.parsertype.1
            });

        }
    }
}