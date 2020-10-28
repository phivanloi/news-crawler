using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pl.Crawler.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Pl.Crawelr.Caching
{
    public class SqlCacheService : ICacheService
    {
        #region Properties And Constructor

        /// <summary>
        /// Dịch vụ cache phân tán
        /// </summary>
        private readonly IDistributedCache distributedCache;

        private readonly SqlServerCacheOptions sqlServerCacheOptions;

        public SqlCacheService(IDistributedCache _distributedCache, IOptions<SqlServerCacheOptions> _option)
        {
            distributedCache = _distributedCache ?? throw new Exception("No base sql cached service is registered");
            sqlServerCacheOptions = _option.Value;
        }

        #endregion Properties And Constructor

        #region Get Method

        /// <summary>
        /// Lấy đối tượng từ khóa cache và trả về với kiểu được chỉ định
        /// </summary>
        /// <typeparam name="TItem">Kiểu đối tượng cần nhận</typeparam>
        /// <param name="key">Khóa</param>
        /// <returns>TItem object</returns>
        public TItem Get<TItem>(string key)
        {
            byte[] cacheData = distributedCache.Get(key);
            if (cacheData != null)
            {
                JsonConvert.DeserializeObject<TItem>(Encoding.UTF8.GetString(cacheData));
            }
            return default;
        }

        /// <summary>
        /// Lấy đối tượng từ khóa cache nếu chưa có cache sẽ gọi hàm lấy đối tuộng
        /// </summary>
        /// <typeparam name="TItem">Loại đối tượng</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <param name="factory">Hàm lấy đối tượng nếu cache null</param>
        /// <returns>TItem object</returns>
        public TItem GetOrCreate<TItem>(string key, Func<TItem> factory)
        {
            TItem cacheData = Get<TItem>(key);
            if (!EqualityComparer<TItem>.Default.Equals(cacheData, default))
            {
                return cacheData;
            }
            TItem funtionData = factory();
            return Set(key, funtionData);
        }

        /// <summary>
        /// Lấy đối tượng từ khóa cache nếu chưa có cache sẽ gọi hàm lấy đối tuộng
        /// </summary>
        /// <typeparam name="TItem">Loại đối tượng</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <param name="factory">Hàm lấy đối tượng nếu cache null</param>
        /// <param name="time">Thời gian tồn tại cache tính bắng giây</param>
        /// <returns>TItem object</returns>
        public TItem GetOrCreate<TItem>(string key, Func<TItem> factory, int time)
        {
            TItem cacheData = Get<TItem>(key);
            if (!EqualityComparer<TItem>.Default.Equals(cacheData, default))
            {
                return cacheData;
            }
            TItem funtionData = factory();
            return Set(key, funtionData, time);
        }

        /// <summary>
        /// Lấy đối tượng từ khóa cache nếu chưa có cache sẽ gọi hàm lấy đối tượng, hàm bất đồng bộ
        /// </summary>
        /// <typeparam name="TItem">Loại đối tượng</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <param name="factory">Hàm lấy đối tượng nếu cache null</param>
        /// <returns>TItem object</returns>
        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory)
        {
            TItem cacheData = Get<TItem>(key);
            if (!EqualityComparer<TItem>.Default.Equals(cacheData, default))
            {
                return cacheData;
            }
            TItem funtionData = await factory();
            Set(key, funtionData);
            return funtionData;
        }

        /// <summary>
        /// Lấy đối tượng từ khóa cache nếu chưa có cache sẽ gọi hàm lấy đối tượng, hàm bất đồng bộ
        /// </summary>
        /// <typeparam name="TItem">Loại đối tượng</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <param name="factory">Hàm lấy đối tượng nếu cache null</param>
        /// <param name="time">Thời gian tồn tại cache tính bắng giây</param>
        /// <returns>TItem object</returns>
        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory, int time)
        {
            TItem cacheData = Get<TItem>(key);
            if (!EqualityComparer<TItem>.Default.Equals(cacheData, default))
            {
                return cacheData;
            }
            TItem funtionData = await factory();
            Set(key, funtionData, time);
            return funtionData;
        }

        #endregion Get Method

        #region Set Method

        /// <summary>
        /// Ghi một giá trị vào cache
        /// </summary>
        /// <typeparam name="TItem">Loại đối tượng cần set</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <param name="value">Giá trị</param>
        /// <returns>TItem</returns>
        public TItem Set<TItem>(string key, TItem value)
        {
            distributedCache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
            return value;
        }

        /// <summary>
        /// Ghi một giá trị vào cache
        /// </summary>
        /// <typeparam name="TItem">Loại đối tượng cần set</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <param name="value">Giá trị</param>
        /// <param name="time">Thời gian lưu trong cache tính bằng giây</param>
        /// <returns>Trả lại đối tượng TItem</returns>
        public TItem Set<TItem>(string key, TItem value, int time)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(time));
            distributedCache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), options);
            return value;
        }

        #endregion Set Method

        #region Remove Method

        /// <summary>
        /// Gỡ bỏ cache theo khóa cache
        /// </summary>
        /// <param name="key">Khóa cache</param>
        public void Remove(string key)
        {
            distributedCache.Remove(key);
        }

        /// <summary>
        /// Gỡ bỏ cache theo khóa cache bất đồng bộ
        /// </summary>
        /// <param name="key">Khóa cache</param>
        public async Task RemoveAsync(string key)
        {
            await distributedCache.RemoveAsync(key);
        }

        /// <summary>
        /// Gõ bỏ cache theo mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        public void RemoveByPattern(string pattern)
        {
            SqlCommandExecute($"DELETE [{sqlServerCacheOptions.SchemaName}].[{sqlServerCacheOptions.TableName}] WHERE Id like N'%' + @cacheKey + '%'", new SqlParameter() { ParameterName = "@cacheKey", Value = pattern, SqlDbType = SqlDbType.VarChar, Size = 449 });
        }

        /// <summary>
        /// Gõ bỏ cache theo mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        public async Task RemoveByPatternAsync(string pattern)
        {
            SqlConnection sqlConnection = new SqlConnection(sqlServerCacheOptions.ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand($"DELETE [{sqlServerCacheOptions.SchemaName}].[{sqlServerCacheOptions.TableName}] WHERE Id like N'%' + @cacheKey + '%'");
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@cacheKey", Value = pattern, SqlDbType = SqlDbType.VarChar, Size = 449 });
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;
                sqlConnection.Open();
                await cmd.ExecuteScalarAsync();
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// Xóa hết dữ liệu cache
        /// </summary>
        public virtual void Clear()
        {
            SqlCommandExecute($"TRUNCATE TABLE [{sqlServerCacheOptions.SchemaName}].[{sqlServerCacheOptions.TableName}]", null);
        }

        /// <summary>
        /// Hỗ trợ gọi sql ExecuteScalar và không trà về data
        /// </summary>
        /// <param name="query">Câu query cần chạy</param>
        /// <param name="sqlParameter">Tham số cần truyền query nếu có</param>
        private void SqlCommandExecute(string query, SqlParameter sqlParameter)
        {
            SqlConnection sqlConnection = new SqlConnection(sqlServerCacheOptions.ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand(query);
                if (sqlParameter != null)
                {
                    cmd.Parameters.Add(sqlParameter);
                }
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;
                sqlConnection.Open();
                cmd.ExecuteScalar();
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        #endregion Remove Method
    }
}