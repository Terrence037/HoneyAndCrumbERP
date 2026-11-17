using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HoneyAndCrumb.Client
{
    public static class ApiClient
    {
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7247/")
        };

        public static async Task<T> GetAsync<T>(string relativeUrl)
        {
            HttpResponseMessage response = await _http.GetAsync(relativeUrl);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(string.Format("API error ({0}): {1}", (int)response.StatusCode, json));

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<TResponse> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest body)
        {
            string payload = JsonConvert.SerializeObject(body);
            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _http.PostAsync(relativeUrl, content);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(string.Format("API error ({0}): {1}", (int)response.StatusCode, json));

            return JsonConvert.DeserializeObject<TResponse>(json);
        }
    }
}