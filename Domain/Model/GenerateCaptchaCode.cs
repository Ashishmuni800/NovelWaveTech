using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class GenerateCaptchaCode
    {
        public string Id { get; set; }
        public string CaptchaCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
