using System.ComponentModel.DataAnnotations;

namespace MagicVilla_CouponAPI.Models.DTOs
{
    public record CouponInputDto
    {
        [Required]
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
