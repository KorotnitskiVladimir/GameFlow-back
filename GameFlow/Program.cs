using GameFlow.Data;
using GameFlow.Middleware;
using GameFlow.Services.Date;
using GameFlow.Services.KDF;
using GameFlow.Services.Salt;
using GameFlow.Services.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IKDFService, PBKDF1Service>();

builder.Services.AddSingleton<IstorageService, FileStorageService>();

builder.Services.AddSingleton<ISaltGeneratorService, SaltGenerator>();

builder.Services.AddSingleton<IAgeCalculatorService, AgeCalculator>();

builder.Services.AddDistributedMemoryCache(); // Включаем сессию
builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(10);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    }
);

builder.Services.AddDbContext<DataContext>(
    options => options
        .UseSqlServer(builder
            .Configuration
            .GetConnectionString("LocalMs")));

builder.Services.AddScoped<DataAccessor>();

//builder.Services.AddCors(options =>
   // options.AddPolicy("CorsPolicy", policy => { policy.AllowAnyOrigin(); }));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin().AllowAnyHeader(); }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.UseSession(); // запускаем сессию
app.UseAuthSession();
app.UseAuthToken();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();