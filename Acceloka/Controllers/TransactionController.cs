using Acceloka.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/v1/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _service;
        public TransactionController(TransactionService service)
        {
            _service = service;
        }

        // GET: api/<TransactionController>
        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var datas = await _service.GetAllTransactions();
            return Ok(datas);
        }

        // GET api/<TransactionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var data = await _service.GetTransactionById(id);
            return Ok(data);
        }

        //// PUT api/<TransactionController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{

        //}
    }
}
