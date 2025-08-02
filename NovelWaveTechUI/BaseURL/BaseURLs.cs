
namespace NovelWaveTechUI.BaseURL
{
    public class BaseURLs : IBaseURLs
    {
        private readonly IConfiguration _configuration;
        public BaseURLs(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetURL()
        {
            string baseurl = _configuration["BaseUrl"];
            return baseurl;
        }
    }
}
