using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeigthTrackerApplication.Models;

namespace WeigthTrackerApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly WightListContext _context;
        private readonly IConfiguration _config;

        public LoginController(ILogger<LoginController> logger, WightListContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Email and password are required.");

            var vendor = _context.Vendors
                .FirstOrDefault(v => v.VendorEmail == login.Email && v.PasswordHash == login.Password);

            if (vendor != null)
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, vendor.VendorName),
            new Claim(ClaimTypes.Role, "Vendor"),
            new Claim("VendorId", vendor.VendorId.ToString())
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    vendorId = vendor.VendorId,
                    vendorName = vendor.VendorName,
                    role = "Vendor"
                });
            }

            // Check farmer if not vendor
            var farmer = _context.Farmers
                .FirstOrDefault(f => f.FarmerEmail == login.Email && f.PassswordHAsh == login.Password);

            if (farmer != null)
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, farmer.FarmerName),
            new Claim(ClaimTypes.Role, "Farmer"),
            new Claim("FarmerId", farmer.FarmerId.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    farmerId = farmer.FarmerId,
                    farmerName = farmer.FarmerName,
                    role = "Farmer"
                });
            }

            return Unauthorized("Invalid credentials.");
        }

        [HttpPost("farmer")]
        public IActionResult FarmerLogin([FromBody] LoginRequest login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Email and password are required.");

            var farmer = _context.Farmers
                .FirstOrDefault(f => f.FarmerEmail == login.Email && f.PassswordHAsh == login.Password);

            if (farmer == null)
                return Unauthorized("Invalid farmer credentials.");

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, farmer.FarmerName),
            new Claim(ClaimTypes.Role, "Farmer"),
            new Claim("FarmerId", farmer.FarmerId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                farmerId = farmer.FarmerId,
                farmerName = farmer.FarmerName,
                role = "Farmer"
            });
        }
    }
}
