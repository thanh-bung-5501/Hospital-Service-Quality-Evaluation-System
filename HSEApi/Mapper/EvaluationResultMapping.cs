using AutoMapper;
using Repositories.Models;
using ServiceEvaObjects;

namespace HSEApi.Mapper
{
    public class EvaluationResultMapping : Profile
    {
        public EvaluationResultMapping()
        {
            CreateMap<EvaluationData, EvaluationResultForPatientResponse>();
            CreateMap<EvaluationDataAnswer, EvaluationData>();
            CreateMap<EvaluationData, EvaluationResultForBOMResponse>();
        }
    }
}
