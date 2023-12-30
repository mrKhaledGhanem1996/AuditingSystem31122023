using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskLikehoodsController : ControllerBase
    {
        private readonly IBaseRepository<RiskLikehood, int> _risklikehood;
        private readonly ILogger<RiskLikehoodsController> _logger;
        public RiskLikehoodsController(IBaseRepository<RiskLikehood, int> risklikehood, ILogger<RiskLikehoodsController> logger)
        {
            _risklikehood = risklikehood ?? throw new ArgumentNullException(nameof(risklikehood));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var conteff = await _risklikehood.ListAsync();
                return Ok(conteff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving risk likehood.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var riskLikehood = await _risklikehood.FindByAsync(id);
                if (riskLikehood == null)
                {
                    return NotFound();
                }

                await _risklikehood.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the risk likehood with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete risk likehood with ID: {id}." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RiskLikehood riskLikehood)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _risklikehood.CreateAsync(riskLikehood);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new risk likehood.");
                return StatusCode(500, new { error = "An error occurred while processing your request to add a new risk likehood." });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] RiskLikehood riskLikehood)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingriskimact = await _risklikehood.FindByAsync(id);
                    if (existingriskimact == null)
                    {
                        return NotFound();
                    }


                    existingriskimact.Rate = riskLikehood.Rate;
                   
                    await _risklikehood.UpdateAsync(existingriskimact);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the risk imapct with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit risk impact with ID: {id}." });
            }
        }
    }
}
