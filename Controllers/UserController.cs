using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KinstonLearning.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KinstonLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly KinstonCoursesContext _context;
        private readonly IConfiguration _configuration;

        public UserController(KinstonCoursesContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            // Hash the password
            user.PasswordHash = HashPassword(user.PasswordHash);

            // Set default values
            user.IsActive = false;
            user.IsApproved = false;
         //   user.RegistrationDate = DateTime.Now;

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                return Ok("User registered successfully.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while saving the user: " + ex.InnerException?.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email );

            // Check if user exists and password matches
            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            // Check if the user is approved
            if (!user.IsApproved)
            {
                return Unauthorized("User is not yet approved.");
            }

            // Generate a JWT token
            var token = GenerateJwtToken(user); // Call the method here

            return Ok(new
            {
                Token = token,
                UserName = user.Username,
                Role = user.Role,
                UserId = user.UserId // Add user ID to the response
            });


        }




        private string GenerateJwtToken(User user)
        {
            // Create claims based on the user information
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Subject
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique identifier
        new Claim("role", user.Role) // Include role as a claim
    };

            // Retrieve JWT settings from configuration
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationInMinutes = int.Parse(_configuration["JwtSettings:TokenExpirationInMinutes"]);

            // Create a security key based on the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationInMinutes), // Set expiration time
                signingCredentials: creds);

            // Return the generated token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            var enteredPasswordHash = HashPassword(enteredPassword);
            return enteredPasswordHash == storedPasswordHash;
        }
    }
}
