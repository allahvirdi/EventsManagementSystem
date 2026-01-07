namespace EventsManagement.Shared.DTOs
{
    /// <summary>
    /// پاسخ API - ساختار یکنواخت برای تمام پاسخ‌های API
    /// </summary>
    public class ApiResult<TData> where TData : class
    {
        /// <summary>
        /// آیا درخواست موفق بوده است
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// پیام پاسخ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// داده‌های پاسخ
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// خطاهای اعتبارسنجی
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; }

        /// <summary>
        /// کد HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// سازنده برای پاسخ موفق
        /// </summary>
        public static ApiResult<TData> Success(TData data, string message = "عملیات موفقیت‌آمیز", int statusCode = 200)
        {
            return new ApiResult<TData>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// سازنده برای پاسخ ناموفق
        /// </summary>
        public static ApiResult<TData> Failure(string message = "خطایی رخ داده است", int statusCode = 400, Dictionary<string, string[]> errors = null)
        {
            return new ApiResult<TData>
            {
                IsSuccess = false,
                Message = message,
                Data = null,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// نسخه عمومی بدون داده
    /// </summary>
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResult Success(string message = "عملیات موفقیت‌آمیز", int statusCode = 200)
        {
            return new ApiResult
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResult Failure(string message = "خطایی رخ داده است", int statusCode = 400, Dictionary<string, string[]> errors = null)
        {
            return new ApiResult
            {
                IsSuccess = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}
