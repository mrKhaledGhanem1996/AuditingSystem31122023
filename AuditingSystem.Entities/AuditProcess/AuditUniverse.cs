using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AuditingSystem.Entities.AuditProcess
{
    public class AuditUniverse:Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public int? IndustryId { get; set; }
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? FunctionId { get; set; }
        public int? ActivityId { get; set; }
        public int? ObjectiveId { get; set; }
        public int? TaskId { get; set; }
        public int? PracticeId { get; set; }

        public virtual Industry? Industry { get; set; }
        public virtual Company? Company { get; set; }
        public virtual Department? Department { get; set; }
        public virtual Function? Function { get; set; }
        public virtual Activity? Activity { get; set; }
        public virtual Objective? Objective { get; set; }
        public virtual Tasks? Task { get; set; }
        public virtual Practice? Practice { get; set; }

 
    }
}
