using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.Lockups
{
    public class RiskLikehood:Base
    {
        public int Id { get; set; }
        public int? Rate { get; set; }
     
    }
}
