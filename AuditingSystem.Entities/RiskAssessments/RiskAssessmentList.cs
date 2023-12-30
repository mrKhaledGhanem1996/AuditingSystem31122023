using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.RiskAssessments
{
    public class RiskAssessmentList:Base
    {
        public int Id { get; set; }
        public int? RiskIdentificationId { get; set; }
        public virtual RiskIdentification? RiskIdentification { get; set; }

        public int? ControlId { get; set; }
        public virtual Control? Control { get; set; }

        public string? ResidualRiskRating { get; set; }

        public virtual IEnumerable<RiskAssessmentList>? RiskAssessments { get; set; }

         
    }
}
