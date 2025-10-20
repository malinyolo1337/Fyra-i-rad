var builder = WebApplication.CreateBuilder(args);

// ?? Lägg till stöd för MVC (controllers och views)
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

// ?? Standardroute: öppna startsida eller controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Spelar}/{action=Index}/{id?}");

app.Run();

