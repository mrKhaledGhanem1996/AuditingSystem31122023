using AuditingSystem.Entities.Lockups;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using AuditingSystem.Web.Controllers.api.Setup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlEffectivenesscsController : ControllerBase
    {
        private readonly IBaseRepository<ControlEffectiveness, int> _controleeffevt;
        private readonly ILogger<ControlEffectivenesscsController> _logger;
        public ControlEffectivenesscsController(IBaseRepository<ControlEffectiveness, int> controleffect, ILogger<ControlEffectivenesscsController> logger)
        {
            _controleeffevt = controleffect ?? throw new ArgumentNullException(nameof(controleffect));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var conteff = await _controleeffevt.ListAsync();
                return Ok(conteff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving controleffect.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var control = await _controleeffevt.FindByAsync(id);
                if (control == null)
                {
                    return NotFound();
                }

                await _controleeffevt.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the controleffect with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete role with ID: {id}." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ControlEffectiveness control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _controleeffevt.CreateAsync(control);
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
        public async Task<IActionResult> Edit(int id, [FromBody] ControlEffectiveness control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingeffect = await _controleeffevt.FindByAsync(id);
                    if (existingeffect == null)
                    {
                        return NotFound();
                    }

                    existingeffect.Name = control.Name;
                    existingeffect.Description = control.Description;
                    existingeffect.Rate = control.Rate;
                    existingeffect.BGColor = control.BGColor;
                    existingeffect.FontColor = control.FontColor;
                    await _controleeffevt.UpdateAsync(existingeffect);

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
