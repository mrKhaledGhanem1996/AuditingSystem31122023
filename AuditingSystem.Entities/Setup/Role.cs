using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.Setup
{
    public class Role : Base
    {
        public int Id { get; set; }
        public virtual IEnumerable<User>? Users { get; set; }
         
    }
}
