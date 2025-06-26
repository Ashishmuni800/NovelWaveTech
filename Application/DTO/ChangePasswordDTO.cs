using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ChangePasswordDTO
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        public string Password { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string NewPassword { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(32)]
        public string ConfirmNewPassword { get; set; }
        [Required]
        [MaxLength(6)]
        [MinLength(6)]
        public string CaptchaCode { get; set; }
    }
}
