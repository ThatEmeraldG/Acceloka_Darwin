using Acceloka.Entities;
using Acceloka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Acceloka.Services
{
    public class UserService
    {
        private readonly AccelokaContext _db;
        public UserService(AccelokaContext db)
        {
            _db = db;
        }

        // GET user
        public async Task<List<UserModel>> Get()
        {
            var datas = await _db.Users
                .Select(Q => new UserModel
                {
                    UserId = Q.UserId,
                    UserName = Q.UserName,
                    UserEmail = Q.UserEmail,
                    CreatedAt = Q.CreatedAt,
                    UpdatedAt = Q.UpdatedAt
                }).ToListAsync();

            return datas;
        }

        public async Task<UserModel> GetUserById(string id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);

            if (user == null)
            {
                return null;
            }

            return new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }


        // POST create-user
        public async Task<string> Post(UserModel request)
        {
            var newData = new User
            {
                UserId = request.UserId,
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                UserPassword = request.UserPassword,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _db.Add(newData);

            await _db.SaveChangesAsync();

            return "Success";
        }
    }
}
