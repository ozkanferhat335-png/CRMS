using System;
using System.Collections.Generic;

namespace CRMS.Common.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public static ApiResponse<T> Success(T data, string message = "İşlem başarılı")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                Errors = new Dictionary<string, string>()
            };
        }

        public static ApiResponse<T> Failure(string message, Dictionary<string, string> errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default(T),
                Errors = errors ?? new Dictionary<string, string>()
            };
        }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public static ApiResponse Success(string message = "İşlem başarılı")
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message,
                Errors = new Dictionary<string, string>()
            };
        }

        public static ApiResponse Failure(string message, Dictionary<string, string> errors = null)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new Dictionary<string, string>()
            };
        }
    }
}