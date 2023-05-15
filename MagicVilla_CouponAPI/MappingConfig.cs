using AutoMapper;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTOs;

namespace MagicVilla_CouponAPI
{
    public class MappingConfig :Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponInputDto>().ReverseMap();
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
