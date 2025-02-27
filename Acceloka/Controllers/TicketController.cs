﻿using Acceloka.Entities;
using Acceloka.Models;
using Acceloka.Services;
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
        public TicketController(TicketService service)
        {
            _service = service;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get([FromQuery] GetTicketRequest request)
        {

            var datas = await _service.GetTickets(request);

            return Ok(datas);
        }

        // POST api/<TicketController>
        [HttpPost("create-ticket")]
        public async Task<IActionResult> Post([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var datas = await _service.PostTicket(request);

            return Ok(datas);
        }
    }
}
