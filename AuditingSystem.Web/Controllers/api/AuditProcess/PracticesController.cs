using AuditingSystem.Database;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.AuditProcess
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracticesController : ControllerBase
    {
        private readonly IBaseRepository<Practice, int> _practiceRepository;
        private readonly AuditingSystemDbContext db;

        public PracticesController(IBaseRepository<Practice, int> practiceRepository, AuditingSystemDbContext db)
        {
            _practiceRepository = practiceRepository ?? throw new ArgumentNullException(nameof(practiceRepository));
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var practices = await _practiceRepository.ListAsync();
                return Ok(practices);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error retrieving practices: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _practiceRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error deleting practice: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Practice practice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRecord = db.Practices
                      .Where(x => x.Source == "System")
                      .OrderByDescending(x => x.Id)
                      .FirstOrDefault();

                    int increment = 5000;
                    if (existingRecord != null)
                    {
                        practice.Id = existingRecord.Id + 1;
                        practice.Source = existingRecord.Source;
                        await _practiceRepository.CreateAsync(practice);
                        return NoContent();
                    }
                    else
                    {
                        practice.Id = increment;
                        practice.Source = "System";
                        await _practiceRepository.CreateAsync(practice);
                        return NoContent();
                    }
                }

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error adding practice: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Practice updatedPractice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingPractice = await _practiceRepository.FindByAsync(id);

                    if (existingPractice == null)
                    {
                        return NotFound(new { error = $"Practice with ID {id} not found" });
                    }

                    // تحديث الخصائص
                    existingPractice.Name = updatedPractice.Name;
                    existingPractice.Description = updatedPractice.Description;
                    existingPractice.TaskId = updatedPractice.TaskId;

                    await _practiceRepository.UpdateAsync(existingPractice);

                    return NoContent();
                }

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error updating practice: {ex.Message}" });
            }
        }
    }
}
