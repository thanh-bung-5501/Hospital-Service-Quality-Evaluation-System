using ServiceEvaObjects;

namespace Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatient(string patId);

        Task<Patient> VerifyPatientCode(string patientId);

        Task<bool> SubmitEvaluation(ServiceFeedback feedback, List<EvaluationData> evaluationDatas);

        Task<int> AddAllEvaluaionData(List<EvaluationData> evaluationDatas);

        Task<bool> AddServiceFeedback(ServiceFeedback serviceFeedback);

        Task<int> CountAllPatient();
    }
}
