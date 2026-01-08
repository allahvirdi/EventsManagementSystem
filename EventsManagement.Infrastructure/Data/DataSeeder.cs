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

            // Seed Organization Units
            await SeedOrganizationUnitsAsync();

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
                // EventSource - منابع رویداد
                new() { TableName = "EventSource", Code = 1, Value = "داخلی", Description = "رویداد داخلی سازمان", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSource", Code = 2, Value = "خارجی", Description = "رویداد خارج از سازمان", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSource", Code = 3, Value = "آموزش و پرورش", Description = "رویداد آموزش و پرورش", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSource", Code = 4, Value = "دولتی", Description = "رویداد دولتی", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // EventSubject - موضوعات رویداد
                new() { TableName = "EventSubject", Code = 1, Value = "آموزشی", Description = "موضوعات آموزشی", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSubject", Code = 2, Value = "پرورشی", Description = "موضوعات پرورشی", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSubject", Code = 3, Value = "فرهنگی", Description = "موضوعات فرهنگی", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSubject", Code = 4, Value = "ورزشی", Description = "موضوعات ورزشی", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSubject", Code = 5, Value = "اداری", Description = "موضوعات اداری", IsActive = true, DisplayOrder = 5, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSubject", Code = 6, Value = "مالی", Description = "موضوعات مالی", IsActive = true, DisplayOrder = 6, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSubject", Code = 7, Value = "فناوری اطلاعات", Description = "موضوعات IT", IsActive = true, DisplayOrder = 7, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // Urgency - میزان فوریت
                new() { TableName = "Urgency", Code = 1, Value = "عادی", Description = "اولویت عادی", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "Urgency", Code = 2, Value = "متوسط", Description = "اولویت متوسط", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "Urgency", Code = 3, Value = "بالا", Description = "اولویت بالا", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "Urgency", Code = 4, Value = "فوری", Description = "فوری و حیاتی", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "Urgency", Code = 5, Value = "بحرانی", Description = "وضعیت بحرانی", IsActive = true, DisplayOrder = 5, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // ScopeType - نوع محدوده
                new() { TableName = "ScopeType", Code = 1, Value = "استانی", Description = "سطح استان", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ScopeType", Code = 2, Value = "منطقه‌ای", Description = "سطح منطقه", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ScopeType", Code = 3, Value = "مدرسه", Description = "سطح مدرسه", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ScopeType", Code = 4, Value = "کشوری", Description = "سطح کشور", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // ImpactScopeType - نوع محدوده تأثیر
                new() { TableName = "ImpactScopeType", Code = 1, Value = "استانی", Description = "تأثیر استانی", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScopeType", Code = 2, Value = "منطقه‌ای", Description = "تأثیر منطقه‌ای", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScopeType", Code = 3, Value = "مدرسه", Description = "تأثیر مدرسه", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScopeType", Code = 4, Value = "چند استانی", Description = "تأثیر چند استانی", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScopeType", Code = 5, Value = "کشوری", Description = "تأثیر کشوری", IsActive = true, DisplayOrder = 5, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // ImpactRange - دامنه تأثیر (درون/برون دستگاهی)
                new() { TableName = "ImpactRange", Code = 1, Value = "درون دستگاهی", Description = "تأثیر داخل سازمان", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactRange", Code = 2, Value = "برون دستگاهی", Description = "تأثیر خارج از سازمان", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactRange", Code = 3, Value = "ترکیبی", Description = "تأثیر داخل و خارج", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // وضعیت رویداد
                new() { TableName = "EventStatus", Code = 1, Value = "ثبت شده", Description = "رویداد ثبت شده است", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Code = 2, Value = "در حال بررسی", Description = "رویداد در حال بررسی است", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Code = 3, Value = "تایید شده", Description = "رویداد تایید شده است", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Code = 4, Value = "رد شده", Description = "رویداد رد شده است", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventStatus", Code = 5, Value = "بسته شده", Description = "رویداد بسته شده است", IsActive = true, DisplayOrder = 5, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // اهمیت رویداد
                new() { TableName = "EventSeverity", Code = 1, Value = "کم", Description = "اهمیت کم", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSeverity", Code = 2, Value = "متوسط", Description = "اهمیت متوسط", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSeverity", Code = 3, Value = "زیاد", Description = "اهمیت زیاد", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "EventSeverity", Code = 4, Value = "بحرانی", Description = "اهمیت بحرانی", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // دامنه تاثیر
                new() { TableName = "ImpactScope", Code = 1, Value = "مدرسه", Description = "تاثیر در سطح مدرسه", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScope", Code = 2, Value = "ناحیه", Description = "تاثیر در سطح ناحیه", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScope", Code = 3, Value = "استان", Description = "تاثیر در سطح استان", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "ImpactScope", Code = 4, Value = "کشور", Description = "تاثیر در سطح کشور", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // وضعیت وظیفه
                new() { TableName = "TaskStatus", Code = 1, Value = "معلق", Description = "وظیفه معلق است", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "TaskStatus", Code = 2, Value = "در حال انجام", Description = "وظیفه در حال انجام است", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "TaskStatus", Code = 3, Value = "تکمیل شده", Description = "وظیفه تکمیل شده است", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "TaskStatus", Code = 4, Value = "مسدود شده", Description = "وظیفه مسدود شده است", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // اولویت وظیفه
                new() { TableName = "TaskPriority", Code = 1, Value = "کم", Description = "اولویت کم", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "TaskPriority", Code = 2, Value = "متوسط", Description = "اولویت متوسط", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "TaskPriority", Code = 3, Value = "زیاد", Description = "اولویت زیاد", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "TaskPriority", Code = 4, Value = "فوری", Description = "اولویت فوری", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },

                // نوع واحد سازمانی
                new() { TableName = "OrganizationUnitType", Code = 1, Value = "وزارتخانه", Description = "سطح وزارتخانه", IsActive = true, DisplayOrder = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "OrganizationUnitType", Code = 2, Value = "استان", Description = "سطح استان", IsActive = true, DisplayOrder = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "OrganizationUnitType", Code = 3, Value = "ناحیه", Description = "سطح ناحیه", IsActive = true, DisplayOrder = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { TableName = "OrganizationUnitType", Code = 4, Value = "مدرسه", Description = "سطح مدرسه", IsActive = true, DisplayOrder = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
            };

            await _context.DynamicTables.AddRangeAsync(dynamicTables);
            _logger.LogInformation($"Seeded {dynamicTables.Count} dynamic table entries");
        }
    }

    private async Task SeedOrganizationUnitsAsync()
    {
        if (!await _context.OrganizationUnits.AnyAsync())
        {
            var orgUnits = new List<OrganizationUnit>
            {
                new() { Code = 1, Name = "دفتر برنامه‌ریزی و نظارت راهبردی", UnitType = "Internal", ParentUnitId = null, IsActive = true, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Code = 2, Name = "دفتر فناوری اطلاعات و ارتباطات", UnitType = "Internal", ParentUnitId = null, IsActive = true, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Code = 3, Name = "دفتر امور مالی و اداری", UnitType = "Internal", ParentUnitId = null, IsActive = true, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Code = 4, Name = "دفتر آموزش ابتدایی", UnitType = "Internal", ParentUnitId = null, IsActive = true, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Code = 5, Name = "دفتر آموزش متوسطه", UnitType = "Internal", ParentUnitId = null, IsActive = true, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Code = 6, Name = "دفتر امور پرورشی و فرهنگی", UnitType = "Internal", ParentUnitId = null, IsActive = true, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
            };

            await _context.OrganizationUnits.AddRangeAsync(orgUnits);
            _logger.LogInformation($"Seeded {orgUnits.Count} organization units");
        }
    }

    private async Task SeedMasterDataAsync()
    {
        // Seed Provinces
        if (!await _context.Provinces.AnyAsync())
        {
            var provinces = new List<Province>
            {
                new() { Name = "تهران", Code = 1, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Name = "اصفهان", Code = 2, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Name = "فارس", Code = 3, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Name = "خراسان رضوی", Code = 4, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
            };

            await _context.Provinces.AddRangeAsync(provinces);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {provinces.Count} provinces");

            // Seed Regions for Tehran Province
            var tehranProvince = await _context.Provinces.FirstAsync(p => p.Name == "تهران");
            var regions = new List<Region>
            {
                new() { Name = "ناحیه 1 تهران", Code = 101, ProvinceId = tehranProvince.Id, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Name = "ناحیه 2 تهران", Code = 102, ProvinceId = tehranProvince.Id, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
                new() { Name = "ناحیه 3 تهران", Code = 103, ProvinceId = tehranProvince.Id, CreatedBy = "System", UpdatedBy = "", CreatedAt = DateTime.Now },
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
                    UpdatedBy = "",
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
                    UpdatedBy = "",
                    CreatedAt = DateTime.Now 
                },
            };

            await _context.Schools.AddRangeAsync(schools);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Seeded {schools.Count} schools");
        }
    }
}
