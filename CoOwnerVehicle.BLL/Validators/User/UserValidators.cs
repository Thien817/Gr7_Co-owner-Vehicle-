using FluentValidation;
using CoOwnerVehicle.BLL.DTOs.User;

namespace CoOwnerVehicle.BLL.Validators.User
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }

    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.FirstName).MaximumLength(100).When(x => x.FirstName != null);
            RuleFor(x => x.LastName).MaximumLength(100).When(x => x.LastName != null);
            RuleFor(x => x.PhoneNumber).MaximumLength(20).When(x => x.PhoneNumber != null);
        }
    }
}

