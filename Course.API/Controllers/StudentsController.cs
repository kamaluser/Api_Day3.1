using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("")]
        public ActionResult Create(StudentCreateDto createDto)
        {
            try
            {
                return StatusCode(201, new { id = _studentService.Create(createDto) });
            }
            catch (DuplicateEntityException e)
            {
                return Conflict(e.Message);
            }
            catch (GroupLimitException e)
            {
                return Conflict(e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch
            {
                return StatusCode(500, "Error Occurred!");
            }
        }

        [HttpGet("")]
        public ActionResult<List<StudentGetDto>> GetAll()
        {
            return Ok(_studentService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<StudentGetDto> GetById(int id)
        {
            try
            {
                return Ok(_studentService.GetById(id));
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch
            {
                return StatusCode(500, "Error Occurred!");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, StudentEditDto editDto)
        {
            try
            {
                _studentService.Edit(id, editDto);
                return NoContent();
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (GroupLimitException e)
            {
                return NotFound(e.Message);
            }
            catch (DuplicateEntityException e)
            {
                return NotFound(e.Message);
            }
            catch
            {
                return StatusCode(500, "Error Occurred!");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _studentService.Delete(id);
                return NoContent();
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch
            {
                return StatusCode(500, "Error Occurred!");
            }
        }
    }
}
