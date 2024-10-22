using KinstonLearning.DTOs; // Import the DTO namespace
using KinstonLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;                                                                 
using System.Threading.Tasks;

namespace KinstonLearning.Controllers
{
    [Authorize(Roles = "Professor")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorCourseManagementController : ControllerBase
    {
        private readonly KinstonCoursesContext _context;

        public ProfessorCourseManagementController(KinstonCoursesContext context)
        {
            _context = context;
        }

        [HttpPost("create-course")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            // Validate the professor ID
            var professor = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == request.ProfessorId && u.Role == "Professor");
            if (professor == null)
            {
                return BadRequest("Invalid Professor ID.");
            }

            // Map the request to a new Course entity
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                CurrentBatchStartDate = request.CurrentBatchStartDate,
                CurrentBatchEndDate = request.CurrentBatchEndDate,
                IsActive = request.IsActive,
                IsApproved = false, // Default to not approved
                ProfessorId = request.ProfessorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok(course);
        }

        [HttpGet("approved-courses/{professorId}")]
        public async Task<IActionResult> GetApprovedCourses(int professorId)
        {
            // Validate the professor ID
            var professor = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == professorId && u.Role == "Professor");
            if (professor == null)
            {
                return BadRequest("Invalid Professor ID.");
            }

            // Retrieve all courses that are approved and belong to the specified professor
            var approvedCourses = await _context.Courses
                .Where(c => c.IsApproved && c.ProfessorId == professorId)
                .ToListAsync();

            return Ok(approvedCourses);
        }


        [HttpPut("disable-course/{id}")]
        public async Task<IActionResult> DisableCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            course.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(course);
        }

        [HttpPut("enable-course/{id}")]
        public async Task<IActionResult> EnableCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            course.IsActive = true;
            await _context.SaveChangesAsync();
            return Ok(course);
        }

        
    }
}
