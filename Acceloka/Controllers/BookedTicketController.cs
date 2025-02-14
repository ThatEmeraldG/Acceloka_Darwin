using Acceloka.Models;
using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _service;
        public BookedTicketController(BookedTicketService service)
        {
            _service = service;
        }

        // POST: booking ticket yang quota masih tersisa
        [HttpPost("book-ticket")]
        public async Task<IActionResult> Post([FromBody] List<BookedTicketRequest> request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var datas = await _service.Post(request);
                return Ok(datas);
            }
            catch (Exception e)
            {
                return BadRequest($"{e.Message}");
            }
        }
    }
}
