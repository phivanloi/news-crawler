using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Settings;

namespace Pl.Crawler.Data
{
    public class WebpageContentData : MogoRepository<WebpageContent>, IWebpageContentData
    {
        public WebpageContentData(IOptions<Connections> options) : base(options.Value.CrawlDataLog)
        {

        }

        /// <summary>
        /// Tạo index cho trường WebpageId
        /// </summary>
        public void CreateIndexWebpageId()
        {
            Entitys.Indexes.CreateOne(new CreateIndexModel<WebpageContent>(Builders<WebpageContent>.IndexKeys.Ascending(q => q.WebpageId)));
        }

        /// <summary>
        /// Hàm phụ trách tạo mới hoặc update trường content
        /// </summary>
        /// <param name="webpageId">Id webpage</param>
        /// <param name="content">Nội dung cần update hoặc thêm mới</param>
        public void UpdateOrCreateContent(string webpageId, string content)
        {
            var webpageContent = Entitys.Find(q => q.WebpageId == webpageId).FirstOrDefault();
            if (webpageContent != null)
            {
                var filter = Builders<WebpageContent>.Filter.Eq(f => f.Id, webpageContent.Id);
                var updater = Builders<WebpageContent>.Update.Set(q => q.Content, content);
                Entitys.UpdateOne(filter, updater);
            }
            else
            {
                Entitys.InsertOne(new WebpageContent()
                {
                    WebpageId = webpageId,
                    Content = content,
                });
            }
        }

        /// <summary>
        /// Lấy nội dung của một webpage theo id
        /// </summary>
        /// <param name="webpageId">Id webpage</param>
        /// <returns>string</returns>
        public string GetContent(string webpageId)
        {
            var webpageContent = Entitys.Find(q => q.WebpageId == webpageId).FirstOrDefault();
            if (webpageContent != null)
            {
                return webpageContent.Content;
            }
            return string.Empty;
        }
    }
}
