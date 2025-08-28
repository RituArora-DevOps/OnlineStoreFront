using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStoreFront.Data;
var builder = WebApplication.CreateBuilder(args);

// Identity DB
var connectionString = builder.Configuration.GetConnectionString("SecureOnlineStoreContextConnection") ?? throw new InvalidOperationException("Connection string 'SecureOnlineStoreContextConnection' not found.");;

builder.Services.AddDbContext<SecureOnlineStoreContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<SecureOnlineStoreContext>();

// Business DbContext (OnlineStore)
var businessConnectionString = builder.Configuration.GetConnectionString("OnlineStoreContextConnection") ?? throw new InvalidOperationException("Connection string 'OnlineStoreContextConnection' not found."); ;

//builder.Services.AddDbContext<OnlineStoreContext>(options =>
    //options.UseSqlServer(businessConnectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
