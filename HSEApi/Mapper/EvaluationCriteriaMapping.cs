using AutoMapper;
using Repositories.Models;
using SystemObjects;

namespace HSEApi.Mapper
{
    public class EvaluationCriteriaMapping : Profile
    {
        public EvaluationCriteriaMapping()
        {
            CreateMap<EvaluationCriteria, EvaluationCriteriaResponse>();
            CreateMap<EvaluationCriteriaAddRequest, EvaluationCriteria>();
            CreateMap<EvaluationCriteriaEditRequest, EvaluationCriteria>();
            CreateMap<EvaluationCriteria, EvaluationCriteriaForPatientResponse>();
        }
    }
}
