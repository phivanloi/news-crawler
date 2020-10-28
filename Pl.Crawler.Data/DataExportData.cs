using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using Pl.Crawler.Core.Settings;

namespace Pl.Crawler.Data
{
    public class DataExportData : MogoRepository<DataExport>, IDataExportData
    {
        public DataExportData(IOptions<Connections> options) : base(options.Value.CrawlDataLog)
        {

        }

        /// <summary>
        /// Tạo index cho trường ExistKey
        /// </summary>
        public void CreateIndexForExistKey()
        {
            Entitys.Indexes.CreateOne(new CreateIndexModel<DataExport>(Builders<DataExport>.IndexKeys.Descending(q => q.ExistKey)));
        }

    }
}
