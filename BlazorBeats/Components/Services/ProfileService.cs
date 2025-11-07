using BlazorBeats.Components.Data;
using BlazorBeats.Components.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorBeats.Services
{
    public class ProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Profile?> GetProfileByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task SaveProfileAsync(Profile profile)
        {
            var existing = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == profile.Id);

            if (existing != null)
            {
                existing.Name = profile.Name;
                existing.AvatarUrl = profile.AvatarUrl;
                _context.Profiles.Update(existing);
            }
            else
            {
                await _context.Profiles.AddAsync(profile);
            }

            await _context.SaveChangesAsync();
        }
    }
}
