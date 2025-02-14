using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/revoke-ticket")]
    [ApiController]
    public class BookedTicketDetailsController : ControllerBase
    {
        // DELETE api/<BookedTicketDetailsController>/5
        [HttpDelete("{BookedTicketId}/{TicketCode}/{Qty}")]
        public async Task<IActionResult> Delete(int BookedTicketId, String TicketCode, int Qty)
        {


            return Ok();
        }
    }
}
