using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ?? L�gg till st�d f�r MVC (controllers och views)
builder.Services.AddControllersWithViews();

// ?? L�gg till st�d f�r sessionshantering
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // sessionen varar i 30 minuter
    options.Cookie.HttpOnly = true; // s�krare cookies
    options.Cookie.IsEssential = true; // kr�vs f�r GDPR
});

// ?? Bygg appen
var app = builder.Build();

// ?? Felhantering och HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ?? Viktigt: aktivera session innan authorization
app.UseSession();

app.UseAuthorization();

// ?? Standardroute: �ppna startsida eller controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Spelar}/{action=Index}/{id?}");

app.Run();
