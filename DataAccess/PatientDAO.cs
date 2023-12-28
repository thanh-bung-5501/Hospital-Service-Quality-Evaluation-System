using Microsoft.EntityFrameworkCore;
using ServiceEvaObjects;

namespace DataAccess
{
    public class PatientDAO
    {
        private readonly ServiceEvaDBCotext _context = new ServiceEvaDBCotext();

        public async Task<Patient?> GetPatient(string patId)
        {
            var patient = new Patient();
            try
            {
                patient = await _context.Patient.SingleOrDefaultAsync(x => x.PatientId.Equals(patId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return patient!;
        }

        public async Task<Patient> VerifyPatientCode(string patientId)
        {
            try
            {
                var patient = await _context.Patient.SingleOrDefaultAsync(x => x.PatientId.Equals(patientId));
                return patient;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> SubmitEvaluation(ServiceFeedback newServiceFeedback, List<EvaluationData> newEvaluaionData)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool success;
                try
                {
                    var dateCreated = DateTime.Now;

                    newServiceFeedback.CreatedOn = dateCreated;
                    newServiceFeedback.Status = true;
                    await _context.ServiceFeedback.AddAsync(newServiceFeedback);
                    await _context.SaveChangesAsync();

                    foreach(var item in newEvaluaionData)
                    {
                        item.CreatedOn = dateCreated;
                        item.Status = true;
                    }

                    await _context.EvaluationData.AddRangeAsync(newEvaluaionData);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    success = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return success;
            }
        }

        public async Task<bool> AddEvaluationData(EvaluationData evaluationData)
        {
            bool result = false;
            try
            {
                await _context.AddAsync(evaluationData);
                int res = await _context.SaveChangesAsync();
                if (res != 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public async Task<bool> AddServiceFeedback(ServiceFeedback serviceFeedback)
        {
            bool result = false;
            try
            {
                await _context.AddAsync(serviceFeedback);
                int res = await _context.SaveChangesAsync();
                if (res != 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public async Task<int> CountAllPatient()
        {
            return await _context.Patient.CountAsync();
        }
    }
}
