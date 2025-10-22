//var builder = WebApplication.CreateBuilder(args);

//// ?? Lägg till stöd för MVC (controllers och views)
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// ?? Felhantering och HTTPS
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//// ?? Standardroute: öppna startsida eller controller
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Spelar}/{action=Index}/{id?}");

//app.Run();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSession();

    app.UseRouting();

    app.UseAuthorization();
    app.UseCookiePolicy();



    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Spelar}/{action=Index}/{id?}");


    app.Run();



