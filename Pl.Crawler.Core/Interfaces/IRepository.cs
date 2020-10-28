using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pl.Crawler.Core.Interfaces
{
    /// <summary>
    /// Định nhĩa các hàm cơ bản để làm việc đọc ghi dữ liệu cho một đối tượng trong db
    /// </summary>
    /// <typeparam name="Entity">Loại đối tượng</typeparam>
    public interface IRepository<Entity>
    {
        #region Check method

        /// <summary>
        /// Lấy một object theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        bool Any(Expression<Func<Entity, bool>> predicate);

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        Task<bool> AnyAsync(Expression<Func<Entity, bool>> predicate);

        #endregion Check method

        #region Get method

        /// <summary>
        /// Tìm đối tượng theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        Entity FindByKey(object pk);

        /// <summary>
        /// Tìm đối tượng bất đồng bộ theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        Task<Entity> FindByKeyAsync(object pk);

        /// <summary>
        /// Tìm đối tượng theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        Entity FindByKeyNoTracking(object pk);

        /// <summary>
        /// Tìm đối tượng bất đồng bộ theo khóa chính đầu tiên
        /// </summary>
        /// <param name="pk">Giá trị khóa chính</param>
        /// <returns>Entity</returns>
        Task<Entity> FindByKeyNoTrackingAsync(object pk);

        /// <summary>
        /// Lấy một object theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        Entity Find(Expression<Func<Entity, bool>> predicate);

        /// <summary>
        /// Lấy một object theo điều kiện, và không theo dõi đối tượng sau khi lấy ra
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        Entity FindNoTracking(Expression<Func<Entity, bool>> predicate);

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        Task<Entity> FindAsync(Expression<Func<Entity, bool>> predicate);

        /// <summary>
        /// Lấy một object bất đồng bộ theo điều kiện, và không theo dõi đối tượng sau khi lấy ra
        /// </summary>
        /// <param name="predicate">Điều kiện cần lấy</param>
        /// <returns>Entity</returns>
        Task<Entity> FindNoTrackingAsync(Expression<Func<Entity, bool>> predicate);

        #endregion Get method

        #region Counter

        /// <summary>
        /// lấy tổng số bản ghi dựa vào biểu thức điều kiện
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện, để null nếu muốn lấy tất cả</param>
        /// <returns></returns>
        long Count(Expression<Func<Entity, bool>> predicate);

        /// <summary>
        /// lấy tổng số bản ghi dựa vào biểu thức điều kiện
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện, để null nếu muốn lấy tất cả</param>
        /// <returns></returns>
        Task<long> CountAsync(Expression<Func<Entity, bool>> predicate);

        #endregion Counter

        #region Listing Methods

        /// <summary>
        /// Lấy toàn bộ tập hợp các đối tượng
        /// </summary>
        /// <returns>List Entity</returns>
        IEnumerable<Entity> FindAll();

        /// <summary>
        /// Lấy toàn bộ tập hợp các đối tượng và không theo dõi thay đổi
        /// </summary>
        /// <returns>List Entity</returns>
        IEnumerable<Entity> FindAllNoTracking();

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="predicate">Điều kiện lấy</param>
        /// <returns>List Entity</returns>
        IEnumerable<Entity> FindAll(Expression<Func<Entity, bool>> predicate);

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="predicate">Điều kiện lấy</param>
        /// <returns>IQueryable Entity</returns>
        IEnumerable<Entity> FindAllNoTracking(Expression<Func<Entity, bool>> predicate);

        #endregion Listing Methods

        #region Insert methods

        /// <summary>
        /// Thêm mới một entity
        /// </summary>
        /// <param name="entity">đối tượng cần thêm mới</param>
        /// <returns>bool</returns>
        bool Insert(Entity entity);

        /// <summary>
        /// Thêm mới một entity bất đồng bộ
        /// </summary>
        /// <param name="entity">đối tượng cần thêm mới</param>
        /// <returns>bool</returns>
        Task<bool> InsertAsync(Entity entity);

        /// <summary>
        /// Thêm mới một tập hợp các đối tượng
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng</param>
        /// <returns>bool</returns>
        bool Insert(IEnumerable<Entity> entities);

        /// <summary>
        /// Thêm mới một tập hợp các đối tượng bất đồng bộ
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng</param>
        /// <returns>bool</returns>
        Task<bool> InsertAsync(IEnumerable<Entity> entities);

        #endregion Insert methods

        #region Update methods

        /// <summary>
        /// Cập nhập một đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <returns>bool</returns>
        bool Update(Entity entity);

        /// <summary>
        /// Cập nhập một đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <returns>bool</returns>
        Task<bool> UpdateAsync(Entity entity);

        /// <summary>
        /// Cập nhập một tập hợp các đối tượng
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng cần thêm mới</param>
        bool Update(IEnumerable<Entity> entities);

        /// <summary>
        /// Cập nhập một tập hợp các đối tượng bất đồng bộ
        /// </summary>
        /// <param name="entities">Tập hợp các đối tượng cần thêm mới</param>
        Task<bool> UpdateAsync(IEnumerable<Entity> entities);

        #endregion Update methods

        #region Delete methods

        /// <summary>
        /// Xóa toàn bộ dự liệu trong bản và restart lại index.
        /// Khuyên lên dùng cho bảng không có khóa ngoại hoặc trường dữ liệu không còn giàn buộc
        /// </summary>
        /// <returns>bool</returns>
        bool Truncate();

        /// <summary>
        /// Xóa toàn bộ dự liệu trong bản và restart lại index. Bất đồng bộ
        /// </summary>
        /// <returns>bool</returns>
        Task<bool> TruncateAsync();

        /// <summary>
        /// Xóa một tập hợp dữ liệu, đảm bảo không còn giàn buộc dữ liệu khi gọi hàm này
        /// </summary>
        /// <param name="entities">Tập hợp dữ liệu</param>
        /// <returns>bool</returns>
        bool Delete(IEnumerable<Entity> entities);

        /// <summary>
        /// Xóa một tập hợp dữ liệu, đảm bảo không còn giàn buộc dữ liệu khi gọi hàm này bất đồng bộ
        /// </summary>
        /// <param name="entities">Tập hợp dữ liệu</param>
        /// <returns>bool</returns>
        Task<bool> DeleteAsync(IEnumerable<Entity> entities);

        /// <summary>
        /// Xóa một đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng thật cần xóa</param>
        /// <returns>bool</returns>
        bool Delete(Entity entity);

        /// <summary>
        /// Xóa một đối tượng bất đồng bộ
        /// </summary>
        /// <param name="entity">Đối tượng thật cần xóa</param>
        /// <returns>bool</returns>
        Task<bool> DeleteAsync(Entity entity);

        #endregion Delete methods

    }
}