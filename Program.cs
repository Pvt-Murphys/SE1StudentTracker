using SE1StudentTracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;
using SE1StudentTracker.Data;
using Microsoft.CodeAnalysis.Scripting.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()         
.AddEntityFrameworkStores<AppDbContext>();



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
<<<<<<< HEAD
builder.Services.AddSingleton(new OracleService("User Id=STUDENT_TRACKER;Password=Strong#Password1;Data Source=localhost:1521/XE;"));



=======
builder.Services.AddHttpContextAccessor();
>>>>>>> origin/garrett-student-tracker

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeed.SeedRoles(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var adminEmail = "admin@test.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
        await userManager.AddToRoleAsync(adminUser, "User");
        await userManager.AddToRoleAsync(adminUser, "Teacher");
        await userManager.AddToRoleAsync(adminUser, "Student");
    }

    

}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
