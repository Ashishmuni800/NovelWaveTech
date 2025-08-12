using Application.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApiHttpClient
{
    public class HttpClients : IHttpClients
    {
        private readonly HttpClient _httpClient;
        public HttpClients(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeleteAsync(string url, string id)
        {
            // Normally, you don't send data in a DELETE request body,
            // but if you must, ensure the server expects it like this.

            var jsonContent = JsonConvert.SerializeObject(id);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestUrl = $"{url}/{Uri.EscapeDataString(id)}";

            // Sending the DELETE request with the serialized ID as content
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUrl)
            {
                Content = content
            }).ConfigureAwait(false);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                return true; // Return true for success
            }
            else
            {
                // Handle failure and return false or throw an exception
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
            }
        }


        public async Task<string> GetAsync(string url, string jwtToken)
        {


            //using var client = new HttpClient();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Request failed with status code {response.StatusCode}");

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return content;
        }

        public async Task<string> PostAsync(string url, object data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);

            // Create a StringContent object with the serialized JSON and set the content type to application/json
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Sending the POST request asynchronously with the JSON content
            var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Return the response content as a string if successful
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                // Handle failure (you could throw an exception or return the error message)
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
            }
        }
        public async Task<string> GetByIdAsync(string url, string id)
        {
            // Append the ID as a query parameter
            var requestUrl = $"{url}/{Uri.EscapeDataString(id)}";

            // Send GET request
            var response = await _httpClient.GetAsync(requestUrl).ConfigureAwait(false);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Assuming you want to return the response body as a string
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                // Handle failure and throw an exception
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
            }
        }

        public async Task<string> PostUpdateAsync(string url, string Id, object data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);

            // Create a StringContent object with the serialized JSON and set the content type to application/json
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestUrl = $"{url}/{Uri.EscapeDataString(Id)}";
            // Sending the POST request asynchronously with the JSON content
            var response = await _httpClient.PutAsync(requestUrl, content).ConfigureAwait(false);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Return the response content as a string if successful
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                // Handle failure (you could throw an exception or return the error message)
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
            }
        }

        public async Task<string> PostAsync(string url, object data, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var jsonContent = JsonConvert.SerializeObject(data);

            // Create a StringContent object with the serialized JSON and set the content type to application/json
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Sending the POST request asynchronously with the JSON content
            var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Return the response content as a string if successful
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                // Handle failure (you could throw an exception or return the error message)
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
            }
        }

        public async Task<string> GetAsync(string url)
        {

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return ($"Request failed with status code {response.StatusCode}");

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var contentStream = await response.Content.ReadAsStreamAsync();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return content;
        }
    }
}