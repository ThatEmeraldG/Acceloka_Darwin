using Acceloka.Application.Commands.Tickets;
using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/ticket")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _service;
        private readonly IMediator _mediator;
        public TicketController(TicketService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
        }

        // GET
        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get([FromQuery] GetTicketRequest request)
        {

            var datas = await _service.GetTickets(request);

            return Ok(datas);
        }

        // POST api/<TicketController>
        [HttpPost("create-ticket")]
        public async Task<IActionResult> Post(
            [FromBody] CreateTicketRequest request,
            [FromHeader (Name = "Username")] string? username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var command = new CreateTicketCommand(
                Username: username ?? "System",
                TicketCode: request.TicketCode,
                TicketName: request.TicketName,
                CategoryName: request.CategoryName,
                EventStart: request.EventStart,
                EventEnd: request.EventEnd,
                Price: request.Price,
                Quota: request.Quota
            );

            //var datas = await _service.PostTicket(request, username);
            var ticketId = await _mediator.Send(command);

            return Ok(ticketId);
        }
    }
}
