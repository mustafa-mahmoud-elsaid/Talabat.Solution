
using System;

namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? ErrorMessageToReturn(statusCode);
        }

        private string? ErrorMessageToReturn(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have made",
                401=> "Authorized, you are not",
                404  => "Resource was not found",
                500  => "Errors are the path to the dark side.Errors lead to anger. anger leads to death",
                _ => null,
            };
        }
    }
}
