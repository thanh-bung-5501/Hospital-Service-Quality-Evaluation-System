using Microsoft.EntityFrameworkCore;
using ServiceEvaObjects;

namespace DataAccess
{
    public class ServiceFeedbackDAO
    {
        private readonly ServiceEvaDBCotext _context = new ServiceEvaDBCotext();

        public async Task<ServiceFeedback?> GetServiceFeedbackBySerIdForPatient(int serId, string patId, DateTime? createdOn)
        {
            var evaData = new ServiceFeedback();
            try
            {
                // get ser...feed... for patient
                evaData = await _context.ServiceFeedback.SingleOrDefaultAsync(x => x.SerId == serId && x.PatientId!.Equals(patId) &&
                x.CreatedOn == createdOn && x.Status == true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaData!;
        }

        public async Task<int> CountNumberOfEvaluations(DateTime date)
        {
            return await _context.ServiceFeedback.CountAsync(x => date.AddMonths(-1) <= x.CreatedOn && x.CreatedOn < date);// 1/12/2023
        }

        public async Task<int> CountNumberOfEvaluations(int serId, DateTime dateFrom, DateTime dateTo)
        {
            return await _context.ServiceFeedback.CountAsync(x => x.SerId == serId && dateFrom <= x.CreatedOn && x.CreatedOn <= dateTo.AddDays(1));
        }

        public async Task<int> CountNumberOfEvaluations(int serId)
        {
            return await _context.ServiceFeedback.Where(x => x.SerId == serId).CountAsync();
        }

        public async Task<List<int>> CountNumberOfEvaluations(int serId, DateTime week0,
            DateTime week1, DateTime week2, DateTime week3, DateTime week4)
        {
            var query = _context.ServiceFeedback.AsQueryable().Where(x => x.CreatedOn < week4 && x.SerId == serId);

            int rsWeek1 = await query.Where(x => week0 <= x.CreatedOn && x.CreatedOn < week1).CountAsync();
            int rsWeek2 = await query.Where(x => week1 <= x.CreatedOn && x.CreatedOn < week2).CountAsync();
            int rsWeek3 = await query.Where(x => week2 <= x.CreatedOn && x.CreatedOn < week3).CountAsync();
            int rsWeek4 = await query.Where(x => week3 <= x.CreatedOn).CountAsync();

            return new List<int>
            {
                rsWeek1 , rsWeek2 , rsWeek3 , rsWeek4
            };
        }

        public async Task<int> CountNumberOfPatientEvaluated(DateTime date)
        {
            var query = _context.ServiceFeedback.AsQueryable().Where(x => date.AddMonths(-1) <= x.CreatedOn && x.CreatedOn < date);

            return await query.Select(x => x.PatientId).Distinct().CountAsync();
        }

        public async Task<List<int>> CountNumberOfPatientEvaluated(int serId, DateTime week0,
            DateTime week1, DateTime week2, DateTime week3, DateTime week4)
        {
            var query = _context.ServiceFeedback.AsQueryable().Where(x => x.CreatedOn < week4 && x.SerId == serId);

            int rsWeek1 = await query.Where(x => week0 <= x.CreatedOn && x.CreatedOn < week1)
                .Select(x => x.PatientId).Distinct().CountAsync();
            int rsWeek2 = await query.Where(x => week1 <= x.CreatedOn && x.CreatedOn < week2)
                .Select(x => x.PatientId).Distinct().CountAsync();
            int rsWeek3 = await query.Where(x => week2 <= x.CreatedOn && x.CreatedOn < week3)
                .Select(x => x.PatientId).Distinct().CountAsync();
            int rsWeek4 = await query.Where(x => week3 <= x.CreatedOn)
                .Select(x => x.PatientId).Distinct().CountAsync();

            return new List<int>
            {
                0, rsWeek1 , rsWeek2 , rsWeek3 , rsWeek4
            };
        }

        public async Task<List<ServiceFeedback>> GetAllServiceFeedbackForBOM()
        {
            var allSerFb = new List<ServiceFeedback>();
            try
            {
                // get all ser...feed... for bom
                allSerFb = await _context.ServiceFeedback.Where(x => x.Status == true).OrderByDescending(x => x.FbId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return allSerFb;
        }

        public async Task<ServiceFeedback?> GetServiceFeedbackByFbIdForBOM(int fbId)
        {
            var serFb = new ServiceFeedback();
            try
            {
                // get ser...feed... for bom
                serFb = await _context.ServiceFeedback.SingleOrDefaultAsync(x => x.FbId == fbId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return serFb!;
        }

        public async Task<ServiceFeedback?> GetServiceFeedbackByDateForBOM(DateTime createdOn)
        {
            var evaData = new ServiceFeedback();
            try
            {
                // get ser...feed... for bom
                evaData = await _context.ServiceFeedback.SingleOrDefaultAsync(x => x.CreatedOn == createdOn);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return evaData!;
        }
    }
}
