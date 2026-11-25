using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParcelTrack.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ParcelTrack.Middleware;
using ParcelTrack.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF Core: any controller that asks for an AppDbContext gets one,
// connected to the database named in appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ParcelTrackDb")));

// Identity: user accounts and roles, stored through AppDbContext.
// RequireConfirmedAccount = false: this app can't send email.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

string? jwt_key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwt_key))
{
    throw new Exception("Put a Jwt:Key in appsettings.json");
}
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        TokenValidationParameters rules = new TokenValidationParameters();
        rules.ValidateIssuer = true;              // made by Top5?
        rules.ValidIssuer = builder.Configuration["Jwt:Issuer"];
        rules.ValidateAudience = true;            // made for Top5's API?
        rules.ValidAudience = builder.Configuration["Jwt:Audience"];
        rules.ValidateLifetime = true;            // not expired?
        rules.ValidateIssuerSigningKey = true;    // signed with OUR key?
        rules.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key));
        options.TokenValidationParameters = rules;
    });

// A policy: "CanManageParcels" = Admin or Staff.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageParcels", policy => policy.RequireRole("Admin", "Staff"));
});

builder.Services.AddSingleton<IDeliveryFeeCalculator, DeliveryFeeCalculator>();

var app = builder.Build();

// Start-up seed: the roles and the two demo accounts. A scope, because there is no request yet.
using (IServiceScope scope = app.Services.CreateScope())
{
    await RoleSeed.Run(scope.ServiceProvider, app.Configuration);
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
app.UseMiddleware<RequestLogMiddleware>();

app.UseRouting();

// Who are you? first, then: are you allowed? The order matters.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// The Register, Log in and Log out pages come with the Identity.UI package.
app.MapRazorPages();

app.Run();
