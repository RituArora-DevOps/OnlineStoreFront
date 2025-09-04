using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStoreFront.Data;
using OnlineStoreFront.Models.Business;
using OnlineStoreFront.Services;
using System.IO;
using ECommerceSecureApp.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Identity DB
var connectionString = builder.Configuration.GetConnectionString("SecureOnlineStoreContextConnection") ?? throw new InvalidOperationException("Connection string 'SecureOnlineStoreContextConnection' not found.");;

// Itdentity + Auth
builder.Services.AddDbContext<SecureOnlineStoreContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => { 
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()    // To enable roles
.AddEntityFrameworkStores<SecureOnlineStoreContext>();

// Business DbContext (OnlineStore)
var businessConnectionString = builder.Configuration.GetConnectionString("OnlineStoreContextConnection") ?? throw new InvalidOperationException("Connection string 'OnlineStoreContextConnection' not found."); ;

builder.Services.AddDbContext<OnlineStoreContext>(options =>
    options.UseSqlServer(businessConnectionString));

// Register services
builder.Services.AddScoped<ReviewService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Shared key ring + cookie so both apps recognize the same login
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\KeyRing")) // same path in BOTH apps
    .SetApplicationName("OnlineStoreSuite");                    // same name in BOTH apps

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = ".OnlineStore.Auth";        // same name in BOTH apps
    o.LoginPath = "/Identity/Account/Login";    // Auth UI lives here
    o.SlidingExpiration = true;
});

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

var app = builder.Build();

// Role seeding block
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAndAdminAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await app.RunAsync();
