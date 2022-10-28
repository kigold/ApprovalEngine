using ApprovalEngine.Models;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Models;
using SampleApp.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SampleApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudentResponse>), 200)]
        public async Task<IActionResult> Get([FromQuery] PagedRequestModel request)
        {
            return await Process(() => _studentService.GetAllStudents(request));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 200)]
        public async Task<IActionResult> Get(long id)
        {
            return await Process(() => _studentService.GetStudent(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Post([FromBody] CreateStudentRequest model)
        {
            return await Process(() => _studentService.CreateStudent(model));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Put([FromBody] UpdateStudentRequest model)
        {
            return await Process(() => _studentService.UpdateStudent(model));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Delete(long id)
        {
            return await Process(() => _studentService.DeleteStudent(id));
        }
    }
}
