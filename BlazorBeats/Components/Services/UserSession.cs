using BlazorBeats.Components.Data.Models;

namespace BlazorBeats.Services
{
    public class UserSession
    {
        public User? CurrentUser { get; set; }
        public bool IsAuthenticated => CurrentUser != null;

        public void Logout() => CurrentUser = null;
    }
}
