using DataAccess;
using Repositories.Models;
using Repositories.Utils;
using SystemObjects;

namespace Repositories
{
    public class EvaluationCriteriaRepository : IEvaluationCriteriaRepository
    {
        private readonly EvaluationCriteriaDAO _evaluationCriteriaDAO = new EvaluationCriteriaDAO();
        public async Task<List<EvaluationCriteria>> GetAllEvaluationCriteria(EvaluationCriteriaRequest request, FilteredResponse filterResponse)
        {
            var allEvaluationCriteria = new List<EvaluationCriteria>();

            try
            {
                // default list
                allEvaluationCriteria = await _evaluationCriteriaDAO.GetAllEvaluationCriteriaBySerId((int)request.serId!);

                #region Filter created on in list
                if (request.from != null || request.to != null)
                {
                    // list by patient id
                    allEvaluationCriteria = allEvaluationCriteria.Where(x => x.CreatedOn >= request.from && x.CreatedOn <= request.to).ToList();
                }
                #endregion

                #region Search criteria desc in list
                if (!string.IsNullOrWhiteSpace(filterResponse.search))
                {
                    // list by searching description
                    allEvaluationCriteria = allEvaluationCriteria.Where(x => x.CriDesc != null && x.CriDesc.Contains(filterResponse.search)).ToList();
                }
                #endregion

                #region Sorted in list
                if (!string.IsNullOrWhiteSpace(filterResponse.sortedBy))
                {
                    // list by sorting service id, created on and modified on
                    switch (filterResponse.sortedBy)
                    {
                        case Constants.SERVICE_NAME_ASC:
                            allEvaluationCriteria = allEvaluationCriteria.OrderBy(x => x.SerId).ThenBy(x => x.CriId).ToList();
                            break;
                        case Constants.SERVICE_NAME_DESC:
                            allEvaluationCriteria = allEvaluationCriteria.OrderByDescending(x => x.SerId).ThenBy(x => x.CriId).ToList();
                            break;
                        case Constants.CREATED_ON_ASC:
                            allEvaluationCriteria = allEvaluationCriteria.OrderBy(x => x.CreatedOn).ThenBy(x => x.CriId).ToList();
                            break;
                        case Constants.CREATED_ON_DESC:
                            allEvaluationCriteria = allEvaluationCriteria.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.CriId).ToList();
                            break;
                        case Constants.MODIFIED_ON_ASC:
                            allEvaluationCriteria = allEvaluationCriteria.OrderBy(x => x.ModifiedOn).ThenBy(x => x.CriId).ToList();
                            break;
                        case Constants.MODIFIED_ON_DESC:
                            allEvaluationCriteria = allEvaluationCriteria.OrderByDescending(x => x.ModifiedOn).ThenBy(x => x.CriId).ToList();
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
            return allEvaluationCriteria;
        }

        public async Task<List<EvaluationCriteria>> GetAllEvaluationCriteria(int serId)
        {
            var allEvaluationCriteria = new List<EvaluationCriteria>();

            try
            {
                allEvaluationCriteria = await _evaluationCriteriaDAO.GetAllEvaluationCriteriaBySerId(serId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaluationCriteria;
        }

        public async Task<List<EvaluationCriteria>> GetAllEvaluationCriteria()
        {
            var allEvaluationCriteria = new List<EvaluationCriteria>();

            try
            {
                allEvaluationCriteria = await _evaluationCriteriaDAO.GetAllEvaluationCriteria();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaluationCriteria;
        }

        public async Task<EvaluationCriteria> GetEvaluationCriteria(int criId)
        {
            return await _evaluationCriteriaDAO.GetEvaluationCriteria(criId);
        }

        public async Task<EvaluationCriteria> GetEvaluationCriteriaByDescription(string description)
        {
            return await _evaluationCriteriaDAO.GetEvaluationCriteriaByDescription(description);
        }

        public async Task<int> AddEvaluationCriteria(EvaluationCriteria evaluationCriteria)
        {
            return await _evaluationCriteriaDAO.AddEvaluationCriteria(evaluationCriteria);
        }

        public async Task<int> EditEvaluationCriteria(EvaluationCriteria evaluationCriteria)
        {
            return await _evaluationCriteriaDAO.EditEvaluationCriteria(evaluationCriteria);
        }

        public async Task<int> ChagneStatus(EvaluationCriteria evaluationCriteria)
        {
            return await _evaluationCriteriaDAO.ChagneStatus(evaluationCriteria);
        }

        public async Task<List<EvaluationCriteria>> GetEvaluationCriteriaForPatient(int serId)
        {
            return await _evaluationCriteriaDAO.GetEvaluationCriteriaBySerIdForPatient(serId);
        }
    }
}
