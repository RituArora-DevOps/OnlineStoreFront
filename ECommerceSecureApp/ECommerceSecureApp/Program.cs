using ECommerceSecureApp.Data;
using ECommerceSecureApp.Models;
using ECommerceSecureApp.Repository;
using ECommerceSecureApp.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using System.IO;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var identityConn = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConn));

builder.Services.AddDbContext<OnlineStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineStoreConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    //2FA uses autheticator token provider:
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services for Razor Pages and MVC controllers with views
builder.Services.AddControllersWithViews();

// Register repositories in DI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// This tells ASP.NET - whenever a controller asks for IProductRepository, give them an instance of ProductRepository.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Enable Session for guest carts
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.Cookie.Name = ".OnlineStore.Session";
    o.IdleTimeout = TimeSpan.FromHours(12);
    o.Cookie.HttpOnly = true;
});

// Share the same auth keys/cookie with the Auth app
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\KeyRing")) // same path as the Auth app
    .SetApplicationName("OnlineStoreSuite");                    // same name as the Auth app

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = ".OnlineStore.Auth";        // same cookie name as the Auth app
    o.LoginPath = "/Identity/Account/Login";    // or the full URL of the Auth app¡¯s login
    o.SlidingExpiration = true;
});


var app = builder.Build();

// for seeding the database with test data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OnlineStoreDbContext>();

    // Optional: apply pending migrations
    context.Database.Migrate();

    Console.WriteLine("Seeding started...");

    // Seed your data
    DbSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseSession(); // needed later when we enable cart/session 

app.MapStaticAssets();

// Default route configuration for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
