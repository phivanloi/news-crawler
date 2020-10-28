using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pl.Crawler.Core.Entities;
using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pl.Crawler.Data
{
    /// <summary>
    /// Lớp cơ sở xử lý đọc, ghi dữ liệu cho entity framework core
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class EfRepository<Entity> : IRepository<Entity> where Entity : BaseEntity
    {
        #region Properties And Constructor

        /// <summary>
        /// Nội dung db
        /// </summary>
        protected readonly DbContext context;

        /// <summary>
        /// Khởi tạo một base data class
        /// </summary>
        /// <param name="_context">db context</param>
        public EfRepository(DbContext _context)
        {
            context = _context;
        }

        #endregion Properties And Constructor

        #region Helper method

        /// <summary>
        /// Lấy tên bảng của loại đối thượng
        /// </summary>
        /// <returns>object table name</returns>
        public virtual string GetTableName()
        {
            return context.Model.FindEntityType(typeof(Entity)).Relational().TableName;
        }

        /// <summary>
        /// Lấy tên bảng của loại đối thượng khác cùng một db context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>object table name</returns>
        public virtual string GetTableName<TEntity>()
        {
            return context.Model.FindEntityType(typeof(TEntity)).Relational().TableName;
        }

        /// <summary>
        /// Ghi lại tất cả thay đổi của db context vào database
        /// </summary>
        /// <returns>int</returns>
        public virtual int SaveChanges()
        {
            return context.SaveChanges();
        }

        /// <summary>
        /// Ghi lại tất cả thay đổi của db context vào database bất đồng bộ
        /// </summary>
        /// <returns>int</returns>
        public virtual Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        #endregion Helper method

        #region Check method

        /// <summary>
        /// Lấy một object theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual bool Any(Expression<Func<Entity, bool>> predicate)
        {
            return context.Set<Entity>().Any(predicate);
        }

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await context.Set<Entity>().AnyAsync(predicate);
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
            return context.Set<Entity>().Find(pk);
        }

        /// <summary>
        /// Tìm đối tượng bất đồng bộ theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindByKeyAsync(object pk)
        {
            return await context.Set<Entity>().FindAsync(pk);
        }

        /// <summary>
        /// Tìm đối tượng theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual Entity FindByKeyNoTracking(object pk)
        {
            Entity entity = context.Set<Entity>().Find(pk);
            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        /// <summary>
        /// Tìm đối tượng bất đồng bộ theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindByKeyNoTrackingAsync(object pk)
        {
            Entity entity = await context.Set<Entity>().FindAsync(pk);
            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        /// <summary>
        /// Lấy một object theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual Entity Find(Expression<Func<Entity, bool>> predicate)
        {
            return context.Set<Entity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Lấy một object theo điều kiện, và không theo dõi đối tượng sau khi lấy ra
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual Entity FindNoTracking(Expression<Func<Entity, bool>> predicate)
        {
            return context.Set<Entity>().AsNoTracking().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await context.Set<Entity>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện, và không theo dõi đối tượng sau khi lấy ra
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        public virtual async Task<Entity> FindNoTrackingAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await context.Set<Entity>().AsNoTracking().FirstOrDefaultAsync(predicate);
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
            return context.Set<Entity>().Where(predicate);
        }

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="predicate">Điều kiện lấy</param>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> FindAllNoTracking(Expression<Func<Entity, bool>> predicate)
        {
            return context.Set<Entity>().AsNoTracking().Where(predicate);
        }

        /// <summary>
        /// Lấy toàn bộ tập hợp các đối tượng
        /// </summary>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> FindAll()
        {
            return context.Set<Entity>();
        }

        /// <summary>
        /// Lấy toàn bộ tập hợp các đối tượng và không theo dõi thay đổi
        /// </summary>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> FindAllNoTracking()
        {
            return context.Set<Entity>().AsNoTracking();
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
            if (predicate != null)
            {
                return context.Set<Entity>().Count(predicate);
            }
            return context.Set<Entity>().Count();
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
                return await context.Set<Entity>().CountAsync(predicate);
            }
            return await context.Set<Entity>().CountAsync();
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

            context.Set<Entity>().Add(entity);

            return context.SaveChanges() > 0;
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

            await context.Set<Entity>().AddAsync(entity);
            return await context.SaveChangesAsync() > 0;
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

            context.Set<Entity>().AddRange(entities);
            return context.SaveChanges() > 0;
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

            await context.Set<Entity>().AddRangeAsync(entities);
            return await context.SaveChangesAsync() > 0;
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
            context.Set<Entity>().Update(entity);
            return context.SaveChanges() > 0;
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
            context.Set<Entity>().Update(entity);
            return await context.SaveChangesAsync() > 0;
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
            }
            context.Set<Entity>().UpdateRange(entities);
            return context.SaveChanges() > 0;
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
            }
            context.Set<Entity>().UpdateRange(entities);
            return await context.SaveChangesAsync() > 0;
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
            string query = $"TRUNCATE TABLE {GetTableName()}";
            context.Database.ExecuteSqlCommand(query);
            return true;
        }

        /// <summary>
        /// Xóa toàn bộ dự liệu trong bản và restart lại index. Bất đồng bộ
        /// </summary>
        /// <returns>bool</returns>
        public virtual async Task<bool> TruncateAsync()
        {
            string query = $"TRUNCATE TABLE {GetTableName()}";
            return await context.Database.ExecuteSqlCommandAsync(query) > 0;
        }

        /// <summary>
        /// Xóa một tập hợp dữ liệu, đảm bảo không còn giàn buộc dữ liệu khi gọi hàm này
        /// </summary>
        /// <param name="entities">Tập hợp dữ liệu</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <returns>bool</returns>
        public virtual bool Delete(IEnumerable<Entity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            context.Set<Entity>().RemoveRange(entities);
            return context.SaveChanges() > 0;
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

            context.Set<Entity>().RemoveRange(entities);
            return await context.SaveChangesAsync() > 0;
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

            context.Set<Entity>().Remove(entity);
            return context.SaveChanges() > 0;
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

            context.Set<Entity>().Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }

        #endregion Delete methods

        #region QueryString Method

        /// <summary>
        /// Hàm chạy một sql command
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <returns>bool</returns>
        public virtual bool ExecuteSqlCommand(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = context.Database.BeginTransaction(isolationLevel))
            {
                int dataQuery = context.Database.ExecuteSqlCommand(queryCommand);
                dbContextTransaction.Commit();
                return dataQuery > 0;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command bất đồng bộ
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> ExecuteSqlCommandAsync(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = await context.Database.BeginTransactionAsync(isolationLevel))
            {
                int dataQuery = await context.Database.ExecuteSqlCommandAsync(queryCommand);
                dbContextTransaction.Commit();
                return dataQuery > 0;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <param name="parameters">parameters</param>
        /// <returns>bool</returns>
        public virtual bool ExecuteSqlCommand(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, params object[] parameters)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = context.Database.BeginTransaction(isolationLevel))
            {
                int dataQuery = context.Database.ExecuteSqlCommand(queryCommand, parameters);
                dbContextTransaction.Commit();
                return dataQuery > 0;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command bất đồng bộ
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <param name="parameters">parameters</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> ExecuteSqlCommandAsync(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, params object[] parameters)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = await context.Database.BeginTransactionAsync(isolationLevel))
            {
                int dataQuery = await context.Database.ExecuteSqlCommandAsync(queryCommand, default(CancellationToken), parameters);
                dbContextTransaction.Commit();
                return dataQuery > 0;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <returns>IQueryable Entity</returns>
        public virtual IEnumerable<Entity> SqlQuery(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = context.Database.BeginTransaction(isolationLevel))
            {
                IQueryable<Entity> dataQuery = context.Set<Entity>().FromSql(queryCommand);
                dbContextTransaction.Commit();
                return dataQuery;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command bất đồng bộ
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <returns>IQueryable Entity</returns>
        public virtual async Task<IEnumerable<Entity>> SqlQueryAsync(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = await context.Database.BeginTransactionAsync(isolationLevel))
            {
                IQueryable<Entity> dataQuery = context.Set<Entity>().FromSql(queryCommand);
                dbContextTransaction.Commit();
                return dataQuery;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <param name="parameters">Danh sách các sql parameters</param>
        /// <returns>List Entity</returns>
        public virtual IEnumerable<Entity> SqlQuery(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, params object[] parameters)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = context.Database.BeginTransaction(isolationLevel))
            {
                IQueryable<Entity> dataQuery = context.Set<Entity>().FromSql(queryCommand, parameters);
                dbContextTransaction.Commit();
                return dataQuery;
            }
        }

        /// <summary>
        /// Hàm chạy một sql command
        /// </summary>
        /// <param name="queryCommand">cấu query cần chạy</param>
        /// <param name="isolationLevel">Mức độ khóa dữ liệu khi thực hiện execute</param>
        /// <param name="parameters">Danh sách các sql parameters</param>
        /// <returns>List Entity</returns>
        public virtual async Task<IEnumerable<Entity>> SqlQueryAsync(string queryCommand, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, params object[] parameters)
        {
            if (string.IsNullOrEmpty(queryCommand))
            {
                throw new ArgumentException($"Required input 'queryCommand' was empty.", queryCommand);
            }

            using (IDbContextTransaction dbContextTransaction = await context.Database.BeginTransactionAsync(isolationLevel))
            {
                IQueryable<Entity> dataQuery = context.Set<Entity>().FromSql(queryCommand, parameters);
                dbContextTransaction.Commit();
                return dataQuery;
            }
        }

        #endregion QueryString Method
    }
}