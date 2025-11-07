namespace BlazorBeats.Components.Data.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public string Bio { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public float Balance { get; set; }

        public User User { get; set; } = null!;
        

        public Profile() { }
    }

}
