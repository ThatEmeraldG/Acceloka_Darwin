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

        // GET: view details of a booked ticket
        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Get()
        {
            var datas = await _service.Get(BookedTicketId);

            return Ok(datas);
        }

        // POST: booking ticket yang quota masih tersisa
        [HttpPost("book-ticket")]
        public void Post([FromBody] string value)
        {
        }

        // PUT: edit quantity ticket yang sudah pernah di booking
        [HttpPut("edit-booked/{BookedTicketId}")]
        public void Put(int BookedTicketId, [FromBody] string value)
        {

        }
    }
}
