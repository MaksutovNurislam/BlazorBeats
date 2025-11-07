using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorBeats.Components.Data;
using BlazorBeats.Components.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorBeats.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BeatSalesDto>> GetTopBeatsThisMonthAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            return await _context.Orders
                .Where(o => o.CreatedAt >= startOfMonth)
                .GroupBy(o => o.Beat.Title)
                .Select(g => new BeatSalesDto
                {
                    Title = g.Key,
                    SalesCount = g.Count()
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(10)
                .ToListAsync();
        }
        public async Task<double> GetPlatformIncomeAsync(DateTime startDate, DateTime endDate)
        {
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            return await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .SumAsync(o => o.Price * 0.15);
        }



        public async Task<List<MusicianPayoutDto>> GetMusicianPayoutsAsync(DateTime startDate, DateTime endDate, int? sellerId = null)
        {
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            var query = _context.Orders
                .Include(o => o.Seller)
                .ThenInclude(s => s.Profile)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate);

            if (sellerId.HasValue)
            {
                query = query.Where(o => o.SellerId == sellerId.Value);
            }

            return await query
                .GroupBy(o => new { o.Seller.Email, DisplayName = o.Seller.Profile.Name, AvatarUrl = o.Seller.Profile.AvatarUrl })
                .Select(g => new MusicianPayoutDto
                {
                    Email = g.Key.Email,
                    DisplayName = g.Key.DisplayName,
                    AvatarUrl = g.Key.AvatarUrl,
                    AmountPaid = g.Sum(x => x.Price * 0.85)
                })
                .OrderByDescending(x => x.AmountPaid)
                .ToListAsync();
        }



        public async Task<List<UserDto>> GetSellersAsync()
        {
            return await _context.Users
                .Where(u => u.OrdersAsSeller.Any() && u.Profile != null)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    DisplayName = u.Profile.Name
                })
                .ToListAsync();
        }

    }

    public class BeatSalesDto
    {
        public string Title { get; set; }
        public int SalesCount { get; set; }
    }

    public class MusicianPayoutDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public double AmountPaid { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = "";
    }

}
