using AutoMapper;
using Repositories.Models;
using UserObjects;

namespace HSEApi.Mapper
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == true ? "Active" : "Block"));

            CreateMap<User, UserDetailsResponse>()
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == true ? "Active" : "Block"));

            CreateMap<User, UserProfileResponse>();

            CreateMap<UserAddRequest, User>();

            CreateMap<RoleDistribution, UserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.MAdmin == true ? "Admin"
                : src.MBOM == true ? "BOM"
                : src.MQAO == true ? "QAO"
                : "No Role"));

            CreateMap<RoleDistribution, RoleDistributionResponse>();
            CreateMap<RoleDistributionAddRequest, RoleDistribution>();
            CreateMap<RoleDistributionEditRequest, RoleDistribution>();
        }
    }
}
