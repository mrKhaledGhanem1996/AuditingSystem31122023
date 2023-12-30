using AuditingSystem.Database;
using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuditingSystem.Web.Controllers.api.RiskAssessment
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlsController : ControllerBase
    {
        private readonly IBaseRepository<Control, int> _controlRepository;
        private readonly IBaseRepository<RiskAssessmentList, int> _riskAssessmentListRepository;
        private readonly IBaseRepository<RiskIdentification, int> _riskIdentificationRepository;

        public ControlsController(
            IBaseRepository<Control, int> controlRepository,
            IBaseRepository<RiskAssessmentList, int> riskAssessmentListRepository,
            IBaseRepository<RiskIdentification, int> riskIdentificationRepository)
        {
            _controlRepository = controlRepository ?? throw new ArgumentNullException(nameof(controlRepository));
            _riskAssessmentListRepository = riskAssessmentListRepository ?? throw new ArgumentNullException(nameof(riskAssessmentListRepository));
            _riskIdentificationRepository = riskIdentificationRepository ?? throw new ArgumentNullException(nameof(riskIdentificationRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var controls = await _controlRepository.ListAsync();
                return Ok(controls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error retrieving controls: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _controlRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error deleting control: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Control control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _controlRepository.CreateAsync(control);

                    var controlEntity = await _controlRepository.FindByAsync(control.Id);
                    var inherentRisk = await _riskIdentificationRepository.FindByAsync((int)control.RiskIdentificationId);

                    var controlRate = controlEntity.ControlEffectivenessRating;
                    var inherentRate = inherentRisk?.InherentRiskRating ?? 0;

                    var riskAssessment = new RiskAssessmentList()
                    {
                        ControlId = controlEntity.Id,
                        RiskIdentificationId = controlEntity.RiskIdentificationId,
                        Description = "test",
                        Name = "test",
                        CreatedBy = "Admin",
                        UpdatedBy = "Admin",
                        CreationDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };

                    if (controlRate >= 4 && inherentRate >= 6)
                        riskAssessment.ResidualRiskRating = "Active Management";
                    else if (controlRate <= 4 && inherentRate >= 6)
                        riskAssessment.ResidualRiskRating = "Continuous Review";
                    else if (controlRate >= 4 && inherentRate <= 6)
                        riskAssessment.ResidualRiskRating = "Periodic Monitoring";
                    else if (controlRate <= 4 && inherentRate <= 6)
                        riskAssessment.ResidualRiskRating = "No major concern";

                    await _riskAssessmentListRepository.CreateAsync(riskAssessment);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal Server Error: {ex.Message}" });
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Control control)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingControl = await _controlRepository.FindByAsync(id);
                    if (existingControl == null)
                        return NotFound();

                    existingControl.Code = control.Code;
                    existingControl.Name = control.Name;
                    existingControl.Description = control.Description;
                    existingControl.ControlTypeId = control.ControlTypeId;
                    existingControl.ControlProcessId = control.ControlProcessId;
                    existingControl.ControlFrequencyId = control.ControlFrequencyId;
                    existingControl.ControlEffectivenessId = control.ControlEffectivenessId;
                    existingControl.ControlEffectivenessRating = control.ControlEffectivenessRating;
                    existingControl.RiskIdentificationId = control.RiskIdentificationId;

                    await _controlRepository.UpdateAsync(existingControl);
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error updating control: {ex.Message}" });
            }

            return BadRequest();
        }
    }
}
