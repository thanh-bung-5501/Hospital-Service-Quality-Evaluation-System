using DataAccess;
using Repositories.Models;
using ServiceEvaObjects;

namespace Repositories
{
    public class ServiceFeedbackRepository : IServiceFeedbackRepository
    {
        private readonly ServiceFeedbackDAO _serviceFeedbackDAO = new ServiceFeedbackDAO();
        public async Task<ServiceFeedback?> GetServiceFeedbackForPatient(EvaluationResultForPatientRequest request)
        {
            return await _serviceFeedbackDAO.GetServiceFeedbackBySerIdForPatient(request.serId, request.patId, request.createdOn);
        }

        public async Task<int> CountNumberOfEvaluations(DateTime date)
        {
            return await _serviceFeedbackDAO.CountNumberOfEvaluations(date);
        }

        public async Task<int> CountNumberOfEvaluations(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _serviceFeedbackDAO.CountNumberOfEvaluations(serId, dateFrom, dateTo);
        }

        public async Task<int> CountNumberOfEvaluations(int serId)
        {
            return await _serviceFeedbackDAO.CountNumberOfEvaluations(serId);
        }

        public async Task<List<int>> CountNumberOfEvaluations(int serId, DateTime week0,
            DateTime week1, DateTime week2, DateTime week3, DateTime week4)
        {
            return await _serviceFeedbackDAO.CountNumberOfEvaluations(serId, week0, week1, week2, week3, week4);
        }

        public async Task<int> CountNumberOfPatientEvaluated(DateTime date)
        {
            return await _serviceFeedbackDAO.CountNumberOfPatientEvaluated(date);
        }
        public async Task<List<int>> CountNumberOfPatientEvaluated(int serId, DateTime week0, DateTime week1, DateTime week2, DateTime week3, DateTime week4)
        {
            return await _serviceFeedbackDAO.CountNumberOfPatientEvaluated(serId, week0, week1, week2, week3, week4);
        }

        //public async Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM(EvaluationResultForBOMRequest bOMRequest, FilteredResponse filteredResponse)
        public async Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM(ServiceFeedbackForBOMRequest bOMRequest)
        {
            var allSerFeedback = new List<ServiceFeedback>();

            try
            {
                // default list
                allSerFeedback.AddRange(await _serviceFeedbackDAO.GetAllServiceFeedbackForBOM());

                #region Filter service, patient, fromdate-todate in list
                //if (bOMRequest.serId > 0)
                //{
                //    // list by service id
                //    allSerFeedback = allSerFeedback.Where(x => x.SerId == bOMRequest.serId).ToList();
                //}
                //if (!string.IsNullOrWhiteSpace(bOMRequest.patId))
                //{
                //    // list by patient id
                //    allSerFeedback = allSerFeedback.Where(x => !string.IsNullOrWhiteSpace(x.PatientId) && x.PatientId.Equals(bOMRequest.patId)).ToList();
                //}
                if (bOMRequest.from != null && bOMRequest.to != null && bOMRequest.to != DateTime.MaxValue)
                {
                    var toDate = bOMRequest.to;
                    toDate = toDate.Value.AddDays(1);
                    allSerFeedback = allSerFeedback.Where(x => x.CreatedOn >= bOMRequest.from && x.CreatedOn <= toDate).ToList();
                }
                #endregion

                #region Search feedback in list
                //if (!string.IsNullOrWhiteSpace(filteredResponse.search))
                //{
                //    // list by searching feedback
                //    allSerFeedback = allSerFeedback.Where(x => !string.IsNullOrWhiteSpace(x.Feedback) && x.Feedback.Contains(filteredResponse.search)).ToList();
                //}
                #endregion

                #region Sorted in list
                //if (!string.IsNullOrWhiteSpace(filteredResponse.sortedBy))
                //{
                //    // list by sorting service id, patient id, created on
                //    switch (filteredResponse.sortedBy)
                //    {
                //        case Constants.SERVICE_NAME_ASC:
                //            allSerFeedback = allSerFeedback.OrderBy(x => x.SerId).ThenBy(x => x.FbId).ToList();
                //            break;
                //        case Constants.SERVICE_NAME_DESC:
                //            allSerFeedback = allSerFeedback.OrderByDescending(x => x.SerId).ThenBy(x => x.FbId).ToList();
                //            break;
                //        case Constants.PATIENT_NAME_ASC:
                //            allSerFeedback = allSerFeedback.OrderBy(x => x.PatientId).ThenBy(x => x.FbId).ToList();
                //            break;
                //        case Constants.PATIENT_NAME_DESC:
                //            allSerFeedback = allSerFeedback.OrderByDescending(x => x.PatientId).ThenBy(x => x.FbId).ToList();
                //            break;
                //        case Constants.CREATED_ON_ASC:
                //            allSerFeedback = allSerFeedback.OrderBy(x => x.CreatedOn).ThenBy(x => x.FbId).ToList();
                //            break;
                //        case Constants.CREATED_ON_DESC:
                //            allSerFeedback = allSerFeedback.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.FbId).ToList();
                //            break;
                //        default: break;
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allSerFeedback;
        }

        public async Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM()
        {
            var allSerFeedback = new List<ServiceFeedback>();

            try
            {
                // default list
                allSerFeedback.AddRange(await _serviceFeedbackDAO.GetAllServiceFeedbackForBOM());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allSerFeedback;
        }

        public async Task<ServiceFeedback?> GetServiceFeedbackForBOM(int fbId)
        {
            return await _serviceFeedbackDAO.GetServiceFeedbackByFbIdForBOM(fbId);
        }

        public async Task<ServiceFeedback?> GetServiceFeedbackByDateForBOM(DateTime createdOn)
        {
            return await _serviceFeedbackDAO.GetServiceFeedbackByDateForBOM(createdOn);
        }
    }
}
