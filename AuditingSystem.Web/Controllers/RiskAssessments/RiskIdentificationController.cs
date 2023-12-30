using AuditingSystem.Entities.Lockups;
using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.RiskAssessments
{
    public class RiskIdentificationController : Controller
    {
        private readonly IBaseRepository<RiskIdentification, int> _riskIdentificaionRepository;
        private readonly IBaseRepository<RiskCategory, int> _riskCategoryRepository;
        private readonly IBaseRepository<RiskImpact, int> _riskImpactRepository;
        private readonly IBaseRepository<RiskLikehood, int> _likelihoodRepository;

        public RiskIdentificationController(
            IBaseRepository<RiskIdentification, int> riskIdentificaionRepository,
            IBaseRepository<RiskCategory, int> riskCategoryRepository,
            IBaseRepository<RiskImpact, int> riskImpactRepository,
            IBaseRepository<RiskLikehood, int> likelihoodRepository)
        {
            _riskIdentificaionRepository = riskIdentificaionRepository;
            _riskCategoryRepository = riskCategoryRepository;
            _riskImpactRepository = riskImpactRepository;
            _likelihoodRepository = likelihoodRepository;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var riskIdentifications = await _riskIdentificaionRepository.ListAsync(
                c => c.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
            c => c.RiskCategory, c => c.RiskImpact, c => c.RiskLikelihood);
            var model = riskIdentifications.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalRow = riskIdentifications.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(riskIdentifications.Count() / (double)pageSize);

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var riskCategory = await _riskCategoryRepository.ListAsync(
                u => u.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
                null);

            var riskImpact = await _riskImpactRepository.ListAsync(
                u => u.IsDeleted == false,
                q => q.OrderBy(u => u.Rate),
                null);

            var likelihood = await _likelihoodRepository.ListAsync(
                u => u.IsDeleted == false,
                q => q.OrderBy(u => u.Rate),
                null);

            ViewBag.RiskCategoryId = new SelectList(riskCategory, "Id", "Name");
            ViewBag.RiskImpactId = new SelectList(riskImpact.Select(r => new { Id = r.Id, Name = $"{r.Rate} - {r.Name}" }), "Id", "Name");
            ViewBag.RiskLikelihoodId = new SelectList(likelihood.Select(l => new { Id = l.Id, Name = $"{l.Rate} - {l.Name}" }), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWithLink(RiskIdentification riskIdentification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _riskIdentificaionRepository.CreateAsync(riskIdentification);

                    // Assuming the repository returns the saved entity with its ID
                    var savedRiskIdentification = await _riskIdentificaionRepository.FindByAsync(riskIdentification.Id);

                    return Ok(new { id = savedRiskIdentification.Id });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }

            return BadRequest("Invalid ModelState");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var riskIdentification = await _riskIdentificaionRepository.FindByAsync(id);

            var riskCategory = await _riskCategoryRepository.ListAsync(
                u => u.IsDeleted == false,
                q => q.OrderBy(u => u.Id),
                null);

            var riskImpact = await _riskImpactRepository.ListAsync(
                u => u.IsDeleted == false,
                q => q.OrderBy(u => u.Rate),
                null);

            var likelihood = await _likelihoodRepository.ListAsync(
                u => u.IsDeleted == false,
                q => q.OrderBy(u => u.Rate),
                null);

            ViewBag.RiskCategoryId = new SelectList(riskCategory, "Id", "Name", riskIdentification.RiskCategoryId);
            ViewBag.RiskImpactId = new SelectList(riskImpact.Select(r => new { Id = r.Id, Name = $"{r.Rate} - {r.Name}" }), "Id", "Name", riskIdentification.RiskImpactId);
            ViewBag.RiskLikelihoodId = new SelectList(likelihood.Select(l => new { Id = l.Id, Name = $"{l.Rate} - {l.Name}" }), "Id", "Name", riskIdentification.RiskLikelihoodId);

            return View(riskIdentification);
        }
    }
}
