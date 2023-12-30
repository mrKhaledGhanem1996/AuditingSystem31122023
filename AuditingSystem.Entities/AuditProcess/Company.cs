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
    public class Company : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public string? Address { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? Source { get; set; }
        public int? IndustryId { get; set; }
        public virtual Industry? Industry { get; set; }

        public virtual IEnumerable<User>? User { get; set; }
        public virtual IEnumerable<Department>? Departments { get; set; }

 

    }
}
