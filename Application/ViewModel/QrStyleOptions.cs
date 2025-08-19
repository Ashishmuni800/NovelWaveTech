using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace Application.ViewModel
{

    public class QrStyleOptions
    {
        public Color? BackgroundColor { get; set; }
        public Color? GradientStart { get; set; }
        public Color? GradientEnd { get; set; }
        public bool TransparentBackground { get; set; } = false;
        public bool CircularLogo { get; set; } = true;
        public string? HeaderText { get; set; }
        public string? ForegroundHex { get; set; }
        public string? BackgroundHex { get; set; }
    }


}
