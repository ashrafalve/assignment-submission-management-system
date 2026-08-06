using AutoMapper;
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
    }
}
