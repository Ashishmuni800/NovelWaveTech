using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApiHttpClient
{
    public interface IHttpClients
    {
        Task<string> GetAsync(string url);
        Task<string> GetAsync(string url, bool AuthHeader);
        Task<string> PostAsync(string url, object data);
        Task<string> PostAsync(string url, object data, bool AuthHeader);
        Task<string> PostUpdateAsync(string url, string Id, object data);
        Task<string> GetByIdAsync(string url, string Id);
        Task<bool> DeleteAsync(string url, string id);
    }
}