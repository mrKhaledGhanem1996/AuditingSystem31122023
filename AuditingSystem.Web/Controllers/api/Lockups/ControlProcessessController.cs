using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using AuditingSystem.Web.Controllers.Lockups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlProcessessController : ControllerBase
    {
        private readonly IBaseRepository<ControlProcess, int> _controlprocess;
        private readonly ILogger<ControlProcessessController> _logger;
        public ControlProcessessController(IBaseRepository<ControlProcess, int> controlprocess)
        {
            _controlprocess = controlprocess;
        }
        public ControlProcessessController(IBaseRepository<ControlProcess, int> controlprocess, ILogger<ControlProcessessController> logger)
        {
           _controlprocess  = controlprocess ?? throw new ArgumentNullException(nameof(controlprocess));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var conteff = await _controlprocess.ListAsync();
                return Ok(conteff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control process.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var controlDelete = await _controlprocess.FindByAsync(id);
                if (controlDelete == null)
                {
                    return NotFound();
                }

                await _controlprocess.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the _controlprocess with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete role with ID: {id}." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ControlProcess control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _controlprocess.CreateAsync(control);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new ControlProcess.");
                return StatusCode(500, new { error = "An error occurred while processing your request to add a new role." });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ControlProcess control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingprocess = await _controlprocess.FindByAsync(id);
                    if (existingprocess == null)
                    {
                        return NotFound();
                    }



                    existingprocess.BGColor = control.BGColor;
                    existingprocess.FontColor = control.FontColor;
                    await _controlprocess.UpdateAsync(control);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the controlprocess with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit controlprocess with ID: {id}." });
            }
        }
    }
}
