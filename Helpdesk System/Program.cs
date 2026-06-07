using Helpdesk_System.Data;
using Helpdesk_System.Services;
using Helpdesk_System.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// login:password for first login as admin is admin@firma.pl:Admin.123
// login:password for first login as admin is admin@firma.pl:Admin.123
// login:password for first login as admin is admin@firma.pl:Admin.123

var builder = WebApplication.CreateBuilder(args);

// login:password for first login as admin is admin@firma.pl:Admin.123
// login:password for first login as admin is admin@firma.pl:Admin.123
// login:password for first login as admin is admin@firma.pl:Admin.123

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)/*
#if DEBUG
    .AddJsonFile("appsettings.Dev.json", optional: true, reloadOnChange: true)
#else
	.AddJsonFile("appsettings.Prod.json", optional: true, reloadOnChange: true)
#endif*/
	.AddEnvironmentVariables();

// login:password for first login as admin is admin@firma.pl:Admin.123
// login:password for first login as admin is admin@firma.pl:Admin.123
// login:password for first login as admin is admin@firma.pl:Admin.123

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Brak connection stringa o nazwie 'DefaultConnection'.");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HelpdeskSystemDbContext>(options => {
	options.UseSqlite(connectionString);

#if DEBUG
	options.EnableSensitiveDataLogging();
	options.EnableDetailedErrors();
#endif
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Accounts/Login";
        options.LogoutPath = "/Accounts/Logout";
        options.AccessDeniedPath = "/Accounts/AccessDenied";

        options.Cookie.Name = "HelpdeskSystem.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<ITicketRatingService, TicketRatingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();