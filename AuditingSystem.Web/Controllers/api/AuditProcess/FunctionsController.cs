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
    public class FunctionsController : ControllerBase
    {
        private readonly IBaseRepository<Function, int> _functionRepository;
        private readonly AuditingSystemDbContext db;

        public FunctionsController(IBaseRepository<Function, int> functionRepository,
            AuditingSystemDbContext db)
        {
            _functionRepository = functionRepository ?? throw new ArgumentNullException(nameof(functionRepository));
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var functions = await _functionRepository.ListAsync();
                return Ok(functions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error retrieving functions: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _functionRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error deleting function: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Function function)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRecord = db.Functions
                    .Where(x => x.Source == "System")
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                    int increment = 5000;
                    if (existingRecord != null)
                    {
                        function.Id = existingRecord.Id + 1;
                        function.Source = existingRecord.Source;
                        await _functionRepository.CreateAsync(function);
                        return NoContent();
                    }
                    else
                    {
                        function.Id = increment;
                        function.Source = "System";
                        await _functionRepository.CreateAsync(function);
                        return NoContent();
                    }
                }

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error adding function: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Function updatedFunction)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingFunction = await _functionRepository.FindByAsync(id);

                    if (existingFunction == null)
                    {
                        return NotFound(new { error = $"Function with ID {id} not found" });
                    }

                    // تحديث الخصائص
                    existingFunction.Name = updatedFunction.Name;
                    existingFunction.Description = updatedFunction.Description;
                    existingFunction.DepartmentId = updatedFunction.DepartmentId;

                    await _functionRepository.UpdateAsync(existingFunction);

                    return NoContent();
                }

                return BadRequest(new { error = "Invalid ModelState" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Error updating function: {ex.Message}" });
            }
        }
    }
}
