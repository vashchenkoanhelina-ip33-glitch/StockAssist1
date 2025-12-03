using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockAssist.Web.Data;
using StockAssist.Web.Models;
using StockAssist.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Operator", policy => policy.RequireRole("Operator"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<WarehouseAllocator3D>();

builder.Services.AddHostedService<PaymentReminderService>();
builder.Services.AddHostedService<OrderStatusUpdateService>();
builder.Services.AddHostedService<StorageReminderEmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = new[] { "Admin", "Operator" };

    foreach (var r in roles)
    {
        if (!await roleMgr.RoleExistsAsync(r))
        {
            await roleMgr.CreateAsync(new IdentityRole(r));
            Console.WriteLine($"[SEED] Created role {r}");
        }
    }

    var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();

    var adminEmails = new[]
    {
        "stockassist.web@gmail.com",
        // "navrotskyimaxym@gmail.com"
    };

    foreach (var adminEmail in adminEmails)
    {
        if (string.IsNullOrWhiteSpace(adminEmail))
            continue;

        var normalizedEmail = adminEmail.Trim();
        var adminUser = await userMgr.FindByEmailAsync(normalizedEmail);
        if (adminUser == null)
        {
            adminUser = await userMgr.FindByNameAsync(normalizedEmail);
        }

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                EmailConfirmed = true
            };

            var createRes = await userMgr.CreateAsync(adminUser, "Admin123!");
            if (!createRes.Succeeded)
            {
                Console.WriteLine($"[SEED] Failed to create admin user {normalizedEmail}:");
                foreach (var e in createRes.Errors)
                    Console.WriteLine($" - {e.Code}: {e.Description}");
                continue;
            }

            Console.WriteLine($"[SEED] Created admin user {normalizedEmail}");
        }
        else
        {
            Console.WriteLine($"[SEED] Found existing user {normalizedEmail}");
        }

        if (!await userMgr.IsInRoleAsync(adminUser, "Admin"))
        {
            await userMgr.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine($"[SEED] User {normalizedEmail} -> role Admin");
        }
        else
        {
            Console.WriteLine($"[SEED] User {normalizedEmail} already in role Admin");
        }
    }
}

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

app.MapRazorPages();

app.Run();
