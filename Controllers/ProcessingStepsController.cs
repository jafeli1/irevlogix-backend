using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProcessingStepsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProcessingStepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? lotId = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.ProcessingSteps
                .Include(x => x.ProcessingLot)
                .Include(x => x.ResponsibleUser)
                .AsQueryable();

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ProcessingLot.ClientId == clientId);
            }

            if (lotId.HasValue)
            {
                query = query.Where(x => x.ProcessingLotId == lotId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.StepOrder)
                .ThenByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.ProcessingLotId,
                    ProcessingLot = x.ProcessingLot == null ? null : new
                    {
                        x.ProcessingLot.Id,
                        x.ProcessingLot.LotNumber
                    },
                    x.StepName,
                    x.Description,
                    x.StepOrder,
                    x.StartTime,
                    x.EndTime,
                    x.Status,
                    x.ResponsibleUserId,
                    ResponsibleUser = x.ResponsibleUser == null ? null : new
                    {
                        x.ResponsibleUser.Id,
                        x.ResponsibleUser.FirstName,
                        x.ResponsibleUser.LastName
                    },
                    x.LaborHours,
                    x.MachineHours,
                    x.EnergyConsumption,
                    x.ProcessingCostPerUnit,
                    x.TotalStepCost,
                    x.Notes,
                    x.Equipment,
                    x.InputWeight,
                    x.OutputWeight,
                    x.WasteWeight
                })
                .ToListAsync();

            return Ok(new { items, totalCount, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.ProcessingSteps
                .Include(x => x.ProcessingLot)
                .Include(x => x.ResponsibleUser)
                .Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ProcessingLot.ClientId == clientId);
            }

            var item = await query.FirstOrDefaultAsync();

            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.ProcessingLotId,
                ProcessingLot = item.ProcessingLot == null ? null : new
                {
                    item.ProcessingLot.Id,
                    item.ProcessingLot.LotNumber
                },
                item.StepName,
                item.Description,
                item.StepOrder,
                item.StartTime,
                item.EndTime,
                item.Status,
                item.ResponsibleUserId,
                ResponsibleUser = item.ResponsibleUser == null ? null : new
                {
                    item.ResponsibleUser.Id,
                    item.ResponsibleUser.FirstName,
                    item.ResponsibleUser.LastName
                },
                item.LaborHours,
                item.MachineHours,
                item.EnergyConsumption,
                item.ProcessingCostPerUnit,
                item.TotalStepCost,
                item.Notes,
                item.Equipment,
                item.InputWeight,
                item.OutputWeight,
                item.WasteWeight
            });
        }

        public class CreateProcessingStepDto
        {
            public int ProcessingLotId { get; set; }
            public string StepName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int StepOrder { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string Status { get; set; } = "Pending";
            public int? ResponsibleUserId { get; set; }
            public decimal? LaborHours { get; set; }
            public decimal? MachineHours { get; set; }
            public decimal? EnergyConsumption { get; set; }
            public decimal? ProcessingCostPerUnit { get; set; }
            public decimal? TotalStepCost { get; set; }
            public string? Notes { get; set; }
            public string? Equipment { get; set; }
            public decimal? InputWeight { get; set; }
            public decimal? OutputWeight { get; set; }
            public decimal? WasteWeight { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessingStepDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var lotQuery = _context.ProcessingLots.Where(x => x.Id == dto.ProcessingLotId);
            if (!IsAdministrator())
            {
                lotQuery = lotQuery.Where(x => x.ClientId == clientId);
            }
            var lot = await lotQuery.FirstOrDefaultAsync();
            if (lot == null) return BadRequest("Invalid processing lot");

            var entity = new ProcessingStep
            {
                ProcessingLotId = dto.ProcessingLotId,
                StepName = dto.StepName,
                Description = dto.Description,
                StepOrder = dto.StepOrder,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = dto.Status,
                ResponsibleUserId = dto.ResponsibleUserId,
                LaborHours = dto.LaborHours,
                MachineHours = dto.MachineHours,
                EnergyConsumption = dto.EnergyConsumption,
                ProcessingCostPerUnit = dto.ProcessingCostPerUnit,
                TotalStepCost = dto.TotalStepCost,
                Notes = dto.Notes,
                Equipment = dto.Equipment,
                InputWeight = dto.InputWeight,
                OutputWeight = dto.OutputWeight,
                WasteWeight = dto.WasteWeight,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ProcessingSteps.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateProcessingStepDto
        {
            public string? StepName { get; set; }
            public string? Description { get; set; }
            public int? StepOrder { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string? Status { get; set; }
            public int? ResponsibleUserId { get; set; }
            public decimal? LaborHours { get; set; }
            public decimal? MachineHours { get; set; }
            public decimal? EnergyConsumption { get; set; }
            public decimal? ProcessingCostPerUnit { get; set; }
            public decimal? TotalStepCost { get; set; }
            public string? Notes { get; set; }
            public string? Equipment { get; set; }
            public decimal? InputWeight { get; set; }
            public decimal? OutputWeight { get; set; }
            public decimal? WasteWeight { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateProcessingStepDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.ProcessingSteps
                .Include(x => x.ProcessingLot)
                .Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ProcessingLot.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.StepName)) entity.StepName = dto.StepName;
            if (!string.IsNullOrWhiteSpace(dto.Description)) entity.Description = dto.Description;
            if (dto.StepOrder.HasValue) entity.StepOrder = dto.StepOrder.Value;
            if (dto.StartTime.HasValue) entity.StartTime = dto.StartTime;
            if (dto.EndTime.HasValue) entity.EndTime = dto.EndTime;
            if (!string.IsNullOrWhiteSpace(dto.Status)) entity.Status = dto.Status;
            if (dto.ResponsibleUserId.HasValue) entity.ResponsibleUserId = dto.ResponsibleUserId;
            if (dto.LaborHours.HasValue) entity.LaborHours = dto.LaborHours;
            if (dto.MachineHours.HasValue) entity.MachineHours = dto.MachineHours;
            if (dto.EnergyConsumption.HasValue) entity.EnergyConsumption = dto.EnergyConsumption;
            if (dto.ProcessingCostPerUnit.HasValue) entity.ProcessingCostPerUnit = dto.ProcessingCostPerUnit;
            if (dto.TotalStepCost.HasValue) entity.TotalStepCost = dto.TotalStepCost;
            if (!string.IsNullOrWhiteSpace(dto.Notes)) entity.Notes = dto.Notes;
            if (!string.IsNullOrWhiteSpace(dto.Equipment)) entity.Equipment = dto.Equipment;
            if (dto.InputWeight.HasValue) entity.InputWeight = dto.InputWeight;
            if (dto.OutputWeight.HasValue) entity.OutputWeight = dto.OutputWeight;
            if (dto.WasteWeight.HasValue) entity.WasteWeight = dto.WasteWeight;
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId) && !IsAdministrator()) return Unauthorized();

            var query = _context.ProcessingSteps
                .Include(x => x.ProcessingLot)
                .Where(x => x.Id == id);

            if (!IsAdministrator())
            {
                query = query.Where(x => x.ProcessingLot.ClientId == clientId);
            }

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            _context.ProcessingSteps.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
