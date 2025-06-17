using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheLoadingBean.Shared.DTOs;
using TheLoadingBean.Shared.Models;
using TheLoadingBeanAPI.Data;

namespace TheLoadingBeanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<CustomerResponseDto>>> GetAllCustomers()
        {
            var customers = await _unitOfWork.Customers.GetAllCustomersAsync();
            return Ok(customers.Select(c => MapToResponseDto(c)));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomer(string id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Only allow customers to view their own profile, or admins to view any profile
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Ok(MapToResponseDto(customer));
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CustomerResponseDto>> SearchCustomerByEmail([FromQuery] string email)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(MapToResponseDto(customer));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponseDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            // Check if customer with same email already exists
            var existingCustomer = await _unitOfWork.Customers.GetCustomerByEmailAsync(createCustomerDto.Email);
            if (existingCustomer != null)
            {
                return BadRequest("A customer with this email already exists.");
            }

            var customer = new Customer
            {
                FirstName = createCustomerDto.FirstName,
                LastName = createCustomerDto.LastName,
                Email = createCustomerDto.Email,
                Phone = createCustomerDto.Phone,
                Address = createCustomerDto.Address,
                IsAdmin = false
            };

            await _unitOfWork.Customers.CreateCustomerAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, MapToResponseDto(customer));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(string id, UpdateCustomerDto updateCustomerDto)
        {
            var existingCustomer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Only allow customers to update their own profile, or admins to update any profile
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Check if new email is already taken by another customer
            if (existingCustomer.Email != updateCustomerDto.Email)
            {
                var emailTaken = await _unitOfWork.Customers.GetCustomerByEmailAsync(updateCustomerDto.Email);
                if (emailTaken != null)
                {
                    return BadRequest("This email is already taken by another customer.");
                }
            }

            existingCustomer.FirstName = updateCustomerDto.FirstName;
            existingCustomer.LastName = updateCustomerDto.LastName;
            existingCustomer.Email = updateCustomerDto.Email;
            existingCustomer.Phone = updateCustomerDto.Phone;
            existingCustomer.Address = updateCustomerDto.Address;

            await _unitOfWork.Customers.UpdateCustomerAsync(id, existingCustomer);
            await _unitOfWork.SaveChangesAsync();

            return Ok(MapToResponseDto(existingCustomer));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(string id)
        {
            var result = await _unitOfWork.Customers.DeleteCustomerAsync(id);
            if (!result)
            {
                return NotFound();
            }
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/make-admin")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            customer.IsAdmin = true;
            await _unitOfWork.Customers.UpdateCustomerAsync(id, customer);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/toggle-admin")]
        public async Task<IActionResult> ToggleAdmin(string id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            customer.IsAdmin = !customer.IsAdmin;

            await _unitOfWork.Customers.UpdateCustomerAsync(id, customer);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        private static CustomerResponseDto MapToResponseDto(Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                IsAdmin = customer.IsAdmin
            };
        }
    }
}
