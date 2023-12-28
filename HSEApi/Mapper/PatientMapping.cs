using AutoMapper;
using Repositories.Models;
using ServiceEvaObjects;

namespace HSEApi.Mapper
{
    public class PatientMapping : Profile
    {
        public PatientMapping()
        {
            CreateMap<Patient, PatientResponse>().ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
        }
    }
}
