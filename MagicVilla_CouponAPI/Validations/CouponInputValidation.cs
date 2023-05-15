using FluentValidation;
using MagicVilla_CouponAPI.Models.DTOs;

namespace MagicVilla_CouponAPI.Validations
{
    public class CouponInputValidation : AbstractValidator<CouponInputDto>
    {
        public CouponInputValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
