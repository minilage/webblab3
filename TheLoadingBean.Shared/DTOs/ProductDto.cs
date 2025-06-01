using System.ComponentModel.DataAnnotations;

namespace TheLoadingBean.Shared.DTOs
{
    public class ProductResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string ProductNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public bool IsDiscontinued { get; set; }

        public string? ImageUrl { get; set; }

    }

    public class CreateProductDto
    {
        [Required]
        public string ProductNumber { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;
        public bool IsDiscontinued { get; set; } = false;
    }

    public class UpdateProductDto
    {
        [Required]
        public string ProductNumber { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
        public bool IsDiscontinued { get; set; }
    }

    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string ProductNumber { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
        public bool IsDiscontinued { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
