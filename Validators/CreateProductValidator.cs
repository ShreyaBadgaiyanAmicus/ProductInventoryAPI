using FluentValidation;
using ProductInventoryAPI.DTO;

namespace ProductInventoryAPI.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .When(x => x.Quantity > 0)
                .WithMessage("Price must be greater than 0 when quantity is greater than 0");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity cannot be negative");
        }
    }
}