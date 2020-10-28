using Pl.Crawler.Core.Entities;

namespace Pl.Crawler.Core.Interfaces
{
    public interface IWebpageContentData : IRepository<WebpageContent>
    {
        /// <summary>
        /// Tạo index cho trường WebpageId
        /// </summary>
        void CreateIndexWebpageId();

        /// <summary>
        /// Hàm phụ trách tạo mới hoặc update trường content
        /// </summary>
        /// <param name="webpageId">Id webpage</param>
        /// <param name="content">Nội dung cần update hoặc thêm mới</param>
        void UpdateOrCreateContent(string webpageId, string content);

        /// <summary>
        /// Lấy nội dung của một webpage theo id
        /// </summary>
        /// <param name="webpageId">Id webpage</param>
        /// <returns>string</returns>
        string GetContent(string webpageId);

    }
}
