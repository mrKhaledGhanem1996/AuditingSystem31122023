using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Setup;

namespace AuditingSystem.Web.ViewModels
{
    public class UserListVM
    {
        public User User { get; set; }
        public Company Company { get; set; }
        public Department Department { get; set; }
        public Role Role { get; set; }
    }
}
