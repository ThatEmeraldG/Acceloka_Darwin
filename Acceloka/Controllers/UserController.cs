using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> Get()
        {
            var datas = await _service.GetAllUsers();
            return Ok(datas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var data = await _service.GetUserById(id);
            return Ok(data);
        }

        // POST api/<UserController>
        [HttpPost("create-user")]
        public async Task<IActionResult> Post([FromBody] CreateUserRequest requestUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            var datas = await _service.PostUser(requestUser);

            return Ok(datas);
        }
    }
}
