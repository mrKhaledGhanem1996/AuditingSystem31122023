using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.AuditProcess
{
    public class Activity:Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public int? FunctionId { get; set; }
        public string? Source { get; set; }
        public virtual Function? Function { get; set; }
        public virtual IEnumerable<Objective>? Objectives { get; set; }

  

    }
}
