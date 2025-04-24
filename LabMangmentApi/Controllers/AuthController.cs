using LabMangmentApi.Data;
using LabMangmentApi.Helper;
using LabMangmentApi.Models.Dtos.Auth;
using LabMangmentApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LabMangmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtOptions _jwt;

        public AuthController(UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtOptions> jwtOptions, ApplicationDbContext context)
        {
            _context = context;
            _userManager = manager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwt = jwtOptions.Value;
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] ApplicationUserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User with this email already exists." });

            var roleExists = await _roleManager.RoleExistsAsync(dto.Type);
            if (!roleExists)
                return BadRequest(new { message = "Role does not exist." });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                IsActive = dto.Status.ToLower() == "active",
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return BadRequest(new { message = string.Join("; ", createResult.Errors.Select(e => e.Description)) });

            var roleResult = await _userManager.AddToRoleAsync(user, dto.Type);
            if (!roleResult.Succeeded)
                return BadRequest(new { message = "User created but failed to assign role." });

            var newUser = new User
            {
                FullName = dto.Name,
                NationalId = dto.NationalId,
                PhoneNumber = dto.PhoneNumber,
                UserType = dto.Type,
                CreatedAt = dto.RegistrationDate,
                ApplicationUser = user
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "User created successfully" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _userManager.Users.Include(u => u.User).FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { status = "fail", message = "Invalid email or account is inactive" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { status = "fail", message = "Invalid credentials" });

            var roles = await _userManager.GetRolesAsync(user);

            // Create JWT Token
            var token = await CreateJwtToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Handle Refresh Token
            var refreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            if (refreshToken == null)
            {
                refreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }


            return Ok(new LoginResponseDto
            {
                Status = "success",
                Token = tokenString,
                Roles = roles,
                User = new
                {
                    id = user.Id,
                    name = user.User?.FullName ?? user.UserName,
                    email = user.Email
                },
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            });
        }

        //[Authorize]
        [HttpGet("AllUsers")]
        public IActionResult GetAll()
        {
            var users = _userManager.Users
                .Include(u => u.User)
                .Where(u => u.User != null)
                .Select(u => new ApplicationUserDto
                {
                    Id = u.Id,
                    Name = u.User.FullName,
                    Email = u.Email,
                    NationalId = u.User.NationalId,
                    Type = u.User.UserType,
                    Status = u.IsActive ? "active" : "inactive",
                    RegistrationDate = u.User.CreatedAt
                }).ToList();
            if (users == null || !users.Any())
                return BadRequest("لا يوجد مستخدمين فى النظام");

            return Ok(users);
        }

        [HttpGet("UserById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.Users.Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("المستخدم غير موجود");

            var dto = new ApplicationUserDto
            {
                Id = user.Id,
                Name = user.User.FullName,
                Email = user.Email,
                NationalId = user.User.NationalId,
                Type = user.User.UserType,
                Status = user.IsActive ? "active" : "inactive",
                RegistrationDate = user.User.CreatedAt
            };

            return Ok(dto);
        }

        [HttpPut("UpdateUserById/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("المستخدم غير موجود");

            // تحقق من وجود الدور
            var roleExists = await _roleManager.RoleExistsAsync(dto.Type);
            if (!roleExists)
                return BadRequest(new { message = $"Role '{dto.Type}' does not exist." });

            // تعديل بيانات ApplicationUser
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.IsActive = dto.Status.ToLower() == "active";

            // تعديل بيانات User (اللي ليها علاقة بجدول آخر)
            if (user.User != null)
            {
                user.User.FullName = dto.Name;
                user.User.NationalId = dto.NationalId;
                user.User.UserType = dto.Type;
                user.User.CreatedAt = dto.RegistrationDate;
            }

            // تحديث بيانات ApplicationUser
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(new { message = string.Join("; ", updateResult.Errors.Select(e => e.Description)) });

            // تحديث الدور
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(dto.Type))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, dto.Type);
            }

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        [HttpDelete("DeleteUserById/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("المستخدم غير موجود");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent(); // 204
        }

        [HttpGet("Search")]
        public IActionResult Search(string query)
        {
            var users = _userManager.Users
                .Include(u => u.User)
                .Where(u =>
                    u.User != null &&
                    (
                        u.User.FullName.Contains(query) ||
                        u.Email.Contains(query) ||
                        u.User.NationalId.Contains(query)
                    ))
                .Select(u => new ApplicationUserDto
                {
                    Id = u.Id,
                    Name = u.User.FullName,
                    Email = u.Email,
                    NationalId = u.User.NationalId,
                    Type = u.User.UserType,
                    Status = u.IsActive ? "active" : "inactive",
                    RegistrationDate = u.User.CreatedAt
                })
                .ToList();

            if (users == null || !users.Any())
                return BadRequest("لا يوجد مستخدمين موافق مع البحث فى النظام");

            return Ok(users);
        }

        [HttpGet("Filter")]
        public IActionResult FilterByType(string type)
        {
            var users = _userManager.Users.Include(u => u.User)
                .Where(u => u.User.UserType.ToLower() == type.ToLower())
                .Select(u => new ApplicationUserDto
                {
                    Id = u.Id,
                    Name = u.User.FullName,
                    Email = u.Email,
                    NationalId = u.User.NationalId,
                    Type = u.User.UserType,
                    Status = u.IsActive ? "active" : "inactive",
                    RegistrationDate = u.User.CreatedAt
                }).ToList();

            if (users == null || !users.Any())
                return BadRequest("لا يوجد مستخدمين فى النظام");

            return Ok(users);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // استخراج userId من الـ JWT Claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid token: no user ID.");

                // البحث عن المستخدم
                var user = await _userManager.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Email == userId);

                if (user == null)
                    return BadRequest("المستخدم غير موجود");

                // إلغاء كل الـ RefreshTokens النشطة (Force logout from all devices)
                foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
                {
                    token.RevokedOn = DateTime.UtcNow;
                }

                await _userManager.UpdateAsync(user);

                return Ok("Logged out successfully. Please login again to continue.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Logout failed: {ex.Message}" });
            }
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: credentials
            );

            return token;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}


