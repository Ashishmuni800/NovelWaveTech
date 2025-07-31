namespace NovelWaveTechUI.BaseURL
{
    public class BaseURLs
    {
        private readonly IConfiguration _configuration;
        public BaseURLs(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SetURL(string url) 
        {
            string baseurl = _configuration["BaseUrl"];
            return baseurl+url;
        }
    }
}
