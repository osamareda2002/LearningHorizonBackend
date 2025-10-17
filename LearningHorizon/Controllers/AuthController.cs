using Google.Apis.Auth;
using LearningHorizon.Data;
using LearningHorizon.Data.DTO;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using LearningHorizon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Org.BouncyCastle.Crypto.Generators;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace LearningHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthController(ApplicationDbContext context, JwtTokenService tokenService, IConfiguration configuration, IUserRepository userRepository)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DtoAuthLogin dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == dto.email);
            if (user == null || user.password != dto.password)
                return Ok(new { result = 401});

            var token = _tokenService.GenerateUserToken(user);
            user.lastToken = token;
            await _context.SaveChangesAsync();

            return Ok(new { result = 200, Token = token });
        }

        [Authorize]
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            var idClaim = (User.FindFirst("id")?.Value);

            if(idClaim == null)
                return Unauthorized(new { isValid = false });

            var userId = int.Parse(idClaim);

            var user = await _context.Users.FindAsync(userId);

            if(user == null || user.lastToken == null)
                return Unauthorized(new { isValid = false });

            return Ok(new { isValid = true });
        }


        [HttpPost("GoogleSignIn")]
        public async Task<IActionResult> GoogleSignIn([FromBody] DtoGoogleSignInRequest request)
        {
            try
            {
                // Verify Google ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

                // Check if user exists
                var user = _context.Users.FirstOrDefault(u => u.email == payload.Email && u.isDeleted != true);
                if (user == null)
                {
                    // Create new user if not exists
                    user = new User
                    {
                        email = payload.Email,
                        firstName = payload.GivenName,
                        lastName = payload.FamilyName,
                        password = Guid.NewGuid().ToString(),
                        profilePic = payload.Picture
                    };

                    _context.Users.Add(user);
                }
                else if(user.profilePic == null)
                    user.profilePic = payload.Picture;
                

                // Generate JWT for your system
                var token = _tokenService.GenerateUserToken(user);
                user.lastToken = token;

                await _context.SaveChangesAsync();

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(DtoSignup dto)
        {
            var user = _userRepository.FindBy(x => x.email == dto.email && x.isDeleted != true).FirstOrDefault();
            if (user == null)
                return Ok(new { message = "If the email exists, a reset link will be sent." });


            var resetToken = Guid.NewGuid().ToString();
            var expiryDate = DateTime.UtcNow.AddHours(1);

            user.resetToken = resetToken;
            user.resetTokenExpiry = expiryDate;


            var resetLink = $"{_configuration["Deployment:FrontEnd"]}reset-password?token={resetToken}";

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Services", "PasswordResetTemplate2.html");

            string templateContent;
            try
            {
                templateContent = System.IO.File.ReadAllText(templatePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reading the email template", details = ex.Message });
            }

            string htmlBody = templateContent.Replace("{{USER_NAME}}", $"{user.firstName} {user.lastName}");
            htmlBody = htmlBody.Replace("{{RESET_LINK}}", resetLink);

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
            };

            // Send email
            try
            {
                await EmailService.SendEmailAsync(user.email, "Password Reset Request", bodyBuilder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending email", details = ex.Message });
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset link sent successfully." });
        }


        [HttpPost("verify-reset-token")]
        public async Task<IActionResult> VerifyResetToken([FromBody] DtoVerifyTokenRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.resetToken == request.Token && u.resetTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return BadRequest(new { error = "Invalid or expired token" });
                }

                return Ok(new { message = "Token is valid" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] DtoResetPasswordRequest request)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.resetToken == request.Token && u.resetTokenExpiry > DateTime.UtcNow);

                if (user == null)
                {
                    return BadRequest(new { error = "Invalid or expired reset token" });
                }

                user.password = request.NewPassword;

                // Clear reset token
                user.resetToken = null;
                user.resetTokenExpiry = null;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DtoAddUser dto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.email == dto.email && u.isDeleted != true);
                if (existingUser != null)
                {
                    return Ok(new { result = 409, message = "Email already registered" });
                }

                // Create new user
                var newUser = new User
                {
                    firstName = dto.firstName,
                    lastName = dto.lastName,
                    email = dto.email,
                    password = dto.password, // Remember to hash this in production!
                                             // password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Use this for hashing
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { result = 200, message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return Ok(new { result = 500, message = ex.Message });
            }
        }

        private static Dictionary<string, string> VerificationCodes = new Dictionary<string, string>();

        [HttpPost]
        [Route("send-verification-code")]
        public async Task<IActionResult> VerifyEmail(DtoSignup dto)
        {
            var user = _userRepository.FindBy(x => x.email == dto.email && x.isDeleted != true).FirstOrDefault();

            if (user != null)
            {
                return Ok(new { result = 409, message = "Email already registered" });
            }

            string otpCode = new Random().Next(100000, 999999).ToString();

            VerificationCodes[dto.email] = otpCode;

            // Read email template
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Services", "VerifyEmailTemplate.html");
            string templateContent;
            try
            {
                templateContent = System.IO.File.ReadAllText(templatePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reading the email template", details = ex.Message });
            }

            // Replace OTP placeholder in HTML
            string htmlBody = templateContent.Replace("{{OTP_CODE}}", otpCode);
            var body = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = $"Your OTP code is: {otpCode}. Enter this code in the verification field to confirm your email."
            };

            // Send email
            try
            {
                await EmailService.SendEmailAsync(dto.email, "Email Confirmation", body);
                return Ok(new { result = 200, message = "Verification code sent successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { result = 500, message = ex.Message });
            }
        }

        [HttpPost("verify-email-code")]
        public IActionResult VerifyEmailCode([FromBody] DtoVerifyCodeRequest request)
        {
            try
            {
                if (!VerificationCodes.ContainsKey(request.Email))
                {
                    return Ok(new { result = 400, message = "No verification code found for this email" });
                }

                if (VerificationCodes[request.Email] != request.Code)
                {
                    return Ok(new { result = 400, message = "Invalid verification code" });
                }

                // Code is valid - remove it
                VerificationCodes.Remove(request.Email);

                return Ok(new { result = 200, message = "Email verified successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { result = 500, message = ex.Message });
            }
        }

    }
}
