using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AuditingSystem.Entities
{
    public abstract class Base
    {

         
        public string? Name { get; set; }

        public string? Code { get; set; }


        [AllowHtml]
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; } = "Admin";
        public DateTime? CreationDate { get; set; }
        public string? UpdatedBy { get; set; } = "Admin";
        public DateTime? UpdatedDate { get; set; }
 
    }
}
