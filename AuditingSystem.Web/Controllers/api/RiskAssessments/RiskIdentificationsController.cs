using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.Api.RiskAssessment
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskIdentificationsController : ControllerBase
    {
        private readonly IBaseRepository<RiskIdentification, int> _riskIdentificationRepository;
        private readonly ILogger<RiskIdentificationsController> _logger;
        private readonly IMemoryCache _cache;

        public RiskIdentificationsController(
            IBaseRepository<RiskIdentification, int> riskIdentificationRepository,
            ILogger<RiskIdentificationsController> logger,
            IMemoryCache cache)
        {
            _riskIdentificationRepository = riskIdentificationRepository ?? throw new ArgumentNullException(nameof(riskIdentificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cacheKey = "RiskIdentifications";
                if (_cache.TryGetValue(cacheKey, out var cachedData))
                {
                    return Ok(cachedData);
                }

                var riskIdentifications = await _riskIdentificationRepository.ListAsync();
                _cache.Set(cacheKey, riskIdentifications, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache for 10 minutes
                });

                return Ok(riskIdentifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving risk identifications.");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _riskIdentificationRepository.DeleteAsync(id);

                // Remove cached data
                _cache.Remove("RiskIdentifications");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the risk identification with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while deleting the risk identification with ID: {id}." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RiskIdentification riskIdentification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _riskIdentificationRepository.CreateAsync(riskIdentification);

                    // Remove cached data
                    _cache.Remove("RiskIdentifications");

                    return CreatedAtAction(nameof(GetById), new { id = riskIdentification.Id }, riskIdentification);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a risk identification.");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] RiskIdentification riskIdentification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingIdentification = await _riskIdentificationRepository.FindByAsync(id);
                    if (existingIdentification == null)
                    {
                        return NotFound();
                    }

                    // Update the existing identification with the new values
                    UpdateRiskIdentification(existingIdentification, riskIdentification);

                    await _riskIdentificationRepository.UpdateAsync(existingIdentification);

                    // Remove cached data
                    _cache.Remove("RiskIdentifications");

                    return NoContent();
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while editing the risk identification with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while editing the risk identification with ID: {id}." });
            }
        }

        private void UpdateRiskIdentification(RiskIdentification existing, RiskIdentification updated)
        {
            existing.Code = updated.Code;
            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.RiskCategoryId = updated.RiskCategoryId;
            existing.RiskImpactId = updated.RiskImpactId;
            existing.RiskLikelihoodId = updated.RiskLikelihoodId;
            existing.RiskRatingRationalization = updated.RiskRatingRationalization;
            existing.InherentRiskRating = updated.InherentRiskRating;
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cacheKey = $"RiskIdentification_{id}";
                if (_cache.TryGetValue(cacheKey, out var cachedData))
                {
                    return Ok(cachedData);
                }

                var riskIdentification = await _riskIdentificationRepository.FindByAsync(id);
                if (riskIdentification == null)
                {
                    return NotFound();
                }

                _cache.Set(cacheKey, riskIdentification, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache for 10 minutes
                });

                return Ok(riskIdentification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the risk identification with ID: {id}.");
                return StatusCode(500, new { error = $"An error occurred while retrieving the risk identification with ID: {id}." });
            }
        }
    }
}
