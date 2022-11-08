using ApprovalEngine.Models;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Models;

namespace SampleApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public IActionResult ApiResponse<T>(T data = default, string message = "",
            ApiResponseCodes codes = ApiResponseCodes.OK, int? totalCount = 0, params string[] errors)
        {
            var response = new ApiResponse<T>(data, message, codes, totalCount, errors);
            response.Description = message ?? response.Code.ToString();
            return Ok(response);
        }

        public async Task<IActionResult> Process<T>(Func<Task<ResultModel<T>>> request)
        {
            var result = await request.Invoke();

            if (result.HasError)
                return ApiResponse<string>(errors: result.ErrorMessages.ToArray());

            if (result.HasError)
                return ApiResponse<string>(errors: result.ErrorMessages.ToArray());

            Type t = result?.Data?.GetType();
            var countProperty = t.GetProperty("Count");
            int count = (int?)countProperty?.GetValue(result.Data) ?? 0;

            return ApiResponse(message: string.IsNullOrEmpty(result.Message) ? "Success" : result.Message, codes: ApiResponseCodes.OK, data: result.Data, totalCount: count);
        }

        public async Task<IActionResult> Process<T>(Func<Task<ResultModel<PagedList<T>>>> request)
        {
            var result = await request.Invoke();

            if (result.HasError)
                return ApiResponse<string>(errors: result.ErrorMessages.ToArray());

            if (result.HasError)
                return ApiResponse<string>(errors: result.ErrorMessages.ToArray());


            return ApiResponse(message: string.IsNullOrEmpty(result.Message) ? "Success" : result.Message, codes: ApiResponseCodes.OK, data: result.Data.Items, totalCount: result.Data.TotalCount);
        }
    }
}
