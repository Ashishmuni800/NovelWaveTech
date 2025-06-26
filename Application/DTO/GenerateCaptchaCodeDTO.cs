using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GenerateCaptchaCodeDTO
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(6)]
        [MinLength(6)]
        public string CaptchaCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
