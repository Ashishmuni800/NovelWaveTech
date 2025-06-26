using Application.ServiceInterface;
using System;
using System.Threading.Tasks;

namespace Application.Service
{
    public class GenerateRandomCaptchaCode : IGenerateRandomCaptchaCode
    {
        public Task<string> GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            // Convert char array to string and return as a completed Task
            return Task.FromResult(new string(code));
        }
    }
}
