using AutoMapper;
using EventsManagement.Shared.DTOs;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Application.Mappings
{
    /// <summary>
    /// پروفایل AutoMapper برای نقشه‌برداری Entities به DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ============= Event Mappings =============
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<CreateEventDto, Event>();

            // ============= EventTask Mappings =============
            CreateMap<EventTask, TaskDto>().ReverseMap();
            CreateMap<CreateTaskDto, EventTask>();

            // ============= TaskReply Mappings =============
            CreateMap<TaskReply, TaskReplyDto>().ReverseMap();
            CreateMap<CreateTaskReplyDto, TaskReply>();

            // ============= DynamicTable Mappings =============
            CreateMap<DynamicTable, DynamicTableDto>().ReverseMap();
            CreateMap<CreateDynamicTableDto, DynamicTable>();

            // ============= Province Mappings =============
            CreateMap<Province, ProvinceDto>().ReverseMap();
            CreateMap<CreateProvinceDto, Province>();

            // ============= Region Mappings =============
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<CreateRegionDto, Region>();

            // ============= School Mappings =============
            CreateMap<School, SchoolDto>().ReverseMap();
            CreateMap<CreateSchoolDto, School>();

            // ============= OrganizationUnit Mappings =============
            CreateMap<OrganizationUnit, OrganizationUnitDto>().ReverseMap();
            CreateMap<CreateOrganizationUnitDto, OrganizationUnit>();

            // ============= AppUser Mappings =============
            CreateMap<AppUser, LoginResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
