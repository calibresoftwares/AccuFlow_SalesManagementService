using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.ResponseTypes
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Response { get; set; }

        public static ApiResponse<T> Success(T response, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Status = "Success",
                StatusCode = statusCode,
                Message = message,
                Response = response
            };
        }

        public static ApiResponse<T> Failure(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Status = "Failure",
                StatusCode = statusCode,
                Message = message,
                Response = default
            };
        }
    }
}
