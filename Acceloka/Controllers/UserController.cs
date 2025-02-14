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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var datas = await _service.Get();
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
        public async Task<IActionResult> Post([FromBody] UserModel requestUser)
        {
            //if (string.IsNullOrWhiteSpace(requestUser.UserName) ||
            //    string.IsNullOrWhiteSpace(requestUser.UserEmail) ||
            //    string.IsNullOrWhiteSpace(requestUser.UserPassword))
            //{
            //    return BadRequest("Invalid Request");
            //}

            //userAccount.UserPassword = PasswordHashHandler.HashPassword(userAccount.UserPassword);
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            var datas = await _service.Post(requestUser);

            return Ok(datas);
        }
    }
}
