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
    [Route("api/admin/users")]
    [ApiController]
    public class AdminUserManagementController : ControllerBase
    {
        private readonly KinstonCoursesContext _context;

        public AdminUserManagementController(KinstonCoursesContext context)
        {
            _context = context;
        }

        // Approve Professor
        [HttpPut("professors/{id}/approve")]
        public async Task<IActionResult> ApproveProfessor(int id)
        {
            var professor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Professor" && !u.IsApproved);
            if (professor == null)
            {
                return NotFound("Professor not found or already approved.");
            }

            professor.IsApproved = true;
            professor.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Professor approved successfully." });
        }

        // Reject Professor
        [HttpPut("professors/{id}/reject")]
        public async Task<IActionResult> RejectProfessor(int id)
        {
            var professor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Professor" && !u.IsApproved);
            if (professor == null)
            {
                return NotFound("Professor not found or already approved.");
            }

            professor.IsApproved = false;
            professor.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Professor rejected successfully." });
        }

        // Suspend Professor
        [HttpPut("professors/{id}/suspend")]
        public async Task<IActionResult> SuspendProfessor(int id)
        {
            var professor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Professor" && u.IsApproved);
            if (professor == null)
            {
                return NotFound("Professor not found or not approved.");
            }

            professor.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Professor suspended successfully." });
        }

        // Enable Professor
        [HttpPut("professors/{id}/enable")]
        public async Task<IActionResult> EnableProfessor(int id)
        {
            var professor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Professor" && u.IsApproved);
            if (professor == null)
            {
                return NotFound("Professor not found or not approved.");
            }

            professor.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Professor enabled successfully." });
        }

        // Approve Student
        [HttpPut("students/{id}/approve")]
        public async Task<IActionResult> ApproveStudent(int id)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Student" && !u.IsApproved);
            if (student == null)
            {
                return NotFound("Student not found or already approved.");
            }

            student.IsApproved = true;
            student.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student approved successfully." });
        }

        // Reject Student
        [HttpPut("students/{id}/reject")]
        public async Task<IActionResult> RejectStudent(int id)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Student" && !u.IsApproved);
            if (student == null)
            {
                return NotFound("Student not found or already approved.");
            }

            student.IsApproved = false;
            student.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student rejected successfully." });
        }

        // Suspend Student
        [HttpPut("students/{id}/suspend")]
        public async Task<IActionResult> SuspendStudent(int id)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Student" && u.IsApproved);
            if (student == null)
            {
                return NotFound("Student not found or not approved.");
            }

            student.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student suspended successfully." });
        }

        // Enable Student
        [HttpPut("students/{id}/enable")]
        public async Task<IActionResult> EnableStudent(int id)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Student" && u.IsApproved);
            if (student == null)
            {
                return NotFound("Student not found or not approved.");
            }

            student.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student enabled successfully." });
        }

        // Get all professors
        [HttpGet("professors")]
        public async Task<ActionResult<IEnumerable<User>>> GetProfessors()
        {
            var professors = await _context.Users
                .Where(u => u.Role == "Professor")
                .ToListAsync();
            return Ok(professors);
        }

        // Get approved professors
        [HttpGet("professors/approved")]
        public async Task<ActionResult<IEnumerable<User>>> GetApprovedProfessors()
        {
            var approvedProfessors = await _context.Users
                .Where(u => u.Role == "Professor" && u.IsApproved)
                .ToListAsync();
            return Ok(approvedProfessors);
        }

        // Get unapproved professors
        [HttpGet("professors/unapproved")]
        public async Task<ActionResult<IEnumerable<User>>> GetUnapprovedProfessors()
        {
            var unapprovedProfessors = await _context.Users
                .Where(u => u.Role == "Professor" && !u.IsApproved)
                .ToListAsync();
            return Ok(unapprovedProfessors);
        }

        // Get all students
        [HttpGet("students")]
        public async Task<ActionResult<IEnumerable<User>>> GetStudents()
        {
            var students = await _context.Users
                .Where(u => u.Role == "Student")
                .ToListAsync();
            return Ok(students);
        }

        // Get approved students
        [HttpGet("students/approved")]
        public async Task<ActionResult<IEnumerable<User>>> GetApprovedStudents()
        {
            var approvedStudents = await _context.Users
                .Where(u => u.Role == "Student" && u.IsApproved)
                .ToListAsync();
            return Ok(approvedStudents);
        }

        // Get unapproved students
        [HttpGet("students/unapproved")]
        public async Task<ActionResult<IEnumerable<User>>> GetUnapprovedStudents()
        {
            var unapprovedStudents = await _context.Users
                .Where(u => u.Role == "Student" && !u.IsApproved)
                .ToListAsync();
            return Ok(unapprovedStudents);
        }

        // Get all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var allUsers = await _context.Users.ToListAsync();
            return Ok(allUsers);
        }

    }
}
