using AuditingSystem.Database;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.AuditProcess
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectivesController : ControllerBase
    {
        private readonly IBaseRepository<Objective, int> _objectiveRepository;
        private readonly AuditingSystemDbContext db;

        public ObjectivesController(IBaseRepository<Objective, int> objectiveRepository,
            AuditingSystemDbContext db)
        {
            _objectiveRepository = objectiveRepository ?? throw new ArgumentNullException(nameof(objectiveRepository));
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var objectives = await _objectiveRepository.ListAsync();
                return Ok(objectives);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error retrieving objectives: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _objectiveRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error deleting objective: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Objective objective)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRecord = db.Objectives
                    .Where(x => x.Source == "System")
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                    int increment = 5000;
                    if (existingRecord != null)
                    {
                        objective.Id = existingRecord.Id + 1;
                        objective.Source = existingRecord.Source;
                        await _objectiveRepository.CreateAsync(objective);
                        return NoContent();
                    }
                    else
                    {
                        objective.Id = increment;
                        objective.Source = "System";
                        await _objectiveRepository.CreateAsync(objective);
                        return NoContent();
                    }
                }

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error adding objective: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Objective updatedObjective)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingObjective = await _objectiveRepository.FindByAsync(id);

                    if (existingObjective == null)
                    {
                        return NotFound(new { error = $"Objective with ID {id} not found" });
                    }

                    // تحديث الخصائص
                    existingObjective.Name = updatedObjective.Name;
                    existingObjective.Description = updatedObjective.Description;
                    existingObjective.ActivityId = updatedObjective.ActivityId;

                    await _objectiveRepository.UpdateAsync(existingObjective);

                    return NoContent();
                }

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error updating objective: {ex.Message}" });
            }
        }
    }
}
