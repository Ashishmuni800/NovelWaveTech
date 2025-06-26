using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class GenerateCaptchaCodeViewModel
    {
        public string Id { get; set; }
        public string CaptchaCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
