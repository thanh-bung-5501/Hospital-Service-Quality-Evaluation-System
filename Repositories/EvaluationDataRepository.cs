using DataAccess;
using Repositories.Models;
using Repositories.Utils;
using ServiceEvaObjects;

namespace Repositories
{
    public class EvaluationDataRepository : IEvaluationDataRepository
    {
        private readonly EvaluationDataDAO _evaluationDataDAO = new EvaluationDataDAO();
        private readonly EvaluationCriteriaDAO _evaluationCriteriaDAO = new EvaluationCriteriaDAO();
        public async Task<EvaluationData?> GetEvaluationResultForPatient(int criId, EvaluationResultForPatientRequest request)
        {
            return await _evaluationDataDAO.GetEvaluationResultByCriIdForPatient(criId, request.patId, request.createdOn);
        }
        public async Task<int> CountVoteByCriId(int criId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountVoteByCriId(criId, dateFrom, dateTo);
        }
        public async Task<int> CountVoteByCriIdAndPoint(int criId, int point, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountVoteByCriIdAndPoint(criId, point, dateFrom, dateTo);
        }

        public async Task<int> CountConcurVote(DateTime date)
        {
            return await _evaluationDataDAO.CountConcurVote(date);
        }

        public async Task<int> CountConcurVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountConcurVoteBySerId(serId, dateFrom, dateTo);
        }

        public async Task<int> CountNeutralVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountNeutralVoteBySerId(serId, dateFrom, dateTo);
        }
        public async Task<int> CountDisconcurVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountDisconcurVoteBySerId(serId, dateFrom, dateTo);
        }

        public async Task<int> CountVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountVoteBySerId(serId, dateFrom, dateTo);
        }

        public async Task<int> CountVote(DateTime date)
        {
            return await _evaluationDataDAO.CountVote(date);
        }

        public async Task<int> CountVote(List<int> criIds, int point, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountVote(criIds, point, dateFrom, dateTo);
        }

        public async Task<int> CountVote(List<int> criIds, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.CountVote(criIds, dateFrom, dateTo);
        }

        public async Task<int> TotalPoint(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.TotalPoint(serId, dateFrom, dateTo);
        }

        public async Task<int> TotalPointByCriId(int criteriaId, DateTime dateFrom, DateTime dateTo)
        {
            return await _evaluationDataDAO.TotalPointByCriId(criteriaId, dateFrom, dateTo);
        }

        public async Task<int> CountVoteByPoint(int point, DateTime date)
        {
            return await _evaluationDataDAO.CountVoteByPoint(point, date);
        }

        public async Task<List<EvaluationData>> GetAllEvaluationResultForBOM(EvaluationResultForBOMRequest bOMRequest, FilteredResponse filteredResponse)
        {
            var allEvaResult = new List<EvaluationData>();
            var criIds = new List<int>();

            try
            {
                // default list
                allEvaResult.AddRange(await _evaluationDataDAO.GetAllEvaluationResultForBOM(filteredResponse.page, filteredResponse.pageSize));

                #region Filter service, patient, created date in list
                if (bOMRequest.serId > 0)
                {
                    var allEvaCri = await _evaluationCriteriaDAO.GetAllEvaluationCriteriaBySerId((int)bOMRequest.serId!);
                    criIds = allEvaCri.Select(x => x.CriId).ToList();
                    // list by service id
                    allEvaResult = allEvaResult.Where(x => criIds.Contains((int)x.CriId!)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(bOMRequest.patId))
                {
                    // list by patient id
                    allEvaResult = allEvaResult.Where(x => !string.IsNullOrWhiteSpace(x.PatientId) && x.PatientId.Equals(bOMRequest.patId)).ToList();
                }

                if (bOMRequest.from != null || bOMRequest.to != null)
                {
                    // list by patient id
                    allEvaResult = allEvaResult.Where(x => x.CreatedOn >= bOMRequest.from && x.CreatedOn <= bOMRequest.to).ToList();
                }
                #endregion

                #region Search criteria desc in list
                if (!string.IsNullOrWhiteSpace(filteredResponse.search))
                {
                    // list by searching description
                    var listDesc = await _evaluationCriteriaDAO.GetAllEvaluationCriteriaByDescription(filteredResponse.search);
                    allEvaResult = allEvaResult.Where(x => listDesc.Contains((int)x.CriId!)).ToList();
                }
                #endregion

                #region Sorted in list
                if (!string.IsNullOrWhiteSpace(filteredResponse.sortedBy))
                {
                    // list by sorting point, service id, patient id, created on
                    switch (filteredResponse.sortedBy)
                    {
                        case Constants.POINT_ASC:
                            allEvaResult = allEvaResult.OrderBy(x => x.Point).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.POINT_DESC:
                            allEvaResult = allEvaResult.OrderByDescending(x => x.Point).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.SERVICE_NAME_ASC:
                            allEvaResult = allEvaResult.OrderBy(x => x.CriId).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.SERVICE_NAME_DESC:
                            allEvaResult = allEvaResult.OrderByDescending(x => x.CriId).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.PATIENT_NAME_ASC:
                            allEvaResult = allEvaResult.OrderBy(x => x.PatientId).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.PATIENT_NAME_DESC:
                            allEvaResult = allEvaResult.OrderByDescending(x => x.PatientId).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.CREATED_ON_ASC:
                            allEvaResult = allEvaResult.OrderBy(x => x.CreatedOn).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        case Constants.CREATED_ON_DESC:
                            allEvaResult = allEvaResult.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.EvaDataId).ToList();
                            break;
                        default: break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaResult;
        }

        public async Task<List<EvaluationData>> GetAllEvaluationResultForBOM(DateTime dateFrom, DateTime dateTo)
        {
            var allEvaResult = new List<EvaluationData>();

            try
            {
                // default list
                allEvaResult.AddRange(await _evaluationDataDAO.GetAllEvaluationResultForBOM(dateFrom, dateTo));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaResult;
        }

        public async Task<int> CountAllEvaluationResultForBOM()
        {
            int count;
            try
            {
                // default list
                count = await _evaluationDataDAO.CountAllEvaluationResultForBOM();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public async Task<EvaluationData?> GetEvaluationResultForBOM(int evaDataId)
        {
            return await _evaluationDataDAO.GetEvaluationResultByEvaIdForBOM(evaDataId);
        }
    }
}
