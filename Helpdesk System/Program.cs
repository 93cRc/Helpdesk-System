using Helpdesk_System.Data;
using Helpdesk_System.Services;
using Helpdesk_System.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.SetBasePath(builder.Environment.ContentRootPath)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
	.AddJsonFile("appsettings.Dev.json", optional: true, reloadOnChange: true)
#else
    .AddJsonFile("appsettings.Prod.json", optional: true, reloadOnChange: true)
#endif
	.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Brak connection stringa o nazwie 'DefaultConnection'.");

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HelpdeskSystemDbContext>(options => {
	options.UseMySql(
		connectionString,
		ServerVersion.AutoDetect(connectionString));

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

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
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

app.Run();