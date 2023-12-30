using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlFrequencysController : ControllerBase
    {
        private readonly IBaseRepository<ControlFrequency, int> _controlefrequncy;
        private readonly ILogger<ControlFrequencysController> _logger;
        public ControlFrequencysController(IBaseRepository<ControlFrequency, int> controlfrequency, ILogger<ControlFrequencysController> logger)
        {
            _controlefrequncy = controlfrequency ?? throw new ArgumentNullException(nameof(controlfrequency));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var contelfrq = await _controlefrequncy.ListAsync();
                return Ok(contelfrq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving control frequncy.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var conrolfredelete = await _controlefrequncy.FindByAsync(id);
                if (conrolfredelete == null)
                {
                    return NotFound();
                }

                await _controlefrequncy.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the controle frequncy with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete role with ID: {id}." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ControlFrequency control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _controlefrequncy.CreateAsync(control);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new _ontrolefrequncy.");
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
                    var existingefreq = await _controlefrequncy.FindByAsync(id);
                    if (existingefreq == null)
                    {
                        return NotFound();
                    }



                    existingefreq.BGColor = control.BGColor;
                    existingefreq.FontColor = control.FontColor;
                    await _controlefrequncy.UpdateAsync(existingefreq);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the controlfrequncy with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit role with ID: {id}." });
            }
        }
    }




}
