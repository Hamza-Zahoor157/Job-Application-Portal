using JobPortal.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JobPortal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApplicantOnly", policy => policy.RequireClaim("UserType", "Applicant"));
    options.AddPolicy("HRorAdminOnly", policy => policy.RequireClaim("UserType", "HR", "Admin" ));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("UserType", "Admin"));
    options.AddPolicy("HROnly", policy => policy.RequireClaim("UserType", "HR"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Commented out for development
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.MapGet("/", context => {
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

// Initialize database with roles and users
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.Initialize(scope.ServiceProvider);
}

app.Run();
