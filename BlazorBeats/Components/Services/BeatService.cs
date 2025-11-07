using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;
using BlazorBeats.Components.Data.Models;
using BlazorBeats.Components.Data;

public class BeatService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IWebHostEnvironment _env;

    public BeatService(IDbContextFactory<AppDbContext> contextFactory, IWebHostEnvironment env)
    {
        _contextFactory = contextFactory;
        _env = env;
    }

    public async Task<bool> UploadBeatAsync(
        Beat beatData,
        IBrowserFile taggedFile,
        IBrowserFile? coverFile,
        int userId,
        IBrowserFile? untaggedMp3 = null,
        IBrowserFile? untaggedWav = null)
    {
        if (taggedFile == null)
            return false;

        await using var context = _contextFactory.CreateDbContext();

        var profile = await context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
            return false;

        string? taggedFileUrl = null;
        string? untaggedMp3Url = null;
        string? untaggedWavUrl = null;
        string? imagePath = null;

        // mp3 tagged (используется для прослушивания)
        var taggedFileName = $"{Guid.NewGuid()}_{taggedFile.Name}";
        var taggedFolder = Path.Combine(_env.WebRootPath, "mp3tagged");
        Directory.CreateDirectory(taggedFolder);
        var taggedPath = Path.Combine(taggedFolder, taggedFileName);
        await using (var stream = File.Create(taggedPath))
        {
            await taggedFile.OpenReadStream(100_000_000).CopyToAsync(stream);
        }
        taggedFileUrl = $"/mp3tagged/{taggedFileName}";

        // mp3 untagged (не используется для прослушивания)
        if (untaggedMp3 != null)
        {
            var mp3FileName = $"{Guid.NewGuid()}_{untaggedMp3.Name}";
            var mp3Folder = Path.Combine(_env.WebRootPath, "mp3untagged");
            Directory.CreateDirectory(mp3Folder);
            var mp3Path = Path.Combine(mp3Folder, mp3FileName);
            await using var mp3Stream = File.Create(mp3Path);
            await untaggedMp3.OpenReadStream(100_000_000).CopyToAsync(mp3Stream);
            untaggedMp3Url = $"/mp3untagged/{mp3FileName}";
        }

        // wav untagged (не используется для прослушивания)
        if (untaggedWav != null)
        {
            var wavFileName = $"{Guid.NewGuid()}_{untaggedWav.Name}";
            var wavFolder = Path.Combine(_env.WebRootPath, "wavuntagged");
            Directory.CreateDirectory(wavFolder);
            var wavPath = Path.Combine(wavFolder, wavFileName);
            await using var wavStream = File.Create(wavPath);
            await untaggedWav.OpenReadStream(200_000_000).CopyToAsync(wavStream);
            untaggedWavUrl = $"/wavuntagged/{wavFileName}";
        }

        // Обложка
        if (coverFile != null)
        {
            var imageName = $"{Guid.NewGuid()}_{coverFile.Name}";
            var coversFolder = Path.Combine(_env.WebRootPath, "covers");
            Directory.CreateDirectory(coversFolder);
            var coverPath = Path.Combine(coversFolder, imageName);
            await using var coverStream = File.Create(coverPath);
            await coverFile.OpenReadStream(10_000_000).CopyToAsync(coverStream);
            imagePath = $"/covers/{imageName}";
        }

        beatData.UserId = userId;
        beatData.FileUrl = taggedFileUrl!;
        beatData.UntaggedMp3Url = untaggedMp3Url;
        beatData.UntaggedWavUrl = untaggedWavUrl;
        beatData.ImagePath = imagePath;
        beatData.AuthorProfileId = profile.Id;

        await context.Beats.AddAsync(beatData);
        await context.SaveChangesAsync(); // сохраняем, чтобы получить beat.Id

        // Привязка лицензий
        var userLicenses = await context.Licenses
            .Where(l => l.UserId == userId && (l.Type == "MP3" || l.Type == "WAV"))
            .ToListAsync();

        foreach (var license in userLicenses)
        {
            Console.WriteLine($"Привязываю лицензию {license.Id} к биту {beatData.Id}");

            context.Add(new BeatLicense
            {
                BeatId = beatData.Id,
                LicenseId = license.Id
            });
        }

        await context.SaveChangesAsync(); // сохраняем связи
        return true;
    }

    public async Task<List<Beat>> GetBeatsByUserIdAsync(int userId)
    {
        await using var context = _contextFactory.CreateDbContext();

        return await context.Beats
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAtbeat)
            .ToListAsync();
    }

    public async Task<List<Beat>> GetAllBeatsAsync()
    {
        await using var context = _contextFactory.CreateDbContext();

        return await context.Beats
            .Where(b => b.IsApproved)
            .Include(b => b.BeatLicenses)
                .ThenInclude(bl => bl.License)
            .ToListAsync();
    }

    public async Task<List<Beat>> GetBeatsByApprovalAsync(bool isApproved)
    {
        await using var context = _contextFactory.CreateDbContext();

        return await context.Beats
            .Where(b => b.IsApproved == isApproved)
            .OrderByDescending(b => b.CreatedAtbeat)
            .ToListAsync();
    }

    public async Task<bool> UpdateBeatAsync(Beat beat)
    {
        await using var context = _contextFactory.CreateDbContext();

        var existing = await context.Beats.FindAsync(beat.Id);
        if (existing == null)
            return false;

        existing.Title = beat.Title;
        existing.Genre = beat.Genre;
        existing.Price = beat.Price;
        existing.Bpm = beat.Bpm;
        existing.Key = beat.Key;
        existing.IsApproved = beat.IsApproved;  // добавь эту строку

        context.Beats.Update(existing);
        await context.SaveChangesAsync();

        return true;
    }




    public async Task<bool> DeleteBeatAsync(int beatId)
    {
        await using var context = _contextFactory.CreateDbContext();

        var beat = await context.Beats.FindAsync(beatId);
        if (beat == null)
            return false;

        context.Beats.Remove(beat);
        await context.SaveChangesAsync();

        return true;
    }
}
