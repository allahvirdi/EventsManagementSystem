using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.DTOs;
using EventsManagement.Shared.Entities;
using EventsManagement.Infrastructure.Data;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت مناطق
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت تمام مناطق
        /// GET: api/Regions
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<List<RegionDto>>>> GetAll()
        {
            try
            {
                var regions = await _context.Regions
                    .Where(r => !r.IsDeleted)
                    .OrderBy(r => r.Name)
                    .Select(r => new RegionDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Code = r.Code,
                        ProvinceId = r.ProvinceId
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<RegionDto>>.Success(regions, $"{regions.Count} منطقه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت مناطق");
                return Ok(ApiResult<List<RegionDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت مناطق یک استان
        /// GET: api/Regions/ByProvince/{provinceId}
        /// </summary>
        [HttpGet("ByProvince/{provinceId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<List<RegionDto>>>> GetByProvince(int provinceId)
        {
            try
            {
                var regions = await _context.Regions
                    .Where(r => r.ProvinceId == provinceId && !r.IsDeleted)
                    .OrderBy(r => r.Name)
                    .Select(r => new RegionDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Code = r.Code,
                        ProvinceId = r.ProvinceId
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<RegionDto>>.Success(regions, $"{regions.Count} منطقه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت مناطق استان {provinceId}");
                return Ok(ApiResult<List<RegionDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک منطقه
        /// GET: api/Regions/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<RegionDto>>> GetById(int id)
        {
            try
            {
                var region = await _context.Regions
                    .Where(r => r.Id == id && !r.IsDeleted)
                    .Select(r => new RegionDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Code = r.Code,
                        ProvinceId = r.ProvinceId
                    })
                    .FirstOrDefaultAsync();

                if (region == null)
                    return Ok(ApiResult<RegionDto>.Failure("منطقه یافت نشد", 404));

                return Ok(ApiResult<RegionDto>.Success(region));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت منطقه {id}");
                return Ok(ApiResult<RegionDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد منطقه جدید
        /// POST: api/Regions
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult<RegionDto>>> Create([FromBody] CreateRegionDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // بررسی وجود استان
                var provinceExists = await _context.Provinces
                    .AnyAsync(p => p.Id == model.ProvinceId && !p.IsDeleted);
                if (!provinceExists)
                    return Ok(ApiResult<RegionDto>.Failure("استان یافت نشد", 404));

                // بررسی تکراری بودن نام
                var exists = await _context.Regions
                    .AnyAsync(r => r.Name == model.Name && r.ProvinceId == model.ProvinceId && !r.IsDeleted);
                if (exists)
                    return Ok(ApiResult<RegionDto>.Failure("نام منطقه در این استان تکراری است", 400));

                // بررسی تکراری بودن کد
                var codeExists = await _context.Regions
                    .AnyAsync(r => r.Code == model.Code && !r.IsDeleted);
                if (codeExists)
                    return Ok(ApiResult<RegionDto>.Failure("کد منطقه تکراری است", 400));

                var region = new Region
                {
                    Name = model.Name,
                    Code = model.Code,
                    ProvinceId = model.ProvinceId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.Regions.Add(region);
                await _context.SaveChangesAsync();

                var dto = new RegionDto
                {
                    Id = region.Id,
                    Name = region.Name,
                    Code = region.Code,
                    ProvinceId = region.ProvinceId
                };

                Log.Information($"منطقه جدید ایجاد شد: {region.Name}");
                return Ok(ApiResult<RegionDto>.Success(dto, "منطقه با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد منطقه");
                return Ok(ApiResult<RegionDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش منطقه
        /// PUT: api/Regions/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult<RegionDto>>> Update(int id, [FromBody] CreateRegionDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var region = await _context.Regions.FindAsync(id);
                if (region == null || region.IsDeleted)
                    return Ok(ApiResult<RegionDto>.Failure("منطقه یافت نشد", 404));

                // بررسی وجود استان
                var provinceExists = await _context.Provinces
                    .AnyAsync(p => p.Id == model.ProvinceId && !p.IsDeleted);
                if (!provinceExists)
                    return Ok(ApiResult<RegionDto>.Failure("استان یافت نشد", 404));

                // بررسی تکراری بودن نام
                var exists = await _context.Regions
                    .AnyAsync(r => r.Name == model.Name && r.ProvinceId == model.ProvinceId && r.Id != id && !r.IsDeleted);
                if (exists)
                    return Ok(ApiResult<RegionDto>.Failure("نام منطقه در این استان تکراری است", 400));

                // بررسی تکراری بودن کد
                var codeExists = await _context.Regions
                    .AnyAsync(r => r.Code == model.Code && r.Id != id && !r.IsDeleted);
                if (codeExists)
                    return Ok(ApiResult<RegionDto>.Failure("کد منطقه تکراری است", 400));

                region.Name = model.Name;
                region.Code = model.Code;
                region.ProvinceId = model.ProvinceId;
                region.UpdatedBy = userId;
                region.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var dto = new RegionDto
                {
                    Id = region.Id,
                    Name = region.Name,
                    Code = region.Code,
                    ProvinceId = region.ProvinceId
                };

                Log.Information($"منطقه {id} ویرایش شد");
                return Ok(ApiResult<RegionDto>.Success(dto, "منطقه با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش منطقه {id}");
                return Ok(ApiResult<RegionDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف منطقه
        /// DELETE: api/Regions/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var region = await _context.Regions.FindAsync(id);
                if (region == null || region.IsDeleted)
                    return Ok(ApiResult.Failure("منطقه یافت نشد", 404));

                // بررسی وابستگی
                var hasSchools = await _context.Schools
                    .AnyAsync(s => s.RegionId == id && !s.IsDeleted);
                if (hasSchools)
                    return Ok(ApiResult.Failure("منطقه دارای مدارس وابسته است", 400));

                region.IsDeleted = true;
                region.UpdatedBy = userId;
                region.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"منطقه {id} حذف شد");
                return Ok(ApiResult.Success("منطقه با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف منطقه {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }
}
