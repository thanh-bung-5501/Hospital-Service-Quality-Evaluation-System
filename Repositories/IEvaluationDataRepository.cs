using Repositories.Models;
using ServiceEvaObjects;

namespace Repositories
{
    public interface IEvaluationDataRepository
    {
        Task<EvaluationData?> GetEvaluationResultForPatient(int criId, EvaluationResultForPatientRequest request);
        Task<int> CountVoteByCriId(int criId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountVoteByCriIdAndPoint(int criId, int point, DateTime dateFrom, DateTime dateTo);
        Task<int> CountConcurVote(DateTime date);
        Task<int> CountVote(DateTime date);
        Task<int> CountVote(List<int> criIds, int point, DateTime dateFrom, DateTime dateTo);
        Task<int> CountVote(List<int> criIds, DateTime dateFrom, DateTime dateTo);
        Task<int> TotalPoint(int serId, DateTime dateFrom, DateTime dateTo);
        Task<int> TotalPointByCriId(int criteriaId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountConcurVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountNeutralVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountDisconcurVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountVoteByPoint(int point, DateTime date);
        Task<List<EvaluationData>> GetAllEvaluationResultForBOM(EvaluationResultForBOMRequest bOMRequest, FilteredResponse filteredResponse);
        Task<List<EvaluationData>> GetAllEvaluationResultForBOM(DateTime dateFrom, DateTime dateTo);
        Task<int> CountAllEvaluationResultForBOM();
        Task<EvaluationData?> GetEvaluationResultForBOM(int evaDataId);
    }
}
