using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Sadkah.API.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Errors { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Metadata { get; set; }

        public static ApiResponse<T> SuccessResponse(string message = "", T? data = default, object? metadata = null)
            => new() { Success = true, Message = message, Data = data, Metadata = metadata };

        public static ApiResponse<T> FailResponse(string message, object? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}