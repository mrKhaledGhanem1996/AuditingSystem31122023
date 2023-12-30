using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.api.Lockups
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskCategoryscsController : ControllerBase
    {
        private readonly IBaseRepository<RiskCategory, int> _riskcategory;
        private readonly ILogger<RiskCategoryscsController> _logger;
        public RiskCategoryscsController(IBaseRepository<RiskCategory, int> riskcategory, ILogger<RiskCategoryscsController> logger)
        {
            _riskcategory = riskcategory ?? throw new ArgumentNullException(nameof(riskcategory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var conteff = await _riskcategory.ListAsync();
                return Ok(conteff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving risk categoy.");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var roleToDelete = await _riskcategory.FindByAsync(id);
                if (roleToDelete == null)
                {
                    return NotFound();
                }

                await _riskcategory.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the riskcategory with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to delete role with ID: {id}." });
            }

        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RiskCategory risk)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _riskcategory.CreateAsync(risk);
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
        public async Task<IActionResult> Edit(int id, [FromBody] RiskCategory risk)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingerisk = await _riskcategory.FindByAsync(id);
                    if (existingerisk == null)
                    {
                        return NotFound();
                    }

;
                    existingerisk.BGColor = risk.BGColor;
                    existingerisk.FontColor = risk.FontColor;
                    await _riskcategory.UpdateAsync(existingerisk);

                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the risk category with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while processing your request to edit role with ID: {id}." });
            }
        }




    }
}
