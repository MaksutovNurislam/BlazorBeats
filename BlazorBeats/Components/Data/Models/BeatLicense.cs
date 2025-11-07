using BlazorBeats.Components.Data.Models;

public class BeatLicense
{
    public int BeatId { get; set; }
    public int LicenseId { get; set; }

    public Beat Beat { get; set; } = null!;
    public License License { get; set; } = null!;
}