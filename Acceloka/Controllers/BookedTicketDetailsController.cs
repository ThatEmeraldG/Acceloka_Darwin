using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class BookedTicketDetailsController : ControllerBase
    {
        private readonly BookedTicketDetailsService _service;
        public BookedTicketDetailsController(BookedTicketDetailsService service)
        {
            _service = service;
        }

        // GET: view details of a booked ticket
        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Get(int BookedTicketId)
        {
            var datas = await _service.Get(BookedTicketId);

            return Ok(datas);
        }

        // DELETE api/<BookedTicketDetailsController>/5
        [HttpDelete("revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}")]
        public async Task<IActionResult> Delete(int bookedTicketId, string ticketCode, int qty)
        {
            var result = await _service.Delete(bookedTicketId, ticketCode, qty);
            if (result == null)
            {
                return BadRequest("Invalid request.");
            }

            return Ok(result);
        }

        // PUT: edit quantity ticket yang sudah pernah di booking
        [HttpPut("edit-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> Put(int bookedTicketId, [FromBody] List<BookedTicketRequest> updatedTickets)
        {
            var result = await _service.Put(bookedTicketId, updatedTickets);
            if (result == null)
            {
                return BadRequest("Invalid request.");
            }

            return Ok(result);
        }
    }
}
