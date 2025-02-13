using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Acceloka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        [HttpGet]
        public async Task<List<User>> Get()
        {
            return await _dbContext.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] User userAccount)
        {
            if(string.IsNullOrWhiteSpace(userAccount.UserName) ||
                string.IsNullOrWhiteSpace(userAccount.UserEmail) ||
                string.IsNullOrWhiteSpace(userAccount.UserPassword))
            {
                return BadRequest("Invalid Request");
            }

            //userAccount.UserPassword = PasswordHashHandler.HashPassword(userAccount.UserPassword);
            //await _dbContext.Users.AddAsync(userAccount);
            //await _dbContext.SaveChangesAsync();

            //return CreatedAtAction(nameof(GetById), userAccount);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
