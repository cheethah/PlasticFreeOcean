using System;
namespace PlasticFreeOcean.Helper
{
    public static class MessageHelper
    {
        public static ModelResponse<T> Success<T>(T data, string message = "")
        {
            return new ModelResponse<T>
            {
                IsError = false,
                Data = data,
                Message = message
            };
        }
        public static ModelResponse<object> Success(string message)
        {
            return new ModelResponse<object>
            {
                IsError = false,
                Data = null,
                Message = message
            };
        }

        public static ModelResponse<object> Error(string message)
        {
            return new ModelResponse<object>
            {
                IsError = true,
                Data = null,
                Message = message
            };
        }
    }

    public class ModelResponse<T>
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
