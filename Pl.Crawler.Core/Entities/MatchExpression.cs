using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pl.Crawler.Core.Entities
{
    public class MatchExpression
    {
        /// <summary>
        /// Danh sách regex có cần kiểm tra
        /// </summary>
        public List<Regex> Regexes { get; set; }

        /// <summary>
        /// hành động được thực hiện khi có regex math
        /// </summary>
        public Func<Match, string> Action { get; set; }
    }
}