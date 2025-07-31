namespace Domain.Model
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CaptchaCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
