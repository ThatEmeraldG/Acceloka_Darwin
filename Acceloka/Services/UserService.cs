using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Acceloka.Services
{
    public class UserService
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<UserService> _logger;
        public UserService(AccelokaContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET user
        public async Task<List<UserModel>> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users...");

            var datas = await _db.Users
                .Select(Q => new UserModel
                {
                    UserId = Q.UserId,
                    UserName = Q.UserName,
                    UserEmail = Q.UserEmail,
                    CreatedAt = Q.CreatedAt,
                    UpdatedAt = Q.UpdatedAt
                }).ToListAsync();

            _logger.LogInformation("Successfully fetched {Count} users", datas.Count);
            return datas;

        }

        public async Task<UserModel> GetUserById(string id)
        {
            _logger.LogInformation("Fetching user {UserId}", id);

            var data = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (data == null)
            {
                _logger.LogWarning("User {UserId} not found", id);
                return null;
            }

            var user = new UserModel
            {
                UserId = data.UserId,
                UserName = data.UserName,
                UserEmail = data.UserEmail,
                CreatedAt = data.CreatedAt,
                UpdatedAt = data.UpdatedAt
            };

            _logger.LogInformation("Successfully fetched user: {UserName} ({UserId})", data.UserName, id);
            return user;
        }

        // POST create-user
        public async Task<string> PostUser(CreateUserRequest requestUser)
        {
            _logger.LogInformation("Creating new user...");

            // Check if user with same email already exists
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.UserEmail == requestUser.UserEmail);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {UserEmail} already exists", requestUser.UserEmail);
                return "Email already exists";
            }

            var newData = new User
            {
                UserName = requestUser.UserName,
                UserEmail = requestUser.UserEmail,
                UserPassword = requestUser.UserPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(newData);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully created new user {UserName} with email {UserEmail}",
                requestUser.UserName, requestUser.UserEmail);

            return "Success";
        }
    }
}