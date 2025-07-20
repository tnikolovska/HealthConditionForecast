using HealthConditionForecast.Data;
using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

/*builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();*/



//Add role services when configuring Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()  // ?? Include IdentityRole here
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();


// added new code

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Important for role seeding
    .AddEntityFrameworkStores<ApplicationDbContext>();*/




/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));*/
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

// Add services for Identity with roles
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();

/*await using var scope = app.Services.CreateAsyncScope();
var services = scope.ServiceProvider;
await SeedRolesAsync(services);
await AssignAdminRoleToUserAsync(services, "nikolovskat95@gmail.com");*/

/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    await SeedRolesAsync(services);
}
app.MapRazorPages();
app.Run();

static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// this is added
app.UseAuthentication();


app.UseAuthorization();

//app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
   //.WithStaticAssets();

// Sed roles and assign Admin role to a specific user
using (var scope1 = app.Services.CreateScope())
{
    var services1 = scope1.ServiceProvider;
    try
    {
        var context = services1.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // ? Ensure all migrations are applied

        // OPTIONAL: Seed roles and admin user
        await SeedRolesAsync(services1);
        await AssignAdminRoleToUserAsync(services1, "nikolovskat95@gmail.com");
    }
    catch (Exception ex)
    {
        var logger = services1.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();


//adding the user and admin roles to the database
static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
// Assign the Admin role to a specific user
static async Task AssignAdminRoleToUserAsync(IServiceProvider serviceProvider, string email)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var user = await userManager.FindByEmailAsync(email);

    if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

