﻿namespace Domain.Model
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
    }
}
