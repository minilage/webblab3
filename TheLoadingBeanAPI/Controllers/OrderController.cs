using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheLoadingBean.Shared.DTOs;
using TheLoadingBean.Shared.Models;
using TheLoadingBeanAPI.Data;

namespace TheLoadingBeanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAllOrders()
        {
            var orders = await _unitOfWork.Orders.GetAllOrdersAsync();
            return Ok(orders.Select(o => MapToResponseDto(o)));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(string id)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Only allow customers to view their own orders, or admins to view any order
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != order.CustomerId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Ok(MapToResponseDto(order));
        }

        [Authorize]
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<OrderResponseDto>>> GetCustomerOrders(string customerId)
        {
            // Only allow customers to view their own orders, or admins to view any customer's orders
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != customerId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var orders = await _unitOfWork.Orders.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders.Select(o => MapToResponseDto(o)));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            // Verify that the customer exists
            var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(createOrderDto.CustomerId);
            if (customer == null)
            {
                return BadRequest("Customer not found.");
            }

            // Verify that the customer is creating their own order
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != createOrderDto.CustomerId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Verify that all products exist and are available
            foreach (var item in createOrderDto.Items)
            {
                var product = await _unitOfWork.Products.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found.");
                }
                if (!product.IsAvailable)
                {
                    return BadRequest($"Product {product.Name} is not available.");
                }
            }

            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = createOrderDto.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                TotalAmount = createOrderDto.Items.Sum(item => item.Quantity * item.UnitPrice)
            };

            await _unitOfWork.Orders.CreateOrderAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapToResponseDto(order));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderResponseDto>> UpdateOrder(string id, UpdateOrderDto updateOrderDto)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = updateOrderDto.Status;
            order.Items = updateOrderDto.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList();
            order.TotalAmount = updateOrderDto.Items.Sum(item => item.Quantity * item.UnitPrice);

            await _unitOfWork.Orders.UpdateOrderAsync(id, order);
            await _unitOfWork.SaveChangesAsync();

            return Ok(MapToResponseDto(order));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(string id)
        {
            var result = await _unitOfWork.Orders.DeleteOrderAsync(id);
            if (!result)
            {
                return NotFound();
            }
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            order.Status = status;

            await _unitOfWork.Orders.UpdateOrderAsync(id, order);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        private static OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }
    }
}
