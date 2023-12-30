using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskImpactsController : ControllerBase
    {
        private readonly IBaseRepository<RiskImpact, int> _riskimpact;
        private readonly ILogger<RiskImpactsController> _logger;
        public RiskImpactsController(IBaseRepository<RiskImpact, int> riskimapct, ILogger<RiskImpactsController> logger)
        {
            _riskimpact = riskimapct ?? throw new ArgumentNullException(nameof(riskimapct));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var riskimpact = await _riskimpact.ListAsync();
                return Ok(riskimpact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving risk impact.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var riskimpact = await _riskimpact.FindByAsync(id);
                if (riskimpact == null)
                {
                    return NotFound();
                }

                await _riskimpact.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the risk impact with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete impact with ID: {id}." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RiskImpact riskImpact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _riskimpact.CreateAsync(riskImpact);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new risk impact.");
                return StatusCode(500, new { error = "An error occurred while processing your request to add a new risk impact." });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] RiskImpact riskImpact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingeriskimpact = await _riskimpact.FindByAsync(id);
                    if (existingeriskimpact == null)
                    {
                        return NotFound();
                    }


                    existingeriskimpact.Rate = riskImpact.Rate;
                   
                    await _riskimpact.UpdateAsync(riskImpact);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the risk impact with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit risk impact with ID: {id}." });
            }
        }
    }
}
