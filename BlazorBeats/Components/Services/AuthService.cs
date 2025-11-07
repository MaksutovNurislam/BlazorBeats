using BlazorBeats.Components.Data;
using BlazorBeats.Components.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorBeats.Services
{
    public class AuthService
    {
        private readonly AppDbContext _dbContext;

        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<User?> LoginAsync(string email, string password)
        {
            var hashed = HashPassword(password);
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == hashed);
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == email))
                return false;

            var user = new User
            {
                Email = email,
                Password = HashPassword(password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new Profile
            {
                UserId = user.Id,
                Name = "Новый пользователь",
                Balance = 0
            };

            _dbContext.Profiles.Add(profile);

            // ✅ Добавляем лицензии без привязки к битам
            var licenses = new List<License>
    {
        new License
        {
            UserId = user.Id,
            Type = "MP3",
            Price = 19.99f,
            Description = "Лицензия MP3 для использования без тегов",
            FileUrl = ""
        },
        new License
        {
            UserId = user.Id,
            Type = "WAV",
            Price = 29.99f,
            Description = "Лицензия WAV высокого качества без тегов",
            FileUrl = ""
        }
    };

            _dbContext.Licenses.AddRange(licenses);

            await _dbContext.SaveChangesAsync();
            return true;
        }



        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}