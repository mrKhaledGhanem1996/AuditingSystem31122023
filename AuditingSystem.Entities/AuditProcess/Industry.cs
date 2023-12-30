using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.AuditProcess
{
    public class Industry: Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        public int? ParentIndustryId { get; set; }
        public string? Source { get; set; }
        public virtual Industry? ParentIndustry { get; set; }
        public virtual IEnumerable<Company>? Companies { get; set; }

        
    }
}
