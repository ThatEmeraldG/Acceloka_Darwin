﻿using Acceloka.Entities;
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
        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> Get(int bookedTicketId)
        {
            try
            {
                var result = await _service.GetBookedTicket(bookedTicketId);
                if (result == null)
                {
                    return NotFound($"Data {bookedTicketId} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<BookedTicketDetailsController>/5
        [HttpDelete("revoke-ticket/{bookedTicketId}/{ticketCode}/{qty}")]
        public async Task<IActionResult> Delete(int bookedTicketId, string ticketCode, int qty, [FromHeader(Name = "Username")] string? username)
        {
            if (qty <= 0)
            {
                return BadRequest("Invalid quantity to delete.");
            }

            try
            {
                var result = await _service.DeleteBookedTicket(bookedTicketId, ticketCode, qty, username);
                if (result == null)
                {
                    return NotFound($"Data {bookedTicketId} Not Found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: edit quantity ticket yang sudah pernah di booking
        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> Put(int bookedTicketId, [FromBody] List<BookTicketRequest> updatedTickets, [FromHeader(Name = "Username")] string? username)
        {
            try
            {
                var result = await _service.EditBookedTicket(bookedTicketId, updatedTickets, username);

                if (result == null)
                {
                    return NotFound($"Data {bookedTicketId} Not Found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
