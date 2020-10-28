using Pl.Crawler.Core.Entities;

namespace Pl.Crawler.Core.Interfaces
{
    public interface IDataExportData : IRepository<DataExport>
    {

        /// <summary>
        /// Tạo index cho trường ExistKey
        /// </summary>
        void CreateIndexForExistKey();

    }
}
