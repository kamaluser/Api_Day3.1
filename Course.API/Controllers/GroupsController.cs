using Course.Service.Dtos.GroupDtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("")]
        public ActionResult Create(GroupCreateDto createDto)
        {
            try
            {
                return StatusCode(201, new { id = _groupService.Create(createDto) });
            }
            catch (DuplicateEntityException e)
            {
                return Conflict();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Error Occured!");
            }
        }

        [HttpGet("")]
        public ActionResult<List<GroupGetDto>> GetAll()
        {
            return Ok(_groupService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<GroupGetDto> GetById(int id)
        {
            try
            {
                return Ok(_groupService.GetById(id));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch
            {
                return StatusCode(500, "Error Occurred!");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, GroupEditDto editDto)
        {
            try
            {
                _groupService.Edit(id, editDto);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
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
                _groupService.Delete(id);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch
            {
                return StatusCode(500, "Error Occurred!");
            }
        }
    }
}
