using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string Password { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        public string CaptchaCode { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
    }
}
