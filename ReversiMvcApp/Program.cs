using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReversiMvcApp.Controllers;
using ReversiMvcApp.Data;

static string GetFilledConnectionString(string connectionString)
{
    connectionString = connectionString.Replace("<SQLSource>", Environment.GetEnvironmentVariable("SQLSource"));
    connectionString = connectionString.Replace("<SQLPass>", Environment.GetEnvironmentVariable("SQLPass"));

    return connectionString;
}

var builder = WebApplication.CreateBuilder(args);

var connectionStringDefault = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(GetFilledConnectionString(connectionStringDefault)));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

var dbContextUsers = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
dbContextUsers.Database.Migrate();

var connectionStringReversi = builder.Configuration.GetConnectionString("ReversiConnection");
builder.Services.AddDbContext<ReversiDbContext>(options =>
    options.UseSqlServer(GetFilledConnectionString(connectionStringReversi)));

var dbContextReversi = builder.Services.BuildServiceProvider().GetRequiredService<ReversiDbContext>();
dbContextReversi.Database.Migrate();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddTransient<SpelerController>();
builder.Services.AddTransient<ApiController>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

