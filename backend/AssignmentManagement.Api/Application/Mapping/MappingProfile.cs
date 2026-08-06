using AutoMapper;
using AssignmentManagement.Api.Application.DTOs.Admin;
using AssignmentManagement.Api.Application.DTOs.Auth;
using AssignmentManagement.Api.Domain.Entities;

namespace AssignmentManagement.Api.Application.Mapping;

/// <summary>
/// AutoMapper profile containing all entity ↔ DTO mapping configurations.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── User ──────────────────────────────────────────────────────────────
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Role,     opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<User, UserListItemDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Role,     opt => opt.MapFrom(src => src.Role.ToString()));

        // ── Class ─────────────────────────────────────────────────────────────
        CreateMap<SchoolClass, ClassDto>()
            .ForMember(dest => dest.StudentCount, opt => opt.Ignore());

        CreateMap<CreateClassDto, SchoolClass>();

        // ── Subject ───────────────────────────────────────────────────────────
        CreateMap<Subject, SubjectDto>();
        CreateMap<CreateSubjectDto, Subject>();

        // ── TeacherSubject ───────────────────────────────────────────────────
        CreateMap<TeacherSubject, TeacherSubjectDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.FullName : string.Empty))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : string.Empty))
            .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Code : string.Empty))
            .ForMember(dest => dest.ClassName,   opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : string.Empty));
    }
}
