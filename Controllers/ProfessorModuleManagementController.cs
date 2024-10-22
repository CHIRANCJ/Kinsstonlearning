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
    public class ProfessorModuleManagementController : ControllerBase
    {
        private readonly KinstonCoursesContext _context;

        public ProfessorModuleManagementController(KinstonCoursesContext context)
        {
            _context = context;
        }

        // Create a module associated with a course and professor
        [HttpPost("create-module")]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequest request)
        {
            // Validate the CourseId and ProfessorId
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == request.CourseId && c.ProfessorId == request.ProfessorId && c.IsActive && c.IsApproved);

            if (course == null)
            {
                return BadRequest("Invalid Course ID or the course is inactive/unapproved or not assigned to this professor.");
            }

            // Create a new Module entity using the request data
            var module = new Module
            {
                CourseId = request.CourseId, // Foreign Key
                Title = request.Title,
                Description = request.Description,
                Order = request.Order,
                IsActive = request.IsActive
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return Ok(module);
        }

        // Get all approved modules for a given course and professor
        [HttpGet("course-modules/{courseId}/{professorId}")]
        public async Task<IActionResult> GetModulesByCourse(int courseId, int professorId)
        {
            // Validate the CourseId and ProfessorId
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.ProfessorId == professorId && c.IsApproved);

            if (course == null)
            {
                return NotFound("Course not found or is not approved for this professor.");
            }

            // Retrieve all modules associated with the given approved course
            var modules = await _context.Modules
                .Where(m => m.CourseId == courseId)
                .ToListAsync();

            return Ok(modules);
        }

        // Disable an existing module for a specific course and professor
        [HttpPut("disable-module/{id}/{professorId}/{courseId}")]
        public async Task<IActionResult> DisableModule(int id, int professorId, int courseId)
        {
            // Validate the CourseId and ProfessorId
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.ProfessorId == professorId);

            if (course == null)
            {
                return BadRequest("Invalid Course ID or this course is not assigned to the professor.");
            }

            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound();

            module.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(module);
        }

        // Enable an existing module for a specific course and professor
        [HttpPut("enable-module/{id}/{professorId}/{courseId}")]
        public async Task<IActionResult> EnableModule(int id, int professorId, int courseId)
        {
            // Validate the CourseId and ProfessorId
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.ProfessorId == professorId);

            if (course == null)
            {
                return BadRequest("Invalid Course ID or this course is not assigned to the professor.");
            }

            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound();

            module.IsActive = true;
            await _context.SaveChangesAsync();
            return Ok(module);
        }
    }
}
