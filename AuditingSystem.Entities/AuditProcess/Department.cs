using AuditingSystem.Entities.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.AuditProcess
{
    public class Department : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string? Source { get; set; }
        public virtual Company? Company { get; set; }

        public virtual IEnumerable<User>? Users { get; set; }
        public virtual IEnumerable<Function>? Functions { get; set; }
   
    }
}
