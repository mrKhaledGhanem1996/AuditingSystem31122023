using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlTypesscsController : ControllerBase
    {
        private readonly IBaseRepository<ControlType, int> _controltype;
        private readonly ILogger<ControlTypesscsController> _logger;
        public ControlTypesscsController(IBaseRepository<ControlType, int> controltype)
        {
            _controltype = controltype;
        }
        public ControlTypesscsController(IBaseRepository<ControlType, int> controltype, ILogger<ControlTypesscsController> logger)
        {
            _controltype = controltype ?? throw new ArgumentNullException(nameof(controltype));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var controlTypes = await _controltype.ListAsync();
                return Ok(controlTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control type.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var controlDelete = await _controltype.FindByAsync(id);
                if (controlDelete == null)
                {
                    return NotFound();
                }

                await _controltype.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the control type with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete control tyoe with ID: {id}." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ControlType control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _controltype.CreateAsync(control);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new Controls type.");
                return StatusCode(500, new { error = "An error occurred while processing your request to add a new role." });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ControlType control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingtype = await _controltype.FindByAsync(id);
                    if (existingtype == null)
                    {
                        return NotFound();
                    }



                    existingtype.BGColor = control.BGColor;
                    existingtype.FontColor = control.FontColor;
                    await _controltype.UpdateAsync(control);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the controltyoe with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit controltype with ID: {id}." });
            }
        }
    }
}
