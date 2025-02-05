namespace Talabat.APIs.Errors
{
    public class ApiExceptionHandler : ApiResponse
    {
        public string? Details { get; set; }
        public ApiExceptionHandler(int statusCode , string? message = null, string? details = null)
            :base(statusCode , message)
        {
            Details = details ?? string.Empty;
        }
    }
}
