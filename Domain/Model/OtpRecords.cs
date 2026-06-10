namespace Domain.Model
{
    public class OtpRecords
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryAt { get; set; }
        public bool OtpUsed { get; set; }
    }
}
