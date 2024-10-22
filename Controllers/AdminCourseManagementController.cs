using Microsoft.AspNetCore.Mvc;
using KinstonLearning.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace KinstonLearning.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/courses")]
    [ApiController]
    public class AdminCourseManagementController : ControllerBase
    {
        private readonly KinstonCoursesContext _context;

        public AdminCourseManagementController(KinstonCoursesContext context)
        {
            _context = context;
        }

        // Approve Course
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.IsApproved = true;
            course.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course approved successfully." });
        }

        // Reject Course
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.IsApproved = false;
            course.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course rejected successfully." });
        }

        // Suspend Course
        [HttpPut("{id}/suspend")]
        public async Task<IActionResult> SuspendCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course suspended successfully." });
        }

        // Enable Course
        [HttpPut("{id}/enable")]
        public async Task<IActionResult> EnableCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course enabled successfully." });
        }

        // Get all courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }
    }
}
