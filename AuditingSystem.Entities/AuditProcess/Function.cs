using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.AuditProcess
{
    public class Function:Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public string? Source { get; set; }
        public virtual Department? Department { get; set; }
        public virtual IEnumerable<Activity>? Activities { get; set; }

    
    }
}
