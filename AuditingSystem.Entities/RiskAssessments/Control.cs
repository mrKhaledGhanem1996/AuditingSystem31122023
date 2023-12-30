using AuditingSystem.Entities.Lockups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.RiskAssessments
{
    public class Control:Base
    {
        public int Id { get; set; }
        public int? ControlTypeId { get; set; }
        public virtual ControlType? ControlType{ get; set; }

        public int? ControlProcessId { get; set; }
        public virtual ControlProcess? ControlProcess { get; set; }

        public int ControlFrequencyId { get; set; }
        public virtual ControlFrequency? ControlFrequency { get; set; }

        public int? ControlEffectivenessId { get; set; }
        public virtual ControlEffectiveness? ControlEffectiveness { get; set; }

        public  int? RiskIdentificationId { get; set; }
        public virtual RiskIdentification? RiskIdentification { get; set; }
       
        public int? ControlEffectivenessRating { get; set; }

       
    }
}
