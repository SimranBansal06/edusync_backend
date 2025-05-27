//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using webapi.Data;
//using webapi.Models;
//using webapi.Services;

//namespace webapi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CourseModelsController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly BlobStorageService _blobStorageService;

//        public CourseModelsController(AppDbContext context, BlobStorageService blobStorageService)
//        {
//            _context = context;
//            _blobStorageService = blobStorageService;
//        }

//        // GET: api/CourseModels
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<CourseModel>>> GetCourseModels()
//        {
//            return await _context.CourseModels.ToListAsync();
//        }

//        // GET: api/CourseModels/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<CourseModel>> GetCourseModel(Guid id)
//        {
//            var courseModel = await _context.CourseModels.FindAsync(id);

//            if (courseModel == null)
//            {
//                return NotFound();
//            }

//            return courseModel;
//        }

//        // PUT: api/CourseModels/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutCourseModel(Guid id, CourseModel courseModel)
//        {
//            if (id != courseModel.CourseId)
//            {
//                return BadRequest();
//            }

//            _context.Entry(courseModel).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!CourseModelExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/CourseModels
//        [HttpPost]
//        public async Task<ActionResult<CourseModel>> PostCourseModel(CourseModel courseModel)
//        {
//            _context.CourseModels.Add(courseModel);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetCourseModel", new { id = courseModel.CourseId }, courseModel);
//        }

//        // DELETE: api/CourseModels/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCourseModel(Guid id)
//        {
//            var courseModel = await _context.CourseModels.FindAsync(id);
//            if (courseModel == null)
//            {
//                return NotFound();
//            }

//            _context.CourseModels.Remove(courseModel);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool CourseModelExists(Guid id)
//        {
//            return _context.CourseModels.Any(e => e.CourseId == id);
//        }

//        // POST: api/CourseModels/upload
//        [HttpPost("upload")]
//        public async Task<IActionResult> UploadFile(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//            {
//                return BadRequest("No file uploaded");
//            }

//            try
//            {
//                // Upload file to Azure Blob Storage
//                string blobUrl = await _blobStorageService.UploadCourseFileAsync(file);

//                // Return with the "url" property exactly as expected by the frontend
//                return Ok(new
//                {
//                    url = blobUrl,
//                    fileName = file.FileName,
//                    fileType = file.ContentType,
//                    fileSize = file.Length
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="courseId"></param>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        // POST: api/CourseModels/upload/{courseId}
//        [HttpPost("upload/{courseId}")]
//        public async Task<IActionResult> UploadCourseFile(Guid courseId, IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//            {
//                return BadRequest("No file uploaded");
//            }

//            var course = await _context.CourseModels.FindAsync(courseId);
//            if (course == null)
//            {
//                return NotFound("Course not found");
//            }

//            try
//            {
//                // Upload file to Azure Blob Storage with course ID prefix
//                string blobUrl = await _blobStorageService.UploadCourseFileAsync(file, courseId.ToString());

//                // Update the course's MediaUrl
//                course.MediaUrl = blobUrl;
//                await _context.SaveChangesAsync();

//                return Ok(new
//                {
//                    url = blobUrl,
//                    fileName = file.FileName,
//                    fileType = file.ContentType,
//                    fileSize = file.Length
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }
//    }
//}


// Add this method to your existing CourseModelsController.cs class

// GET: api/CourseModels/download/{blobFileName}
[HttpGet("download/{blobFileName}")]
async Task<IActionResult> DownloadFile(string blobFileName)
{
    try
    {
        // Extract just the filename part if a full path was provided
        string fileName = Path.GetFileName(blobFileName);

        // Try to get the blob URL first
        try
        {
            // Get the file's URL from the blob storage
            string blobUrl = await _blobStorageService.GetBlobUrlAsync(fileName);

            // Redirect to the blob URL for direct download
            return Redirect(blobUrl);
        }
        catch (FileNotFoundException)
        {
            // If getting the URL fails, try to download the blob directly
            try
            {
                Stream fileStream = await _blobStorageService.DownloadBlobAsync(fileName);

                // Try to determine content type
                string contentType = "application/octet-stream";
                string extension = Path.GetExtension(fileName).ToLowerInvariant();

                switch (extension)
                {
                    case ".pdf": contentType = "application/pdf"; break;
                    case ".doc": case ".docx": contentType = "application/msword"; break;
                    case ".xls": case ".xlsx": contentType = "application/vnd.ms-excel"; break;
                    case ".ppt": case ".pptx": contentType = "application/vnd.ms-powerpoint"; break;
                    case ".jpg": case ".jpeg": contentType = "image/jpeg"; break;
                    case ".png": contentType = "image/png"; break;
                    case ".gif": contentType = "image/gif"; break;
                    case ".mp4": contentType = "video/mp4"; break;
                    case ".mp3": contentType = "audio/mpeg"; break;
                }

                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}