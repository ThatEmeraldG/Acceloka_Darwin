using Acceloka.Models;
using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookTicketService _service;
        public BookingController(BookTicketService service)
        {
            _service = service;
        }

        // POST: booking ticket yang quota masih tersisa
        [HttpPost("book-ticket")]
        public async Task<IActionResult> Post([FromBody] List<BookTicketRequest> request, [FromHeader (Name = "Username")] string? username)
        {
            if (request == null || !request.Any())
            {
                return BadRequest("No tickets specified for booking");
            };

            try
            {
                var result = await _service.BookTickets(request, username);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
