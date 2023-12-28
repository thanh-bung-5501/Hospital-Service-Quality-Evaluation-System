using DataAccess;
using ServiceEvaObjects;

namespace Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDAO _patientDAO = new PatientDAO();
        public async Task<Patient?> GetPatient(string patId)
        {
            return await _patientDAO.GetPatient(patId);
        }

        public async Task<Patient> VerifyPatientCode(string patientId)
        {
            return await _patientDAO.VerifyPatientCode(patientId);
        }

        public async Task<bool> SubmitEvaluation(ServiceFeedback feedback, List<EvaluationData> evaluationDatas)
        {
            return await _patientDAO.SubmitEvaluation(feedback, evaluationDatas);
        }

        public async Task<int> AddAllEvaluaionData(List<EvaluationData> evaluationDatas)
        {
            var count = 0;
            bool result = false;
            try
            {
                foreach (EvaluationData data in evaluationDatas)
                {
                    data.EvaDataId = 0;
                    data.CreatedOn = DateTime.Now;
                    data.Status = true;
                    result = await _patientDAO.AddEvaluationData(data);
                    if (result == true)
                    {
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public async Task<bool> AddServiceFeedback(ServiceFeedback serviceFeedback)
        {
            bool result = false;
            try
            {
                serviceFeedback.FbId = 0;
                serviceFeedback.CreatedOn = DateTime.Now;
                serviceFeedback.Status = true;
                result = await _patientDAO.AddServiceFeedback(serviceFeedback);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public async Task<int> CountAllPatient()
        {
            return await _patientDAO.CountAllPatient();
        }
    }
}
