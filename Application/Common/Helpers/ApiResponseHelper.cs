using Application.Common.Models;
using Application.Dtos;
using AutoMapper;

namespace Application.Common.Helpers
{
    public static class ApiResponseHelper
    {
        public static ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Request successful", MetaData? meta = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Meta = meta,
                Errors = null,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> CreateFailureResponse<T>(Exception? ex = null, List<ApiError>? errors = null, string message = "Request failed")
        {
            var errorList = errors != null ? [.. errors] : new List<ApiError>();
            if (ex != null)
            {
                errorList.AddRange([..ex.Message.Split('\n').Select((message, index) => new ApiError
                {
                    Field = index.ToString(),
                    Message = message
                })]);
            }
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errorList,
                Timestamp = DateTime.UtcNow,
                TraceId = ex?.StackTrace,
            };
        }

        public static ApiResponse<List<K>> CreateSuccessResponse<T, K>(PagedResult<T> pagedResult, IMapper mapper, string message = "Request successful")
        {
            var meta = mapper.Map<PagedResult<T>, MetaData>(pagedResult);
            if (pagedResult.Data is null || !pagedResult.Data.Any())
            {
                return new ApiResponse<List<K>>
                {
                    Success = true,
                    Message = message,
                    Data = [],
                    Meta = meta,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                };
            }
            var data = pagedResult.Data.Select(item => mapper.Map<K>(item)).ToList();
            return new ApiResponse<List<K>>
            {
                Success = true,
                Message = message,
                Data = data,
                Meta = meta,
                Errors = null,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}