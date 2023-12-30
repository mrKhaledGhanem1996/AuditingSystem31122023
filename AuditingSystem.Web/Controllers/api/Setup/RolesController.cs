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
    public class RolesController : ControllerBase
    {
        private readonly IBaseRepository<Role, int> _roleRepository;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IBaseRepository<Role, int> roleRepository, ILogger<RolesController> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _roleRepository.ListAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var roleToDelete = await _roleRepository.FindByAsync(id);
                if (roleToDelete == null)
                {
                    return NotFound();
                }

                await _roleRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the role with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete role with ID: {id}." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Role role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _roleRepository.CreateAsync(role);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new role.");
                return StatusCode(500, new { error = "An error occurred while processing your request to add a new role." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Role updatedRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingRole = await _roleRepository.FindByAsync(id);
                    if (existingRole == null)
                    {
                        return NotFound();
                    }

                    // Update role properties
                    existingRole.Name = updatedRole.Name;
                    existingRole.Description = updatedRole.Description;

                    await _roleRepository.UpdateAsync(existingRole);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the role with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit role with ID: {id}." });
            }
        }
    }
}
