﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterface
{
    public interface IGenerateRandomCaptchaCode
    {
        Task<string> GenerateRandomCode(int length);
    }
}
