using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;
using webapi.DTOs;  // Add this line
using Microsoft.Extensions.Logging;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultModelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly webapi.Services.EventHubService _eventHubService;
        private readonly ILogger<ResultModelsController> _logger;

        public ResultModelsController(AppDbContext context, webapi.Services.EventHubService eventHubService, ILogger<ResultModelsController> logger)
        {
            _context = context;
            _eventHubService = eventHubService;
            _logger = logger;
        }

        // GET: api/ResultModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultModel>>> GetResultModels()
        {
            return await _context.ResultModels.ToListAsync();
        }

        // GET: api/ResultModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultModel>> GetResultModel(Guid id)
        {
            var resultModel = await _context.ResultModels.FindAsync(id);

            if (resultModel == null)
            {
                return NotFound();
            }

            return resultModel;
        }

        // PUT: api/ResultModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResultModel(Guid id, [FromBody] ResultCreateDto resultDto)
        {
            var existingResult = await _context.ResultModels.FindAsync(id);

            if (existingResult == null)
            {
                return NotFound();
            }

            existingResult.AssessmentId = resultDto.AssessmentId;
            existingResult.UserId = resultDto.UserId;
            existingResult.Score = resultDto.Score;
            existingResult.AttemptDate = resultDto.AttemptDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/ResultModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResultModel>> PostResultModel(ResultCreateDto resultDto)
        {
            var resultModel = new ResultModel
            {
                ResultId = Guid.NewGuid(),
                AssessmentId = resultDto.AssessmentId,
                UserId = resultDto.UserId,
                Score = resultDto.Score,
                AttemptDate = resultDto.AttemptDate
            };

            _context.ResultModels.Add(resultModel);
            try
            {
                await _context.SaveChangesAsync();
                // Send event to Event Hub
                var eventData = new
                {
                    EventType = "ResultCreated",
                    ResultId = resultModel.ResultId,
                    AssessmentId = resultModel.AssessmentId,
                    UserId = resultModel.UserId,
                    Score = resultModel.Score,
                    AttemptDate = resultModel.AttemptDate
                };
                await _eventHubService.SendQuizEventAsync(eventData);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error saving result");
                if (ResultModelExists(resultModel.ResultId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetResultModel", new { id = resultModel.ResultId }, resultModel);
        }

        // DELETE: api/ResultModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResultModel(Guid id)
        {
            var resultModel = await _context.ResultModels.FindAsync(id);
            if (resultModel == null)
            {
                return NotFound();
            }

            _context.ResultModels.Remove(resultModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultModelExists(Guid id)
        {
            return _context.ResultModels.Any(e => e.ResultId == id);
        }
    }
}