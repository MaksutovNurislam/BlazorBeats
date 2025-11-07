using BlazorBeats.Models;

namespace BlazorBeats.Components.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "User";

        public Profile Profile { get; set; } = null!;

        // Навигационные свойства для заказов
        
        public ICollection<Order> OrdersAsBuyer { get; set; } = new List<Order>();
        public ICollection<Order> OrdersAsSeller { get; set; } = new List<Order>();

        public List<Beat> Beats { get; set; } = new();
    }
}
