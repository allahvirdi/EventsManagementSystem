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
    /// کنترلر مدیریت مدارس
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SchoolsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchoolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت تمام مدارس
        /// GET: api/Schools
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<SchoolDto>>>> GetAll()
        {
            try
            {
                var schools = await _context.Schools
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.Name)
                    .Select(s => new SchoolDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        RegionId = s.RegionId,
                        ProvinceId = s.ProvinceId,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<SchoolDto>>.Success(schools, $"{schools.Count} مدرسه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت مدارس");
                return Ok(ApiResult<List<SchoolDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت مدارس یک منطقه
        /// GET: api/Schools/ByRegion/{regionId}
        /// </summary>
        [HttpGet("ByRegion/{regionId}")]
        public async Task<ActionResult<ApiResult<List<SchoolDto>>>> GetByRegion(int regionId)
        {
            try
            {
                var schools = await _context.Schools
                    .Where(s => s.RegionId == regionId && !s.IsDeleted)
                    .OrderBy(s => s.Name)
                    .Select(s => new SchoolDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        RegionId = s.RegionId,
                        ProvinceId = s.ProvinceId,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<SchoolDto>>.Success(schools, $"{schools.Count} مدرسه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت مدارس منطقه {regionId}");
                return Ok(ApiResult<List<SchoolDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت مدارس یک استان
        /// GET: api/Schools/ByProvince/{provinceId}
        /// </summary>
        [HttpGet("ByProvince/{provinceId}")]
        public async Task<ActionResult<ApiResult<List<SchoolDto>>>> GetByProvince(int provinceId)
        {
            try
            {
                var schools = await _context.Schools
                    .Where(s => s.ProvinceId == provinceId && !s.IsDeleted)
                    .OrderBy(s => s.Name)
                    .Select(s => new SchoolDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        RegionId = s.RegionId,
                        ProvinceId = s.ProvinceId,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<SchoolDto>>.Success(schools, $"{schools.Count} مدرسه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت مدارس استان {provinceId}");
                return Ok(ApiResult<List<SchoolDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// جستجو در مدارس
        /// GET: api/Schools/Search?keyword=
        /// </summary>
        [HttpGet("Search")]
        public async Task<ActionResult<ApiResult<List<SchoolDto>>>> Search([FromQuery] string keyword)
        {
            try
            {
                var query = _context.Schools.Where(s => !s.IsDeleted);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(s => s.Name.Contains(keyword) || 
                                           s.Address.Contains(keyword) ||
                                           s.Code.ToString().Contains(keyword));
                }

                var schools = await query
                    .OrderBy(s => s.Name)
                    .Select(s => new SchoolDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        RegionId = s.RegionId,
                        ProvinceId = s.ProvinceId,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<SchoolDto>>.Success(schools, $"{schools.Count} مدرسه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در جستجوی مدارس");
                return Ok(ApiResult<List<SchoolDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک مدرسه
        /// GET: api/Schools/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<SchoolDto>>> GetById(int id)
        {
            try
            {
                var school = await _context.Schools
                    .Where(s => s.Id == id && !s.IsDeleted)
                    .Select(s => new SchoolDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        RegionId = s.RegionId,
                        ProvinceId = s.ProvinceId,
                        Address = s.Address,
                        PhoneNumber = s.PhoneNumber
                    })
                    .FirstOrDefaultAsync();

                if (school == null)
                    return Ok(ApiResult<SchoolDto>.Failure("مدرسه یافت نشد", 404));

                return Ok(ApiResult<SchoolDto>.Success(school));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت مدرسه {id}");
                return Ok(ApiResult<SchoolDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد مدرسه جدید
        /// POST: api/Schools
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult<SchoolDto>>> Create([FromBody] CreateSchoolDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // بررسی تکراری بودن نام
                var exists = await _context.Schools
                    .AnyAsync(s => s.Name == model.Name && !s.IsDeleted);
                if (exists)
                    return Ok(ApiResult<SchoolDto>.Failure("نام مدرسه تکراری است", 400));

                // بررسی تکراری بودن کد
                var codeExists = await _context.Schools
                    .AnyAsync(s => s.Code == model.Code && !s.IsDeleted);
                if (codeExists)
                    return Ok(ApiResult<SchoolDto>.Failure("کد مدرسه تکراری است", 400));

                var school = new School
                {
                    Name = model.Name,
                    Code = model.Code,
                    RegionId = model.RegionId,
                    ProvinceId = model.ProvinceId,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.Schools.Add(school);
                await _context.SaveChangesAsync();

                var dto = new SchoolDto
                {
                    Id = school.Id,
                    Name = school.Name,
                    Code = school.Code,
                    RegionId = school.RegionId,
                    ProvinceId = school.ProvinceId,
                    Address = school.Address,
                    PhoneNumber = school.PhoneNumber
                };

                Log.Information($"مدرسه جدید ایجاد شد: {school.Name}");
                return Ok(ApiResult<SchoolDto>.Success(dto, "مدرسه با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد مدرسه");
                return Ok(ApiResult<SchoolDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش مدرسه
        /// PUT: api/Schools/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult<SchoolDto>>> Update(int id, [FromBody] CreateSchoolDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var school = await _context.Schools.FindAsync(id);
                if (school == null || school.IsDeleted)
                    return Ok(ApiResult<SchoolDto>.Failure("مدرسه یافت نشد", 404));

                // بررسی تکراری بودن نام
                var exists = await _context.Schools
                    .AnyAsync(s => s.Name == model.Name && s.Id != id && !s.IsDeleted);
                if (exists)
                    return Ok(ApiResult<SchoolDto>.Failure("نام مدرسه تکراری است", 400));

                // بررسی تکراری بودن کد
                var codeExists = await _context.Schools
                    .AnyAsync(s => s.Code == model.Code && s.Id != id && !s.IsDeleted);
                if (codeExists)
                    return Ok(ApiResult<SchoolDto>.Failure("کد مدرسه تکراری است", 400));

                school.Name = model.Name;
                school.Code = model.Code;
                school.RegionId = model.RegionId;
                school.ProvinceId = model.ProvinceId;
                school.Address = model.Address;
                school.PhoneNumber = model.PhoneNumber;
                school.UpdatedBy = userId;
                school.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var dto = new SchoolDto
                {
                    Id = school.Id,
                    Name = school.Name,
                    Code = school.Code,
                    RegionId = school.RegionId,
                    ProvinceId = school.ProvinceId,
                    Address = school.Address,
                    PhoneNumber = school.PhoneNumber
                };

                Log.Information($"مدرسه {id} ویرایش شد");
                return Ok(ApiResult<SchoolDto>.Success(dto, "مدرسه با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش مدرسه {id}");
                return Ok(ApiResult<SchoolDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف مدرسه
        /// DELETE: api/Schools/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var school = await _context.Schools.FindAsync(id);
                if (school == null || school.IsDeleted)
                    return Ok(ApiResult.Failure("مدرسه یافت نشد", 404));

                school.IsDeleted = true;
                school.UpdatedBy = userId;
                school.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"مدرسه {id} حذف شد");
                return Ok(ApiResult.Success("مدرسه با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف مدرسه {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }
}
