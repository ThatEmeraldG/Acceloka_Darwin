using Acceloka.Application.Commands.Users;
using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Services;
using Acceloka.Shared;
using MediatR;
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
        private readonly IMediator _mediator;
        public UserController(UserService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
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
                return BadRequest("Invalid request.");
            }

            var command = new CreateUserCommand(
                requestUser.UserId,
                requestUser.UserName,
                requestUser.UserEmail,
                requestUser.UserPassword
            );

            //var datas = await _service.PostUser(requestUser);

            Result<string> result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result);
        }
    }
}
