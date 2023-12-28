using AutoMapper;
using Repositories.Models;
using SystemObjects;

namespace HSEApi.Mapper
{
    public class SystemInformationMapping : Profile
    {
        public SystemInformationMapping() 
        {
            CreateMap<SystemInformationUpdateRequet, SystemInformation>();
        }
    }
}
