using Microsoft.EntityFrameworkCore;
using SystemObjects;

namespace DataAccess
{
    public class ServiceDAO
    {
        private readonly SystemDBContext _context = new SystemDBContext();
        public IQueryable<Service> GetServiceQueryAble() => _context.Service.AsQueryable();
        public async Task<List<Service>> GetServices() => await _context.Service.OrderByDescending(x => x.SerId).ToListAsync();
        public async Task<List<Service>> GetServicesByStatus(bool status)
        {
            return await _context.Service.Where(x => x.Status == status).OrderByDescending(x => x.SerId).ToListAsync();
        }
        public async Task<Service> GetService(int id)
        {
            var service = new Service();
            try
            {
                service = await _context.Service.SingleOrDefaultAsync(x => x.SerId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return service!;
        }        
        
        public async Task<Service> GetService(int id, bool status)
        {
            var service = new Service();
            try
            {
                service = await _context.Service.SingleOrDefaultAsync(x => x.SerId == id && x.Status == status);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return service!;
        }

        public async Task<bool> CreateService(Service service, string iconPath)
        {
            bool result = false;
            try
            {
                await _context.Service.AddAsync(new Service
                {
                    SerName = service.SerName,
                    SerDesc = service.SerDesc,
                    Icon = iconPath,
                    CreatedOn = DateTime.Now,
                    CreatedBy = service.CreatedBy,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = service.CreatedBy,
                    Status = service.Status,
                });

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
        public async Task<bool> UpdateService(Service service, string? iconPath)
        {
            bool result = false;
            try
            {
                var serviceOld = await _context.Service.SingleOrDefaultAsync(x => x.SerId == service.SerId);

                if (serviceOld == null)
                {
                    throw new Exception("Null service");
                }
                else
                {
                    if (iconPath != null)
                    {
                        serviceOld.Icon = iconPath;

                    }
                    serviceOld.SerName = service.SerName;
                    serviceOld.SerDesc = service.SerDesc;
                    serviceOld.ModifiedOn = DateTime.Now;
                    serviceOld.ModifiedBy = service.ModifiedBy;

                    int res = await _context.SaveChangesAsync();

                    if (res != 0) result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<bool> ChagneStatus(int id, int modifiedBy)
        {
            bool result = false;
            try
            {
                var serviceOld = await _context.Service.SingleOrDefaultAsync(x => x.SerId == id);

                if (serviceOld == null)
                {
                    throw new Exception("Null service");
                }
                else
                {
                    serviceOld.Status = !serviceOld.Status;
                    serviceOld.ModifiedOn = DateTime.Now;
                    serviceOld.ModifiedBy = modifiedBy;
                    int res = await _context.SaveChangesAsync();

                    if (res != 0) result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<bool> DeleteService(int serviceId)
        {
            bool result = false;
            try
            {
                var service = await _context.Service.SingleOrDefaultAsync(x => x.SerId == serviceId);

                if (service == null)
                {
                    throw new Exception("Null service");
                }
                else
                {
                    service.Status = false;

                    int res = await _context.SaveChangesAsync();

                    if (res != 0) result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
