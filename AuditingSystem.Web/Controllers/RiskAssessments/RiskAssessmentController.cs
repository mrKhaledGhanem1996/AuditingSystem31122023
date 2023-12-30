using AuditingSystem.Database;
using AuditingSystem.Entities.Lockups;
using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using AuditingSystem.Services.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.RiskAssessments
{
    public class RiskAssessmentController : Controller
    {
            private readonly IBaseRepository<RiskAssessmentList, int> _riskAssessmentListRepository;
        private readonly AuditingSystemDbContext db;
            public RiskAssessmentController(
                IBaseRepository<RiskAssessmentList, int> riskAssessmentList, AuditingSystemDbContext db)
            {
                _riskAssessmentListRepository = riskAssessmentList;
                this.db = db;
            }
            public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
            {
            var list = from riskAssessment in db.RiskAssessmentsList
                       join riskIdentification in db.RiskIdentifications on riskAssessment.RiskIdentificationId equals riskIdentification.Id
                       join control in db.Controls on riskAssessment.ControlId equals control.Id
                       join riskCategory in db.RiskCategories on riskIdentification.RiskCategoryId equals riskCategory.Id
                       join riskImpact in db.RiskImpacts on riskIdentification.RiskImpactId equals riskImpact.Id
                       join riskLikelihood in db.RiskLikehoods on riskIdentification.RiskLikelihoodId equals riskLikelihood.Id
                       join controlType in db.ControlTypes on control.ControlTypeId equals controlType.Id
                       join controlProcess in db.ControlProcesses on control.ControlProcessId equals controlProcess.Id
                       join controlFrequency in db.ControlFrequencies on control.ControlFrequencyId equals controlFrequency.Id
                       join controlEffectiveness in db.ControlEffectivenesses on control.ControlEffectivenessId equals controlEffectiveness.Id
                       where riskIdentification.IsDeleted == false && control.IsDeleted == false
                           select new RiskAssessmentVM
                           {
                               RiskAssessmentList = riskAssessment,
                               RiskIdentification = riskIdentification,
                               RiskCategory = riskCategory,
                               RiskImpact = riskImpact,
                               RiskLikehood = riskLikelihood,
                               Control = control,
                               ControlType = controlType,
                               ControlProcess = controlProcess,
                               ControlFrequency = controlFrequency,
                               ControlEffectiveness = controlEffectiveness
                           };

                var model = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.TotalRow = list.Count();
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling(list.Count() / (double)pageSize);

                return View(model);            
             
            }
        
    }
}
