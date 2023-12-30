using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.RiskAssessments
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskAssessmentsController : ControllerBase
    {
        private readonly IBaseRepository<RiskAssessmentList, int> _riskAssessmentListRepository;

        public RiskAssessmentsController(IBaseRepository<RiskAssessmentList, int> riskAssessmentListRepository)
        {
            _riskAssessmentListRepository = riskAssessmentListRepository ?? throw new ArgumentNullException(nameof(riskAssessmentListRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var riskAssessmentList = await _riskAssessmentListRepository.ListAsync();
                return Ok(riskAssessmentList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error retrieving risk assessments: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _riskAssessmentListRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error deleting risk assessment: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(RiskAssessmentList riskAssessmentList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _riskAssessmentListRepository.CreateAsync(riskAssessmentList);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error adding risk assessment: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] RiskAssessmentList riskAssessmentList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var assessment = await _riskAssessmentListRepository.FindByAsync(id);
                    if (assessment == null)
                    {
                        return NotFound();
                    }

                    assessment.Name = riskAssessmentList.Name;
                    assessment.Description = riskAssessmentList.Description;
                    assessment.RiskIdentificationId = riskAssessmentList.RiskIdentificationId;
                    assessment.ControlId = riskAssessmentList.ControlId;
                    assessment.ResidualRiskRating = riskAssessmentList.ResidualRiskRating;

                    await _riskAssessmentListRepository.UpdateAsync(assessment);
                    return NoContent();
                }

                return BadRequest("Invalid ModelState");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error updating risk assessment: {ex.Message}" });
            }
        }
    }
}
