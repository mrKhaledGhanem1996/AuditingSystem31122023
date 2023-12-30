using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Setup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace AuditingSystem.Web.Common
{
    public class AppSession
    {
        private readonly ISession _session;

        public AppSession(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext?.Session ?? throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext), "HttpContext is null.");
        }

        public SelectList Companies
        {
            get => Get<SelectList>("Companies") ?? new SelectList(new List<Company>(), "Id", "Name");
            set => Set("Companies", value);
        }

        public SelectList Departments
        {
            get => Get<SelectList>("Departments") ?? new SelectList(new List<Department>(), "Id", "Name");
            set => Set("Departments", value);
        }

        public SelectList Roles
        {
            get => Get<SelectList>("Roles") ?? new SelectList(new List<Role>(), "Id", "Name");
            set => Set("Roles", value);
        }

        private T? Get<T>(string key)
        {
            var value = _session.GetString(key);
            return value != null ? JsonSerializer.Deserialize<T>(value) : default;
        }

        private void Set<T>(string key, T value)
        {
            _session.SetString(key, JsonSerializer.Serialize(value));
        }
    }
}
