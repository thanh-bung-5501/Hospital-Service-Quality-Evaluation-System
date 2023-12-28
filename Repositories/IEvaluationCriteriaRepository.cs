using Repositories.Models;
using SystemObjects;

namespace Repositories
{
    public interface IEvaluationCriteriaRepository
    {
        Task<List<EvaluationCriteria>> GetAllEvaluationCriteria(EvaluationCriteriaRequest request, FilteredResponse filterResponse);
        //Task<List<EvaluationCriteria>> GetAllEvaluationCriteria(EvaluationCriteriaRequest request);
        Task<List<EvaluationCriteria>> GetAllEvaluationCriteria(int serId);
        Task<List<EvaluationCriteria>> GetAllEvaluationCriteria();
        Task<EvaluationCriteria> GetEvaluationCriteria(int criId);
        Task<EvaluationCriteria> GetEvaluationCriteriaByDescription(string description);
        Task<int> AddEvaluationCriteria(EvaluationCriteria evaluationCriteria);
        Task<int> EditEvaluationCriteria(EvaluationCriteria evaluationCriteria);
        Task<int> ChagneStatus(EvaluationCriteria evaluationCriteria);
        Task<List<EvaluationCriteria>> GetEvaluationCriteriaForPatient(int serId);
    }
}
