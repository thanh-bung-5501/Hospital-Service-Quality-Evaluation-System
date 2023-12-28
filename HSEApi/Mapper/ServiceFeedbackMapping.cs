using AutoMapper;
using Repositories.Models;
using ServiceEvaObjects;

namespace HSEApi.Mapper
{
    public class ServiceFeedbackMapping : Profile
    {
        public ServiceFeedbackMapping()
        {
            CreateMap<ServiceFeedback, ServiceFeedbackForPatientResponse>();
            CreateMap<EvaluationSubmitRequest, ServiceFeedback>();
            CreateMap<ServiceFeedback, ServiceFeedbackResponse>();
            CreateMap<ServiceFeedback, ServiceFeedbackForBOMResponse>();
        }
    }
}
