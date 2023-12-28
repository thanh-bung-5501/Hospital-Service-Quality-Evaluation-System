using Microsoft.EntityFrameworkCore;
using ServiceEvaObjects;
using SystemObjects;

namespace DataAccess
{
    public class EvaluationDataDAO
    {
        private readonly ServiceEvaDBCotext _serEvaContext = new ServiceEvaDBCotext();
        private readonly SystemDBContext _systemContext = new SystemDBContext();

        public async Task<EvaluationData?> GetEvaluationResultByCriIdForPatient(int criId, string patId, DateTime? createdOn)
        {
            var evaData = new EvaluationData();
            try
            {
                // get eva...Data for patient
                evaData = await _serEvaContext.EvaluationData.SingleOrDefaultAsync(x => x.CriId == criId && x.PatientId!.Equals(patId) &&
                x.CreatedOn == createdOn && x.Status == true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaData!;
        }

        public async Task<int> CountVoteByCriId(int criId, DateTime dateFrom, DateTime dateTo)
        {
            return await _serEvaContext.EvaluationData
                .Where(x => x.CriId == criId && dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1))
                .CountAsync();
        }

        public async Task<int> CountVoteByCriIdAndPoint(int criId, int point, DateTime dateFrom, DateTime dateTo)
        {
            return await _serEvaContext.EvaluationData
                .Where(x => x.CriId == criId && x.Point == point && dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1))
                .CountAsync();
        }

        public async Task<int> CountConcurVote(DateTime date)
        {
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => date.AddMonths(-1) <= x.CreatedOn && x.CreatedOn <= date);
            // Strongly agree & agree vote
            return await query.CountAsync(x => x.Point == 4 || x.Point == 5);
        }

        public async Task<int> CountConcurVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            var criterias = _systemContext.EvaluationCriteria.Where(x => x.SerId == serId);
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            int count = 0;

            foreach (var criteria in criterias)
            {
                // Strongly agree & agree vote
                var criData = await query.Where(x => x.CriId == criteria.CriId).CountAsync(x => x.Point == 4 || x.Point == 5);
                if (criData > 0) count += criData;
            }
            return count;
        }

        public async Task<int> CountNeutralVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            var criterias = _systemContext.EvaluationCriteria.Where(x => x.SerId == serId);
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            int count = 0;

            foreach (var criteria in criterias)
            {
                // neutral vote
                var criData = await query.Where(x => x.CriId == criteria.CriId).CountAsync(x => x.Point == 3);
                if (criData > 0) count += criData;
            }

            return count;
        }

        public async Task<int> CountDisconcurVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            var criterias = _systemContext.EvaluationCriteria.Where(x => x.SerId == serId);
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            int count = 0;

            foreach (var criteria in criterias)
            {
                // neutral vote
                var criData = await query.Where(x => x.CriId == criteria.CriId).CountAsync(x => x.Point == 1 || x.Point == 2);
                if (criData > 0) count += criData;
            }

            return count;
        }

        public async Task<int> CountVoteBySerId(int serId, DateTime dateFrom, DateTime dateTo)
        {
            var criterias = _systemContext.EvaluationCriteria.Where(x => x.SerId == serId);
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            int count = 0;

            foreach (var criteria in criterias)
            {
                var criData = await query.Where(x => x.CriId == criteria.CriId).CountAsync();
                if (criData > 0) count += criData;
            }

            return count;
        }

        public async Task<int> CountVote(DateTime date)
        {
            return await _serEvaContext.EvaluationData.CountAsync(x => date.AddMonths(-1) <= x.CreatedOn && x.CreatedOn <= date);
        }

        public async Task<int> CountVote(List<int> criIds, int point, DateTime dateFrom, DateTime dateTo)
        {
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            return await query.CountAsync(x => criIds.Contains(x.CriId!.Value) && x.Point == point);
        }

        public async Task<int> CountVote(List<int> criIds, DateTime dateFrom, DateTime dateTo)
        {
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            return await query.CountAsync(x => criIds.Contains(x.CriId!.Value));
        }

        public async Task<int> TotalPoint(int serId, DateTime dateFrom, DateTime dateTo)
        {
            var criterias = _systemContext.EvaluationCriteria.Where(x => x.SerId == serId);
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
            int total = 0;

            foreach (var criteria in criterias)
            {
                var criData = await query.Where(x => x.CriId == criteria.CriId).SumAsync(x => x.Point);
                if (criData > 0) total += criData.Value;
            }

            return total;
        }

        public async Task<int> TotalPointByCriId(int criteriaId, DateTime dateFrom, DateTime dateTo)
        {
            var point = await _serEvaContext.EvaluationData.Where(x => x.CriId == criteriaId && dateFrom <= x.CreatedOn
                && x.CreatedOn <= dateTo.AddDays(1)).SumAsync(x => x.Point);
            return point.Value;
        }

        public async Task<int> CountVoteByPoint(int point, DateTime date)
        {
            var query = _serEvaContext.EvaluationData.AsQueryable().Where(x => date.AddMonths(-1) <= x.CreatedOn && x.CreatedOn <= date);
            return await query.CountAsync(x => x.Point == point);
        }

        public async Task<List<EvaluationData>> GetAllEvaluationResultForBOM(DateTime dateFrom, DateTime dateTo)
        {
            var allEvaData = new List<EvaluationData>();
            try
            {
                // get all eva...Data for bom
                allEvaData = await _serEvaContext.EvaluationData.Where(x => x.Status == true && dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1))
                    .OrderByDescending(x => x.EvaDataId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaData;
        }
        public async Task<List<EvaluationData>> GetAllEvaluationResultForBOM(int page, int pageSize)
        {
            var allEvaData = new List<EvaluationData>();
            try
            {
                // get all eva...Data for bom
                allEvaData = await _serEvaContext.EvaluationData.Where(x => x.Status == true).Skip((page - 1) * pageSize).Take(pageSize)
                    .OrderByDescending(x => x.EvaDataId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allEvaData;
        }

        public async Task<int> CountAllEvaluationResultForBOM()
        {
            int total = 0;
            try
            {
                // get all eva...Data for bom
                total = await _serEvaContext.EvaluationData.Where(x => x.Status == true).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return total;
        }

        public async Task<EvaluationData?> GetEvaluationResultByEvaIdForBOM(int evaDataId)
        {
            var evaData = new EvaluationData();
            try
            {
                // get eva...Data for bom
                evaData = await _serEvaContext.EvaluationData.SingleOrDefaultAsync(x => x.EvaDataId == evaDataId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaData!;
        }
    }
}
