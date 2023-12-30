using AuditingSystem.Entities.AuditProcess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Entities.Setup
{
    public class User : Base
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The Title field is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "The Email field is required")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "The Contact No field is required")]
        public string? ContactNo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "The Role field is required")]
        public int? RoleId { get; set; }
        public virtual Role? Role { get; set; }

        [Required(ErrorMessage = "The Company field is required")]
        public int? CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        [Required(ErrorMessage = "The Department field is required")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

 
         
    }
}
