namespace BlazorBeats.Components.Data.Models
{
    public class Beat
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Title { get; set; } = "";
        public string Genre { get; set; } = "";
        public float Price { get; set; }

        public string FileUrl { get; set; } = "";
        public string? UntaggedMp3Url { get; set; }
        public string? UntaggedWavUrl { get; set; }

        public bool IsApproved { get; set; }

        public string? ImagePath { get; set; }
        public string? Bpm { get; set; }
        public string? Key { get; set; }
        public int AuthorProfileId { get; set; }
        public DateTime CreatedAtbeat { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;

        public ICollection<BeatLicense> BeatLicenses { get; set; } = new List<BeatLicense>();



        public Beat() { }

    }
}
