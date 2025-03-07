using Acceloka.Application.Commands.Bookings;
using Acceloka.Models;
using Acceloka.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookTicketService _service;
        private readonly IMediator _mediator;
        public BookingController(BookTicketService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
        }

        // POST: booking ticket yang quota masih tersisa
        [HttpPost("book-ticket")]
        public async Task<IActionResult> Post(
            [FromBody] List<BookTicketRequest> request,
            [FromHeader (Name = "Username")] string? username)
        {
            if (request == null || !request.Any())
            {
                return BadRequest("No tickets specified for booking");
            };

            var ticketItems = request
                .Select(r => new BookTicketItem(r.TicketCode, r.Quantity))
                .ToList();

            var command = new BookTicketsCommand(ticketItems, username ?? "System");

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);

            //try
            //{
            //    var result = await _service.BookTickets(request, username);
            //    return Ok(result);
            //}
            //catch (Exception e)
            //{
            //    return BadRequest(e.Message);
            //}
        }
    }
}
