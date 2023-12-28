using Repositories.Models;
using ServiceEvaObjects;

namespace Repositories
{
    public interface IServiceFeedbackRepository
    {
        Task<ServiceFeedback?> GetServiceFeedbackForPatient(EvaluationResultForPatientRequest request);
        Task<int> CountNumberOfEvaluations(DateTime date);
        Task<int> CountNumberOfEvaluations(int serId, DateTime dateFrom, DateTime dateTo);
        Task<int> CountNumberOfEvaluations(int serId);
        Task<List<int>> CountNumberOfEvaluations(int serId, DateTime week0, DateTime week1, DateTime week2, DateTime week3, DateTime week4);
        Task<int> CountNumberOfPatientEvaluated(DateTime date);
        Task<List<int>> CountNumberOfPatientEvaluated(int serId, DateTime week0, DateTime week1, DateTime week2, DateTime week3, DateTime week4);
        //Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM(EvaluationResultForBOMRequest bOMRequest, FilteredResponse filteredResponse);
        Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM(ServiceFeedbackForBOMRequest bOMRequest);
        Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM();
        Task<ServiceFeedback?> GetServiceFeedbackForBOM(int fbId);
        Task<ServiceFeedback?> GetServiceFeedbackByDateForBOM(DateTime createdOn);
    }
}
