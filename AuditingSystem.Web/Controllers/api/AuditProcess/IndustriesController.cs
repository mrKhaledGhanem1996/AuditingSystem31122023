using AuditingSystem.Database;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.AuditProcess
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustriesController : ControllerBase
    {
        private readonly IBaseRepository<Industry, int> _industryRepository;
        private readonly AuditingSystemDbContext db;

        public IndustriesController(IBaseRepository<Industry, int> industryRepository, 
            AuditingSystemDbContext db)
        {
            _industryRepository = industryRepository ?? throw new ArgumentNullException(nameof(industryRepository));
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var industries = await _industryRepository.ListAsync();
                return Ok(industries);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error retrieving industries: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _industryRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error deleting industry: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Industry industry)
        {
            try
            {
                var existingRecord = db.Industries
                  .Where(x => x.Source == "System")
                  .OrderByDescending(x => x.Id)
                  .FirstOrDefault();

                int increment = 5000;
                if (existingRecord != null)
                {
                    industry.Id = existingRecord.Id + 1;
                    industry.Source = existingRecord.Source;
                    await _industryRepository.CreateAsync(industry);
                    return NoContent();
                }
                else
                {
                    industry.Id = increment;
                    industry.Source = "System";
                    await _industryRepository.CreateAsync(industry);
                    return NoContent();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error adding industry: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Industry updatedIndustry)
        {
            try
            {
                
                    var existingIndustry = await _industryRepository.FindByAsync(id);

                    if (existingIndustry == null)
                    {
                        return NotFound(new { error = $"Industry with ID {id} not found" });
                    }

                    existingIndustry.Name = updatedIndustry.Name;
                    existingIndustry.Description = updatedIndustry.Description;
                    existingIndustry.ParentIndustryId = updatedIndustry.ParentIndustryId;


                    await _industryRepository.UpdateAsync(existingIndustry);

                    return NoContent();
                

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error updating industry: {ex.Message}" });
            }
        }
    }
}
