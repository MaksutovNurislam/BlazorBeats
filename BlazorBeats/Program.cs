using BlazorBeats.Components;
using BlazorBeats.Components.Data;
using BlazorBeats.Services;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<NpgsqlConnection>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
    return new NpgsqlConnection(connectionString);
});

// Регистрируем только DbContextFactory, не AddDbContext!
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024; // до 20 MB
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<BeatService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddControllers();

builder.Services.Configure<CircuitOptions>(options => { options.DetailedErrors = true; });

var app = builder.Build();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseHsts();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.MapControllers();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
