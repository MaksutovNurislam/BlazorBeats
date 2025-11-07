using BlazorBeats.Components.Data;
using BlazorBeats.Models;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> PurchaseBeatAsync(int buyerId, int beatId, int licenseId)
    {
        var beat = await _context.Beats.FindAsync(beatId);
        var license = await _context.Licenses.FindAsync(licenseId);
        var buyerProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == buyerId);
        var sellerProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == beat.UserId);

        if (beat == null || license == null || buyerProfile == null || sellerProfile == null)
            return false;

        if (buyerProfile.Balance < license.Price)
            return false;

        // Транзакция
        buyerProfile.Balance -= license.Price;
        sellerProfile.Balance += license.Price;

        var order = new Order
        {
            BuyerId = buyerId,
            SellerId = beat.UserId,
            BeatId = beatId,
            LicenseId = licenseId,
            Price = license.Price
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Order>> GetOrdersByBuyerIdAsync(int buyerId)
    {
        return await _context.Orders // Изменено с _db на _context
            .Include(o => o.Beat)
            .ThenInclude(b => b.User)
            .Include(o => o.License)
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync();
    }
    // Продажи текущего пользователя
    public async Task<List<Order>> GetOrdersBySellerIdAsync(int sellerId)
    {
        return await _context.Orders // Изменено с _db на _context
            .Include(o => o.Beat)
            .ThenInclude(b => b.User)
            .Include(o => o.License)
            .Include(o => o.Buyer)
            .Where(o => o.Beat.UserId == sellerId)
            .ToListAsync();
    }
}
