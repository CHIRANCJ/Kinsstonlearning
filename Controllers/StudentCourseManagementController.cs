using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using KinstonLearning.DTOs; 
using KinstonLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace KinstonLearning.Controllers
{
    [Authorize(Roles = "Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseManagementController : ControllerBase
    {
        private readonly KinstonCoursesContext _context;

        public StudentCourseManagementController(KinstonCoursesContext context)
        {
            _context = context;
        }

        // Get all available courses that are approved and active
        [HttpGet("available-courses")]
        public async Task<IActionResult> GetAvailableCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsApproved && c.IsActive)
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("available-modules/{courseId}")]
        public IActionResult GetAvailableModules(int courseId)
        {
            var modules = _context.Modules.Where(m => m.CourseId == courseId).ToList();
            if (modules == null || !modules.Any())
            {
                return NotFound("No modules found for this course.");
            }
            return Ok(modules);
        }

        // Enroll a student in a course
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollInCourse([FromBody] EnrollRequest request)
        {
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.EnrollmentDate.Date == request.EnrollmentDate.Date);

            if (existingEnrollment != null)
            {
                return BadRequest("Student is already enrolled in a course on this date.");
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == request.CourseId && c.IsApproved && c.IsActive);

            if (course == null)
            {
                return BadRequest("Course is not available or is not approved.");
            }

            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollmentDate = request.EnrollmentDate
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(enrollment);
        }

        [HttpDelete("unenroll/{studentId}/{courseId}")]
        public async Task<IActionResult> UnenrollCourse(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound(new { Message = "Enrollment not found." });
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Successfully unenrolled from the course." });
        }

        // Start learning a module
        [HttpPost("start-learning")]
        public async Task<IActionResult> StartLearning([FromBody] StartLearningRequest request)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId);

            if (enrollment == null)
            {
                return BadRequest("Student is not enrolled in this course.");
            }

            var nextModule = await GetNextModule(request.StudentId, request.CourseId);
            if (nextModule is BadRequestObjectResult)
            {
                return nextModule; // Return the error response
            }

            return Ok(new { Message = "Learning started.", Module = nextModule });
        }

        [HttpPost("complete-module")]
        public async Task<IActionResult> CompleteModule([FromBody] CompleteModuleRequest request)
        {
            // Validate the request (e.g., check for nulls, etc.)

            // Create a new ModuleCompletion entry
            var moduleCompletion = new ModuleCompletion
            {
                StudentId = request.StudentId,
                ModuleId = request.ModuleId,
                CompletionDate = DateTime.UtcNow // or whatever your logic requires
            };

            _context.ModuleCompletions.Add(moduleCompletion);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Module completed successfully." });
        }


        [HttpGet("completed-modules/{studentId}/{courseId}")]
        public async Task<IActionResult> GetCompletedModulesForCourse(int studentId, int courseId)
        {
            var completedModules = await _context.ModuleCompletions
                .Where(mc => mc.StudentId == studentId)
                .Select(mc => new
                {
                    mc.ModuleId,
                    ModuleTitle = _context.Modules
                        .Where(m => m.ModuleId == mc.ModuleId && m.CourseId == courseId)
                        .Select(m => m.Title)
                        .FirstOrDefault(),
                    mc.CompletionDate
                })
                .ToListAsync();

            if (completedModules == null || !completedModules.Any())
            {
                return NotFound(new { Message = "No completed modules found for this student in the specified course." });
            }

            return Ok(completedModules);
        }


        [HttpPost("finish-course")]
        public async Task<IActionResult> FinishCourse([FromBody] FinishCourseRequest request)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId);

            if (enrollment == null)
            {
                return BadRequest("Student is not enrolled in this course.");
            }

            var courseCompletion = new CourseCompletion
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                CompletionDate = DateTime.Now
            };

            _context.CourseCompletions.Add(courseCompletion);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Course completed successfully. You can now download your certificate." });
        }

        [HttpGet("next-module/{studentId}/{courseId}")]
        public async Task<IActionResult> GetNextModule(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return BadRequest("Student is not enrolled in this course.");
            }

            var nextModule = await _context.Modules
                .Where(m => m.CourseId == courseId && m.IsActive &&
                            !_context.ModuleCompletions.Any(mc => mc.ModuleId == m.ModuleId && mc.StudentId == studentId))
                .OrderBy(m => m.Order)
                .FirstOrDefaultAsync();

            if (nextModule == null)
            {
                return Ok("All modules have been completed for this course.");
            }

            return Ok(nextModule);
        }

        [HttpGet("enrolled-courses/{studentId}")]
        public async Task<IActionResult> GetEnrolledCourses(int studentId)
        {
            var enrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => new
                {
                    e.CourseId,
                    CourseTitle = e.Course.Title,
                    CourseDescription = e.Course.Description,
                    e.EnrollmentDate,
                    StartDate = e.Course.CurrentBatchStartDate != DateTime.MinValue ? e.Course.CurrentBatchStartDate : (DateTime?)null,
                    EndDate = e.Course.CurrentBatchEndDate != DateTime.MinValue ? e.Course.CurrentBatchEndDate : (DateTime?)null,
                    Status = e.Course.IsActive ? "Active" : "Inactive",
                    ProfessorName = e.Course.Professor != null ? e.Course.Professor.Username : "Not Assigned",
                    ModuleCount = _context.Modules.Count(m => m.CourseId == e.CourseId)
                })
                .ToListAsync();

            if (enrolledCourses == null || !enrolledCourses.Any())
            {
                return NotFound(new { Message = "No enrolled courses found for this student." });
            }

            return Ok(enrolledCourses);
        }


        [HttpGet("completed-courses/{studentId:int}")]
        public async Task<IActionResult> GetCompletedCourses(int studentId)
        {
            try
            {
                var completedCourses = await _context.CourseCompletions
                    .Where(cc => cc.StudentId == studentId)
                    .Select(cc => new
                    {
                        CourseId = cc.CourseId,
                        CourseTitle = cc.Course.Title,
                        CompletionDate = cc.CompletionDate
                    })
                    .ToListAsync();

                return Ok(completedCourses);
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                Console.Error.WriteLine($"Error fetching completed courses: {ex}");
                return StatusCode(500, "An error occurred while fetching completed courses.");
            }
        }


    }
}
