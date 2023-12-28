using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class RoleDistributionResponse
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public bool MAdmin { get; set; }

        public bool MQAO { get; set; }

        public bool MBOM { get; set; }

        public int MSystem { get; set; }

        public int MUser { get; set; }

        public int MService { get; set; }

        public int MCriteria { get; set; }
    }
}
