using System.ComponentModel.DataAnnotations.Schema;
using BlazorBeats.Components.Data.Models;

namespace BlazorBeats.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public int BeatId { get; set; }
        public int LicenseId { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Явное указание внешних ключей
        [ForeignKey("BuyerId")]
        public User Buyer { get; set; } = null!;

        [ForeignKey("SellerId")]
        public User Seller { get; set; } = null!;

        [ForeignKey("BeatId")]
        public Beat Beat { get; set; } = null!;

        [ForeignKey("LicenseId")]
        public License License { get; set; } = null!;
    }
}
