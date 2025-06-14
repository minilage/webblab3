using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheLoadingBean.Shared.DTOs;
using TheLoadingBean.Shared.Models;
using TheLoadingBeanAPI.Data;

namespace TheLoadingBeanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductResponseDto>>> GetAllProducts()
        {
            var products = await _unitOfWork.Products.GetAllProductsAsync();
            return Ok(products.Select(p => MapToResponseDto(p)));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(string id)
        {
            var product = await _unitOfWork.Products.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(MapToResponseDto(product));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductResponseDto>>> SearchProducts([FromQuery] string searchTerm)
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
            return Ok(products.Select(p => MapToResponseDto(p)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                ProductNumber = createProductDto.ProductNumber,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Category = createProductDto.Category,
                IsAvailable = true,
                IsDiscontinued = false
            };

            await _unitOfWork.Products.CreateProductAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, MapToResponseDto(product));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(string id, UpdateProductDto updateProductDto)
        {
            var existingProduct = await _unitOfWork.Products.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.ProductNumber = updateProductDto.ProductNumber;
            existingProduct.Name = updateProductDto.Name;
            existingProduct.Description = updateProductDto.Description;
            existingProduct.Price = updateProductDto.Price;
            existingProduct.Category = updateProductDto.Category;
            existingProduct.IsAvailable = updateProductDto.IsAvailable;
            existingProduct.IsDiscontinued = updateProductDto.IsDiscontinued;

            await _unitOfWork.Products.UpdateProductAsync(id, existingProduct);
            await _unitOfWork.SaveChangesAsync();

            return Ok(MapToResponseDto(existingProduct));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var result = await _unitOfWork.Products.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("discontinued")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ProductResponseDto>>> GetDiscontinuedProducts()
        {
            var products = await _unitOfWork.Products.GetDiscontinuedProductsAsync();
            return Ok(products.Select(p => MapToResponseDto(p)));
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductResponseDto>>> GetAvailableProducts()
        {
            var products = await _unitOfWork.Products.GetAvailableProductsAsync();
            return Ok(products.Select(p => MapToResponseDto(p)));
        }

        private static ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ProductNumber = product.ProductNumber,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                IsAvailable = product.IsAvailable,
                IsDiscontinued = product.IsDiscontinued
            };
        }
    }
}
