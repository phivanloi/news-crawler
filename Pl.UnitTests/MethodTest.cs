using System;
using System.Net.Http;
using System.Text;
using Pl.Crawler.Core.Constants;
using Xunit;

namespace Pl.UnitTests
{
    public class MethodTest
    {
        [Fact]
        public void FindFullDomainUrl()
        {
            var domain = "cafef.vn";
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 0, 10);
                httpClient.DefaultRequestHeaders.Add("User-Agent", CommonConstants.RequestUserAgent);
                var content = Encoding.UTF8.GetString(httpClient.GetByteArrayAsync(domain).Result);
            }
        }

    }
}
