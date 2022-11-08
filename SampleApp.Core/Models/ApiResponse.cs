using System.ComponentModel;

namespace SampleApp.Core.Models
{
    public class ApiResponse
    {
        public ApiResponseCodes Code { get; set; }
        public string Description { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors => Errors.Any();
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Payload { get; set; } = default;
        public int TotalCount { get; set; }
        public string ResponseCode { get; set; }

        public ApiResponse(T data = default, string message = "",
            ApiResponseCodes codes = ApiResponseCodes.OK, int? totalCount = 0, params string[] errors)
        {
            Payload = data;
            Errors = errors.ToList();
            Code = !errors.Any() ? codes : codes == ApiResponseCodes.OK ? ApiResponseCodes.ERROR : codes;
            Description = message;
            TotalCount = totalCount ?? 0;
        }
    }

    public enum ApiResponseCodes
    {
        EXCEPTION = -5,

        [Description("Unauthorized Access")]
        UNAUTHORIZED = -4,

        [Description("Not Found")]
        NOT_FOUND = -3,

        [Description("Invalid Request")]
        INVALID_REQUEST = -2,

        [Description("Server error occured, please try again.")]
        ERROR = -1,

        [Description("FAILED")]
        FAILED = 2,

        [Description("SUCCESS")]
        OK = 1
    }
}
