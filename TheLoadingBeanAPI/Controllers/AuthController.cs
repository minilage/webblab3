using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TheLoadingBean.Shared.DTOs;
using TheLoadingBean.Shared.Models;
using TheLoadingBeanAPI.Data;
using TheLoadingBeanAPI.Services;

namespace TheLoadingBeanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthController(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterDto registerDto)
        {
            var existingCustomer = await _unitOfWork.Customers.GetCustomerByEmailAsync(registerDto.Email);
            if (existingCustomer != null)
                return BadRequest("A user with this email already exists.");

            var customer = new Customer
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                PasswordHash = HashPassword(registerDto.Password),
                IsAdmin = false
            };

            await _unitOfWork.Customers.CreateCustomerAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtService.GenerateToken(customer.Id, customer.Email, "Customer");

            return Ok(new TokenDto
            {
                Token = token.Token,
                Expiration = token.Expiration,
                Email = customer.Email,
                IsAdmin = customer.IsAdmin
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByEmailAsync(loginDto.Email);
            if (customer == null || !VerifyPassword(loginDto.Password, customer.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var role = customer.IsAdmin ? "Admin" : "Customer";
            var token = _jwtService.GenerateToken(customer.Id, customer.Email, role);

            return Ok(new TokenDto
            {
                Token = token.Token,
                Expiration = token.Expiration,
                Email = customer.Email,
                IsAdmin = customer.IsAdmin
            });
        }

        [HttpPost("admin/register")]
        public async Task<ActionResult<TokenDto>> RegisterAdmin(RegisterDto registerDto)
        {
            var existingCustomer = await _unitOfWork.Customers.GetCustomerByEmailAsync(registerDto.Email);
            if (existingCustomer != null)
                return BadRequest("A user with this email already exists.");

            var customer = new Customer
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                PasswordHash = HashPassword(registerDto.Password),
                IsAdmin = true
            };

            await _unitOfWork.Customers.CreateCustomerAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtService.GenerateToken(customer.Id, customer.Email, "Admin");

            return Ok(new TokenDto
            {
                Token = token.Token,
                Expiration = token.Expiration,
                Email = customer.Email,
                IsAdmin = customer.IsAdmin
            });
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
