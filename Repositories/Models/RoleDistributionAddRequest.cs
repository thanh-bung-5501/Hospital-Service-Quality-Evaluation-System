using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class RoleDistributionAddRequest
    {
        public int UserId { get; set; }

        public bool MAdmin { get; set; }

        public bool MQAO { get; set; }

        public bool MBOM { get; set; }

        public int MSystem { get; set; }

        public int MUser { get; set; }

        public int MService { get; set; }

        public int MCriteria { get; set; }
    }
}
