using System;
using System.Threading.Tasks;

namespace Pl.Crawler.Core.Interfaces
{
    /// <summary>
    /// Lớp chứa toàn bộ phương thức log trong hệ thống
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Hàm ghi lỗi log dạng info
        /// </summary>
        /// <param name="message">Nội dung ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        void Info(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi lỗi log dạng info
        /// </summary>
        /// <param name="message">Nội dung ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        Task InfoAsync(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi lỗi log lỗi
        /// </summary>
        /// <param name="ex">Một error</param>
        void Error(Exception ex);

        /// <summary>
        /// Hàm ghi lỗi log lỗi
        /// </summary>
        /// <param name="ex">Một error</param>
        Task ErrorAsync(Exception ex);

        /// <summary>
        /// Hàm ghi cảnh báo trên hệ thống
        /// </summary>
        /// <param name="message">Tiêu đề ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        void Warn(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi cảnh báo trên hệ thống
        /// </summary>
        /// <param name="message">Tiêu đề ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        Task WarnAsync(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi Debug trên hệ thống
        /// </summary>
        /// <param name="message">Tiêu đề ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        void Debug(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi Debug trên hệ thống
        /// </summary>
        /// <param name="message">Tiêu đề ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        Task DebugAsync(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi Fatal trên hệ thống
        /// </summary>
        /// <param name="message">Tiêu đề ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        void Fatal(string message, string fullMessage = "");

        /// <summary>
        /// Hàm ghi Fatal trên hệ thống
        /// </summary>
        /// <param name="message">Tiêu đề ngắn</param>
        /// <param name="fullMessage">Nội đầy đủ</param>
        Task FatalAsync(string message, string fullMessage = "");
    }
}