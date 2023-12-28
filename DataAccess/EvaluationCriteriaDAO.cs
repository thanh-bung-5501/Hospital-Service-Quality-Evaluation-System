using Microsoft.EntityFrameworkCore;
using SystemObjects;

namespace DataAccess
{
    public class EvaluationCriteriaDAO
    {
        private readonly SystemDBContext _context = new SystemDBContext();

        public async Task<List<EvaluationCriteria>> GetAllEvaluationCriteria()
        {
            var allEvaluationCriteria = new List<EvaluationCriteria>();
            try
            {
                // default list 
                allEvaluationCriteria = await _context.EvaluationCriteria
                    .OrderByDescending(x => x.CriId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaluationCriteria;
        }

        public async Task<List<EvaluationCriteria>> GetAllEvaluationCriteriaBySerId(int serId)
        {
            var allEvaluationCriteria = new List<EvaluationCriteria>();
            try
            {
                // default list 
                allEvaluationCriteria = await _context.EvaluationCriteria.OrderByDescending(x => x.CriId).ToListAsync();

                // filter by service id
                if (serId > 0)
                {
                    allEvaluationCriteria = allEvaluationCriteria.Where(x => x.SerId == serId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaluationCriteria;
        }

        public async Task<EvaluationCriteria> GetEvaluationCriteria(int criId)
        {
            var evaluationCriteria = new EvaluationCriteria();
            try
            {
                evaluationCriteria = await _context.EvaluationCriteria.SingleOrDefaultAsync(x => x.CriId == criId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaluationCriteria!;
        }

        public async Task<EvaluationCriteria> GetEvaluationCriteriaByDescription(string description)
        {
            var evaluationCriteria = new EvaluationCriteria();
            try
            {
                evaluationCriteria = await _context.EvaluationCriteria.SingleOrDefaultAsync(x => x.CriDesc != null && x.CriDesc.Equals(description));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaluationCriteria!;
        }

        public async Task<int> AddEvaluationCriteria(EvaluationCriteria evaluationCriteria)
        {
            int rowAffected = 0;
            try
            {
                // add new evaluation criteria
                await _context.EvaluationCriteria.AddAsync(evaluationCriteria);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return rowAffected;
            }
            return evaluationCriteria.CriId;
        }

        public async Task<int> EditEvaluationCriteria(EvaluationCriteria evaluationCriteria)
        {
            int rowAffected = 0;
            try
            {
                var oldEvaCri = await GetEvaluationCriteria(evaluationCriteria.CriId) ?? throw new Exception();

                // update current evaluation criteria
                oldEvaCri.CriDesc = evaluationCriteria.CriDesc;
                oldEvaCri.ModifiedOn = evaluationCriteria.ModifiedOn;
                oldEvaCri.ModifiedBy = evaluationCriteria.ModifiedBy;
                oldEvaCri.Note = evaluationCriteria.Note;
                oldEvaCri.SerId = evaluationCriteria.SerId;

                _context.Entry<EvaluationCriteria>(oldEvaCri).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return rowAffected;
            }
            return evaluationCriteria.CriId;
        }

        public async Task<int> ChagneStatus(EvaluationCriteria evaluationCriteria)
        {
            int rowAffected = 0;
            try
            {
                var oldEvaCri = await GetEvaluationCriteria(evaluationCriteria.CriId) ?? throw new Exception();

                // deactivate status current evaluation criteria
                oldEvaCri.ModifiedOn = evaluationCriteria.ModifiedOn;
                oldEvaCri.ModifiedBy = evaluationCriteria.ModifiedBy;
                oldEvaCri.Status = !oldEvaCri.Status;

                _context.Entry<EvaluationCriteria>(oldEvaCri).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return rowAffected;
            }
            return evaluationCriteria.CriId;
        }

        public async Task<List<EvaluationCriteria>> GetEvaluationCriteriaBySerIdForPatient(int serId)
        {
            var allEvaluationCriteria = new List<EvaluationCriteria>();
            try
            {
                // get all list eva...cri... for patient
                allEvaluationCriteria = await _context.EvaluationCriteria.
                    Where(x => x.SerId == serId && x.Status == true).OrderByDescending(x => x.CriId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaluationCriteria;
        }

        public async Task<List<int>> GetAllEvaluationCriteriaByDescription(string description)
        {
            var evaluationCriteria = new List<int>();
            try
            {
                evaluationCriteria = await _context.EvaluationCriteria.Where(x => x.CriDesc != null && x.CriDesc.Contains(description)).Select(x => x.CriId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaluationCriteria!;
        }
    }
}
