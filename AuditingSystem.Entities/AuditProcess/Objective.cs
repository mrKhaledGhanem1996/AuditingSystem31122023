using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.AuditProcess
{
    public class Objective:Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public int? ActivityId { get; set; }
        public string? Source { get; set; }
        public virtual Activity? Activity { get; set; }
        public virtual IEnumerable<Tasks>? Tasks { get; set; }
 
       
    }
}
