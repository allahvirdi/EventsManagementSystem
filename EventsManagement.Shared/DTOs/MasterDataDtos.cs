namespace EventsManagement.Shared.DTOs
{
    /// <summary>
    /// DTO برای جداول پویا
    /// </summary>
    public class CreateDynamicTableDto
    {
        public string TableName { get; set; }
        public int Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentId { get; set; }
    }

    /// <summary>
    /// DTO برای نمایش
    /// </summary>
    public class DynamicTableDto
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO برای استان
    /// </summary>
    public class CreateProvinceDto
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }

    public class ProvinceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
    }

    /// <summary>
    /// DTO برای منطقه
    /// </summary>
    public class CreateRegionDto
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public int ProvinceId { get; set; }
    }

    public class RegionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int ProvinceId { get; set; }
    }

    /// <summary>
    /// DTO برای مدرسه
    /// </summary>
    public class CreateSchoolDto
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public int? RegionId { get; set; }
        public int? ProvinceId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SchoolDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int? RegionId { get; set; }
        public int? ProvinceId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// DTO برای واحد سازمانی
    /// </summary>
    public class CreateOrganizationUnitDto
    {
        public string Name { get; set; }
        public string UnitType { get; set; }
        public int Code { get; set; }
        public int? ParentUnitId { get; set; }
        public int? ProvinceId { get; set; }
        public int? RegionId { get; set; }
        public int? SchoolId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class OrganizationUnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UnitType { get; set; }
        public int Code { get; set; }
        public int? ParentUnitId { get; set; }
        public int? ProvinceId { get; set; }
        public int? RegionId { get; set; }
        public int? SchoolId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
