using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ?? Lägg till stöd för MVC (controllers och views)
builder.Services.AddControllersWithViews();

// ?? Lägg till stöd för sessionshantering
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // sessionen varar i 30 minuter
    options.Cookie.HttpOnly = true; // säkrare cookies
    options.Cookie.IsEssential = true; // krävs för GDPR
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

// ?? Standardroute: öppna startsida eller controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Spelar}/{action=Index}/{id?}");

app.Run();
