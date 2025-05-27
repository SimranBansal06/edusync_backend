
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

namespace webapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentModelsController : ControllerBase
    {
        private readonly AppDbContext _context;
       
        public AssessmentModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AssessmentModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssessmentModel>>> GetAssessmentModels()
        {

            try
            {
                return await _context.AssessmentModels.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// get all assessement model
        /// <return> assessement model</return>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/AssessmentModels/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AssessmentModel>> GetAssessmentModel(Guid id)
        //{
        //    var assessmentModel = await _context.AssessmentModels.FindAsync(id);

        //    if (assessmentModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return assessmentModel;
        //}
        [HttpGet("ByCourse/{courseId}")]
        public async Task<ActionResult<AssessmentModel>> GetAssessmentByCourseId(Guid courseId)
        {
            var assessment = await _context.AssessmentModels
                .FirstOrDefaultAsync(a => a.CourseId == courseId);

            if (assessment == null)
            {
                return NotFound();
            }

            return assessment;
        }
        // PUT: api/AssessmentModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssessmentModel(Guid id, AssessmentModel assessmentModel)
        {
            if (id != assessmentModel.AssessmentId)
            {
                return BadRequest();
            }

            _context.Entry(assessmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssessmentModelExists(id))
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

        // POST: api/AssessmentModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssessmentModel>> PostAssessmentModel(AssessmentCreateDto assessmentDto)
        {
            var assessmentModel = new AssessmentModel
            {
                AssessmentId = Guid.NewGuid(),
                CourseId = assessmentDto.CourseId,
                Title = assessmentDto.Title,
                Questions = assessmentDto.Questions,
                MaxScore = assessmentDto.MaxScore
            };

            _context.AssessmentModels.Add(assessmentModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AssessmentModelExists(assessmentModel.AssessmentId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            //return CreatedAtAction("GetAssessmentModel", new { id = assessmentModel.AssessmentId }, assessmentModel);
            return CreatedAtAction("GetAssessmentByCourseId", new { courseId = assessmentModel.CourseId }, assessmentModel);
        }

        // DELETE: api/AssessmentModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessmentModel(Guid id)
        {
            var assessmentModel = await _context.AssessmentModels.FindAsync(id);
            if (assessmentModel == null)
            {
                return NotFound();
            }

            _context.AssessmentModels.Remove(assessmentModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssessmentModelExists(Guid id)
        {
            return _context.AssessmentModels.Any(e => e.AssessmentId == id);
        }
    }
}