using AuditingSystem.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditingSystem.Services.Interfaces
{
    public interface IAccountRepository
    {
        public (int? userId, string userName) Login(User user);
    }
}
