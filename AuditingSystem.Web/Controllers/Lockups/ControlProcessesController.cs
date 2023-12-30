using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.Lockups
{
    public class ControlProcessesController : Controller
    {
        private readonly IBaseRepository<ControlProcess, int> _controleprocess;
        private readonly ILogger<ControlProcessesController> _logger;
        public ControlProcessesController(IBaseRepository<ControlProcess, int> controlprocess)
        {
            _controleprocess = controlprocess;
        }
     

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var controls = await _controleprocess.ListAsync(
                       u => u.IsDeleted == false,
                       q => q.OrderBy(u => u.Id));

                return View(controls);
            }
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Add()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _controleprocess.FindByAsync(id));
        }
    }

}



