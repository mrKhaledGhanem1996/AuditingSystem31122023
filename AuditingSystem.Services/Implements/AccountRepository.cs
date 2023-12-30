    using AuditingSystem.Database;
    using AuditingSystem.Entities.Setup;
    using AuditingSystem.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace AuditingSystem.Services.Implements
    {
    public class AccountRepository : IAccountRepository
    {
        private readonly AuditingSystemDbContext db;

        public AccountRepository(AuditingSystemDbContext db)
        {
            this.db = db;
        }

        public (int? userId, string userName) Login(User user)
        {
            var loggedInUser = db.Users
                .Where(u => u.Email.ToLower() == user.Email.ToLower() && u.Password == user.Password)
                .FirstOrDefault();

            if (loggedInUser != null)
            {
                return (loggedInUser.Id, loggedInUser.Name);
            }

            return (null, null);
        }
    }

}
