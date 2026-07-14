using AutoMapper;
using GodotXR.Domain.Entities;
using System.Security.Claims;

namespace GodotXR.Infrastructure.Mapper
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<User, IEnumerable<Claim>>().ConvertUsing(src => new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, src.Id.ToString()),
                new Claim(ClaimTypes.Email, src.Email),
                new Claim(ClaimTypes.Name, src.FullName),
                new Claim(ClaimTypes.Role, src.Role != null ? src.Role.RoleName.ToString() : "Parent")
            });
        }
    }
}