using AuditingSystem.Database;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.AuditProcess
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IBaseRepository<Activity, int> _activityRepository;
        private readonly AuditingSystemDbContext db;

        public ActivitiesController(IBaseRepository<Activity, int> activityRepository, AuditingSystemDbContext db)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var activities = await _activityRepository.ListAsync();
            return Ok(activities);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _activityRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error deleting activity: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Activity activity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRecord = db.Activities
                    .Where(x => x.Source == "System")
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                    int increment = 5000;
                    if (existingRecord != null)
                    {
                        activity.Id = existingRecord.Id + 1;
                        activity.Source = existingRecord.Source;
                        await _activityRepository.CreateAsync(activity);
                        return NoContent();
                    }
                    else
                    {
                        activity.Id = increment;
                        activity.Source = "System";
                        await _activityRepository.CreateAsync(activity);
                        return NoContent();
                    }
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error adding activity: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Activity updatedActivity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingActivity = await _activityRepository.FindByAsync(id);

                    if (existingActivity == null)
                    {
                        return NotFound();
                    }

                    // تحديث الخصائص
                    existingActivity.Name = updatedActivity.Name;
                    existingActivity.Description = updatedActivity.Description;
                    existingActivity.FunctionId = updatedActivity.FunctionId;

                    await _activityRepository.UpdateAsync(existingActivity);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error updating activity: {ex.Message}" });
            }
        }
    }
}
