using AuditingSystem.Database;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.AuditProcess
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IBaseRepository<Company, int> _companyRepository;
        private readonly AuditingSystemDbContext db;
        public CompaniesController(IBaseRepository<Company, int> companyRepository,
            AuditingSystemDbContext db)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyRepository.ListAsync();
            return Ok(companies);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _companyRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error deleting company: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRecord = db.Companies
                      .Where(x => x.Source == "System")
                      .OrderByDescending(x => x.Id)
                      .FirstOrDefault();

                    int increment = 5000;
                    if (existingRecord != null)
                    {
                        company.Id = existingRecord.Id + 1;
                        company.Source = existingRecord.Source;
                        await _companyRepository.CreateAsync(company);
                        return NoContent();
                    }
                    else
                    {
                        company.Id = increment;
                        company.Source = "System";
                        await _companyRepository.CreateAsync(company);
                        return NoContent();
                    }
                    
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error adding company: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Company updatedCompany)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCompany = await _companyRepository.FindByAsync(id);

                    if (existingCompany == null)
                    {
                        return NotFound();
                    }

                    // تحديث الخصائص
                    existingCompany.Name = updatedCompany.Name;
                    existingCompany.Description = updatedCompany.Description;
                    existingCompany.Address = updatedCompany.Address;
                    existingCompany.ContactNo = updatedCompany.ContactNo;
                    existingCompany.Email = updatedCompany.Email;
                    existingCompany.IndustryId = updatedCompany.IndustryId;

                    await _companyRepository.UpdateAsync(existingCompany);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error updating company: {ex.Message}" });
            }
        }
    }
}
