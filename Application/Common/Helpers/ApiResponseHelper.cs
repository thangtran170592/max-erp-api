using Application.Common.Models;
using Application.Dtos;
using AutoMapper;

namespace Application.Common.Helpers
{
    public static class ApiResponseHelper
    {
        public static ApiResponseDto<T> CreateSuccessResponse<T>(T data, string message = "Request successful")
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponseDto<T> CreateFailureResponse<T>(Exception? ex = null, List<ApiErrorDto>? errors = null, string message = "Request failed")
        {
            var errorList = errors != null ? [.. errors] : new List<ApiErrorDto>();
            if (ex != null)
            {
                errorList.AddRange([..ex.Message.Split('\n').Select((message, index) => new ApiErrorDto
                {
                    Field = index.ToString(),
                    Message = message
                })]);
            }
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errorList,
                Timestamp = DateTime.UtcNow,
                TraceId = ex?.StackTrace,
            };
        }

        public static ApiResponseDto<List<K>> CreateSuccessResponse<T, K>(ApiResponseDto<List<T>> response, IMapper mapper, string message = "Request successful")
        {
            if (response.Data == null || !response.Data.Any())
            {
                return new ApiResponseDto<List<K>>
                {
                    Success = true,
                    Message = message,
                    Data = [],
                    PageData = response.PageData,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                };
            }
            var result = response.Data.Select(item => mapper.Map<K>(item)).ToList();
            return new ApiResponseDto<List<K>>
            {
                Success = true,
                Message = message,
                Data = result,
                PageData = response.PageData,
                Errors = null,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}