using AuditingSystem.Database;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.Setup
{
    [Route("api/[controller]")]
    [ApiController]
    public class YearsController : ControllerBase
    {
        private readonly IBaseRepository<Year, int> _yearRepository;
        private readonly ILogger<YearsController> _logger;

        public YearsController(IBaseRepository<Year, int> yearRepository, ILogger<YearsController> logger)
        {
            _yearRepository = yearRepository ?? throw new ArgumentNullException(nameof(yearRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var years = await _yearRepository.ListAsync();
                return Ok(years);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving years.");
                return Problem("An error occurred while processing your request.", statusCode: 500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _yearRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the year with ID: {id}.");
                return Problem($"An error occurred while processing your request to delete year with ID: {id}.", statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Year year)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _yearRepository.CreateAsync(year);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new year.");
                return Problem("An error occurred while processing your request to add a new year.", statusCode: 500);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(int id, [FromBody] Year updatedYear)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingYear = await _yearRepository.FindByAsync(id);
                    if (existingYear == null)
                        return NotFound();

                    // Update the existing year with the new values
                    existingYear.Name = updatedYear.Name;
                    existingYear.CompanyId = updatedYear.CompanyId;
                    existingYear.DepartmentId = updatedYear.DepartmentId;
                    existingYear.Description = updatedYear.Description;

                    await _yearRepository.UpdateAsync(existingYear);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the year with ID: {id}.");
                return Problem($"An error occurred while processing your request to edit year with ID: {id}.", statusCode: 500);
            }
        }
    }
}
