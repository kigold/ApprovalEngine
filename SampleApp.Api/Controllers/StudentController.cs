using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Models;
using SampleApp.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SampleApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudentResponse>), 200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _studentService.GetAllStudents());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentResponse), 200)]
        public async Task<IActionResult> Get(long id)
        {
            return Ok(await _studentService.GetStudent(id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] CreateStudentRequest model)
        {
            await _studentService.CreateStudent(model);
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Put([FromBody] UpdateStudentRequest model)
        {
            await _studentService.UpdateStudent(model);
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(long id)
        {
            await _studentService.DeleteStudent(id);
            return Ok();
        }
    }
}
