namespace Inventory_Management_System.CustomAttributes
{

    using Inventory_Management_System.Repository;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class QuantityAvailableAttribute : ValidationAttribute
    {
        private readonly string _productIdPropertyName;

        public QuantityAvailableAttribute(string productIdPropertyName)
        {
            _productIdPropertyName = productIdPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not int requestedQuantity)
                return new ValidationResult("Invalid quantity.");

            // Retrieve the ProductId property value dynamically
            var productIdProperty = validationContext.ObjectType.GetProperty(_productIdPropertyName);
            if (productIdProperty == null)
                return new ValidationResult($"Property '{_productIdPropertyName}' not found.");

            var productId = (int)productIdProperty.GetValue(validationContext.ObjectInstance);

            // Get the IProductRepository from the service container
            var productRepository = validationContext.GetService<IProductRepository>();
            if (productRepository == null)
                return new ValidationResult("Product repository not available.");

            // Get available quantity from the repository
            Product product = productRepository.GetById(productId);
            int availableQuantity = product?.StockQuantity ?? 0;

            if (requestedQuantity > availableQuantity)
            {
                return new ValidationResult(
                    $"The requested quantity ({requestedQuantity}) of {product.Name} exceeds the available quantity ({availableQuantity}).");
            }

            return ValidationResult.Success;
        }
    }
}