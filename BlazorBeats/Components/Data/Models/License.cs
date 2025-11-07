using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorBeats.Components.Data.Models
{
    public class License
    {
        public int Id { get; set; }

        public int UserId { get; set; } // связь с владельцем лицензии
        public string Type { get; set; } = null!; // "Mp3", "Wav"
        public float Price { get; set; }
        public string Description { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public License() { }


        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public ICollection<BeatLicense> BeatLicenses { get; set; } = new List<BeatLicense>();


    }
}
