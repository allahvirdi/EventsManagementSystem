using EventsManagement.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventsManagement.Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Seed Roles
            await SeedRolesAsync();

            // Seed Admin User
            await SeedAdminUserAsync();

            // Seed Dynamic Tables
            await SeedDynamicTablesAsync();

            // Seed Master Data (Province, Region, School samples)
            await SeedMasterDataAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = { "Admin", "Manager", "Operator", "Viewer" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation($"Role '{role}' created");
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = "admin@eventsystem.ir";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "مدیر سیستم",
                IsActive = true,
                PhoneNumber = "09120000000",
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, "!QAZ1qaz");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                _logger.LogInformation("Admin user created successfully");
            }
            else
            {
                _logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private async Task SeedDynamicTablesAsync()
    {
        if (!await _context.DynamicTables.AnyAsync())
        {
            var dynamicTables = new List<DynamicTable>
            {
                // وضعیت رویداد
                new() { TableName = "EventStatus", Value = "ثبت شده", Description = "رویداد ثبت شده است", IsActive = true, DisplayOrder = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Value = "در حال بررسی", Description = "رویداد در حال بررسی است", IsActive = true, DisplayOrder = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Value = "تایید شده", Description = "رویداد تایید شده است", IsActive = true, DisplayOrder = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Value = "رد شده", Description = "رویداد رد شده است", IsActive = true, DisplayOrder = 4, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Value = "بسته شده", Description = "رویداد بسته شده است", IsActive = true, DisplayOrder = 5, CreatedBy = "System", CreatedAt = DateTime.Now },

                // اهمیت رویداد
                new() { TableName = "EventSeverity", Value = "کم", Description = "اهمیت کم", IsActive = true, DisplayOrder = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventSeverity", Value = "متوسط", Description = "اهمیت متوسط", IsActive = true, DisplayOrder = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventSeverity", Value = "زیاد", Description = "اهمیت زیاد", IsActive = true, DisplayOrder = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "EventSeverity", Value = "بحرانی", Description = "اهمیت بحرانی", IsActive = true, DisplayOrder = 4, CreatedBy = "System", CreatedAt = DateTime.Now },

                // دامنه تاثیر
                new() { TableName = "ImpactScope", Value = "مدرسه", Description = "تاثیر در سطح مدرسه", IsActive = true, DisplayOrder = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScope", Value = "ناحیه", Description = "تاثیر در سطح ناحیه", IsActive = true, DisplayOrder = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScope", Value = "استان", Description = "تاثیر در سطح استان", IsActive = true, DisplayOrder = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScope", Value = "کشور", Description = "تاثیر در سطح کشور", IsActive = true, DisplayOrder = 4, CreatedBy = "System", CreatedAt = DateTime.Now },

                // وضعیت وظیفه
                new() { TableName = "TaskStatus", Value = "معلق", Description = "وظیفه معلق است", IsActive = true, DisplayOrder = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "TaskStatus", Value = "در حال انجام", Description = "وظیفه در حال انجام است", IsActive = true, DisplayOrder = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "TaskStatus", Value = "تکمیل شده", Description = "وظیفه تکمیل شده است", IsActive = true, DisplayOrder = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "TaskStatus", Value = "مسدود شده", Description = "وظیفه مسدود شده است", IsActive = true, DisplayOrder = 4, CreatedBy = "System", CreatedAt = DateTime.Now },

                // اولویت وظیفه
                new() { TableName = "TaskPriority", Value = "کم", Description = "اولویت کم", IsActive = true, DisplayOrder = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "TaskPriority", Value = "متوسط", Description = "اولویت متوسط", IsActive = true, DisplayOrder = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "TaskPriority", Value = "زیاد", Description = "اولویت زیاد", IsActive = true, DisplayOrder = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "TaskPriority", Value = "فوری", Description = "اولویت فوری", IsActive = true, DisplayOrder = 4, CreatedBy = "System", CreatedAt = DateTime.Now },

                // نوع واحد سازمانی
                new() { TableName = "OrganizationUnitType", Value = "وزارتخانه", Description = "سطح وزارتخانه", IsActive = true, DisplayOrder = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "OrganizationUnitType", Value = "استان", Description = "سطح استان", IsActive = true, DisplayOrder = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "OrganizationUnitType", Value = "ناحیه", Description = "سطح ناحیه", IsActive = true, DisplayOrder = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { TableName = "OrganizationUnitType", Value = "مدرسه", Description = "سطح مدرسه", IsActive = true, DisplayOrder = 4, CreatedBy = "System", CreatedAt = DateTime.Now },
            };

            await _context.DynamicTables.AddRangeAsync(dynamicTables);
            _logger.LogInformation($"Seeded {dynamicTables.Count} dynamic table entries");
        }
    }

    private async Task SeedMasterDataAsync()
    {
        // Seed Provinces
        if (!await _context.Provinces.AnyAsync())
        {
            var provinces = new List<Province>
            {
                new() { Name = "تهران", Code = 1, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { Name = "اصفهان", Code = 2, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { Name = "فارس", Code = 3, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { Name = "خراسان رضوی", Code = 4, CreatedBy = "System", CreatedAt = DateTime.Now },
            };

            await _context.Provinces.AddRangeAsync(provinces);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {provinces.Count} provinces");

            // Seed Regions for Tehran Province
            var tehranProvince = await _context.Provinces.FirstAsync(p => p.Name == "تهران");
            var regions = new List<Region>
            {
                new() { Name = "ناحیه 1 تهران", Code = 101, ProvinceId = tehranProvince.Id, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { Name = "ناحیه 2 تهران", Code = 102, ProvinceId = tehranProvince.Id, CreatedBy = "System", CreatedAt = DateTime.Now },
                new() { Name = "ناحیه 3 تهران", Code = 103, ProvinceId = tehranProvince.Id, CreatedBy = "System", CreatedAt = DateTime.Now },
            };

            await _context.Regions.AddRangeAsync(regions);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {regions.Count} regions");

            // Seed Schools for Region 1
            var region1 = await _context.Regions.FirstAsync(r => r.Name == "ناحیه 1 تهران");
            var schools = new List<School>
            {
                new() 
                { 
                    Name = "دبستان شهید باهنر", 
                    Code = 10101, 
                    RegionId = region1.Id, 
                    ProvinceId = tehranProvince.Id,
                    Address = "تهران، خیابان ولیعصر، کوچه باهنر",
                    PhoneNumber = "02188776655",
                    CreatedBy = "System", 
                    CreatedAt = DateTime.Now 
                },
                new() 
                { 
                    Name = "دبیرستان شهید بهشتی", 
                    Code = 10102, 
                    RegionId = region1.Id, 
                    ProvinceId = tehranProvince.Id,
                    Address = "تهران، خیابان انقلاب، کوچه بهشتی",
                    PhoneNumber = "02188776656",
                    CreatedBy = "System", 
                    CreatedAt = DateTime.Now 
                },
            };

            await _context.Schools.AddRangeAsync(schools);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {schools.Count} schools");
        }
    }
}
