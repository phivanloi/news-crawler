using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;

namespace Pl.Crawler.Data
{
    public class MogoRepository<Entity> : IRepository<Entity> where Entity : MongoBaseEntity
    {
        #region Properties And Constructor

        /// <summary>
        /// danh sách Entity
        /// </summary>
        protected readonly IMongoCollection<Entity> Entitys;

        /// <summary>
        /// Db mongo
        /// </summary>
        protected readonly IMongoDatabase mongoDatabase;

        /// <summary>
        /// Khởi tạo một base data class
        /// </summary>
        public MogoRepository(string dbConnection)
        {
            var mongoUrl = MongoUrl.Create(dbConnection);
            var client = new MongoClient(mongoUrl);
            mongoDatabase = client.GetDatabase(mongoUrl.DatabaseName);
            Entitys = mongoDatabase.GetCollection<Entity>(typeof(Entity).Name);
        }

        #endregion Properties And Constructor

        #region Check method

        /// <summary>
        /// Lấy một object theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual bool Any(Expression<Func<Entity, bool>> predicate)
        {
            return Entitys.CountDocuments(predicate) > 0;
        }

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await Entitys.CountDocumentsAsync(predicate) > 0;
        }

        #endregion Check method

        #region Get method

        /// <summary>
        /// Tìm đối tượng theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual Entity FindByKey(object pk)
        {
            return Entitys.Find(q => q.Id == pk.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// Tìm đối tượng bất đồng bộ theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindByKeyAsync(object pk)
        {
            return (await Entitys.FindAsync(q => q.Id == pk.ToString())).FirstOrDefault();
        }

        /// <summary>
        /// Tìm đối tượng theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual Entity FindByKeyNoTracking(object pk)
        {
            return Entitys.Find(q => q.Id == pk.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// Tìm đối tượng bất đồng bộ theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindByKeyNoTrackingAsync(object pk)
        {
            return (await Entitys.FindAsync(q => q.Id == pk.ToString())).FirstOrDefault();
        }

        /// <summary>
        /// Lấy một object theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual Entity Find(Expression<Func<Entity, bool>> predicate)
        {
            return Entitys.Find(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Lấy một object theo điều kiện, và không theo dõi đối tượng sau khi lấy ra
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual Entity FindNoTracking(Expression<Func<Entity, bool>> predicate)
        {
            return Entitys.Find(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindAsync(Expression<Func<Entity, bool>> predicate)
        {
            return (await Entitys.FindAsync(predicate)).FirstOrDefault();
        }

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện, và không theo dõi đối tượng sau khi lấy ra
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindNoTrackingAsync(Expression<Func<Entity, bool>> predicate)
        {
            return (await Entitys.FindAsync(predicate)).FirstOrDefault();
        }

        #endregion Get method

        #region Listing Methods

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="predicate">Điều kiện lấy</param>
        /// <returns>List Entity</returns>
        public virtual IEnumerable<Entity> FindAll(Expression<Func<Entity, bool>> predicate)
        {
            return Entitys.Find(predicate).ToList();
        }

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="predicate">Điều kiện lấy</param>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> FindAllNoTracking(Expression<Func<Entity, bool>> predicate)
        {
            return Entitys.Find(predicate).ToList();
        }

        /// <summary>
        /// Lấy toàn bộ tập hợp các đối tượng
        /// </summary>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> FindAll()
        {
            return Entitys.Find(q => true).ToList();
        }

        /// <summary>
        /// Lấy toàn bộ tập hợp các đối tượng và không theo dõi thay đổi
        /// </summary>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> FindAllNoTracking()
        {
            return Entitys.Find(q => true).ToList();
        }

        #endregion Listing Methods

        #region Counter

        /// <summary>
        /// lấy tổng số bản ghi dựa vào biểu thức điều kiện
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện, để null nếu muốn lấy tất cả</param>
        /// <returns></returns>
        public virtual long Count(Expression<Func<Entity, bool>> predicate)
        {
            return Entitys.CountDocuments(predicate);
        }

        /// <summary>
        /// lấy tổng số bản ghi dựa vào biểu thức điều kiện
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện, để null nếu muốn lấy tất cả</param>
        /// <returns></returns>
        public virtual async Task<long> CountAsync(Expression<Func<Entity, bool>> predicate)
        {
            if (predicate != null)
            {
                return await Entitys.CountDocumentsAsync(q => true);
            }
            return await Entitys.CountDocumentsAsync(predicate);
        }

        #endregion Counter

        #region Insert methods

        /// <summary>
        /// Thêm mới một entity
        /// </summary>
        /// <param name="entity">đối tượng cần thêm mới</param>
        /// <returns>bool</returns>
        public virtual bool Insert(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Entitys.InsertOne(entity);
            return true;
        }

        /// <summary>
        /// Thêm mới một entity bất đồng bộ
        /// </summary>
        /// <param name="entity">đối tượng cần thêm mới</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> InsertAsync(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Entitys.InsertOneAsync(entity);
            return true;
        }

        /// <summary>
        /// Thêm mới một tập hợp các đối tượng
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng</param>
        /// <returns>bool</returns>
        public virtual bool Insert(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            Entitys.InsertMany(entities);
            return true;
        }

        /// <summary>
        /// Thêm mới một tập hợp các đối tượng bất đồng bộ
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> InsertAsync(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await Entitys.InsertManyAsync(entities);
            return true;
        }

        #endregion Insert methods

        #region Update methods

        /// <summary>
        /// Cập nhập một đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <returns>bool</returns>
        public virtual bool Update(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.UpdatedTime = DateTime.Now;
            var replaceReutl = Entitys.ReplaceOne(q => q.Id == entity.Id, entity);
            return replaceReutl.ModifiedCount > 0;
        }

        /// <summary>
        /// Cập nhập một đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> UpdateAsync(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.UpdatedTime = DateTime.Now;
            var replaceReutl = await Entitys.ReplaceOneAsync(q => q.Id == entity.Id, entity);
            return replaceReutl.ModifiedCount > 0;
        }

        /// <summary>
        /// Cập nhập một tập hợp các đối tượng
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng cần thêm mới</param>
        public virtual bool Update(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            foreach (var item in entities)
            {
                item.UpdatedTime = DateTime.Now;
                Entitys.ReplaceOne(q => q.Id == item.Id, item);
            }
            return true;
        }

        /// <summary>
        /// Cập nhập một tập hợp các đối tượng bất đồng bộ
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng cần thêm mới</param>
        public virtual async Task<bool> UpdateAsync(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            foreach (var item in entities)
            {
                item.UpdatedTime = DateTime.Now;
                await Entitys.ReplaceOneAsync(q => q.Id == item.Id, item);
            }
            return true;
        }

        #endregion Update methods

        #region Delete methods

        /// <summary>
        /// Xóa toàn bộ dự liệu trong bản và restart lại index.
        /// Khuyên lên dùng cho bảng không có khóa ngoại hoặc trường dữ liệu không còn giàn buộc
        /// </summary>
        /// <returns>bool</returns>
        public virtual bool Truncate()
        {
            mongoDatabase.DropCollection(typeof(Entity).Name);
            return true;
        }

        /// <summary>
        /// Xóa toàn bộ dự liệu trong bản và restart lại index. Bất đồng bộ
        /// </summary>
        /// <returns>bool</returns>
        public virtual async Task<bool> TruncateAsync()
        {
            await mongoDatabase.DropCollectionAsync(typeof(Entity).Name);
            return true;
        }

        /// <summary>
        /// Xóa một tập hợp dữ liệu, đảm bảo không còn giàn buộc dữ liệu khi gọi hàm này
        /// </summary>
        /// <param name="entities">Tập hợp dữ liệu</param>
        /// <returns>bool</returns>
        public virtual bool Delete(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var ids = entities.Select(q => q.Id).ToList();
            var deleteResult = Entitys.DeleteMany(q => ids.Contains(q.Id));
            return deleteResult.DeletedCount > 0;
        }

        /// <summary>
        /// Xóa một tập hợp dữ liệu, đảm bảo không còn giàn buộc dữ liệu khi gọi hàm này bất đồng bộ
        /// </summary>
        /// <param name="entities">Tập hợp dữ liệu</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> DeleteAsync(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var ids = entities.Select(q => q.Id).ToList();
            var deleteResult = await Entitys.DeleteManyAsync(q => ids.Contains(q.Id));
            return deleteResult.DeletedCount > 0;
        }

        /// <summary>
        /// Xóa một đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng thật cần xóa</param>
        /// <returns>bool</returns>
        public virtual bool Delete(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var deleteResult = Entitys.DeleteOne(q => q.Id == entity.Id);
            return deleteResult.DeletedCount > 0;
        }

        /// <summary>
        /// Xóa một đối tượng bất đồng bộ
        /// </summary>
        /// <param name="entity">Đối tượng thật cần xóa</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> DeleteAsync(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var deleteResult = await Entitys.DeleteOneAsync(q => q.Id == entity.Id);
            return deleteResult.DeletedCount > 0;
        }

        #endregion Delete methods
    }
}
