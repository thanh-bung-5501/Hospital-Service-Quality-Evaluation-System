using AutoMapper;
using Repositories.Models;
using SystemObjects;

namespace HSEApi.Mapper
{
    public class ServiceMapping : Profile
    {
        public ServiceMapping()
        {
            CreateMap<Service, ServiceResponse>();
            CreateMap<Service, ReportServiceResponse>();

            CreateMap<ServiceCreateRequest, Service>();
            CreateMap<ServiceUpdateRequest, Service>();

            CreateMap<Service, ServiceResponseForPatient>();
        }
    }
}
