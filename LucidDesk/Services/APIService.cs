using LucidDesk.Manager.Settings;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public string BaseUrl = "";

        private JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public ApiService()
        {
            BaseUrl = SettingsManager.Settings.ServerBaseURL;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // GET: api/users
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl + endpoint);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                // Handle the exception as needed

            }
            return default;
        }

        // POST: api/users
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BaseUrl + endpoint, content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }
            catch (Exception ex)
            {
                // Handle the exception as needed

            }
            return default;

        }

        // PUT: api/users/10
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request, settings);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(BaseUrl + endpoint, content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }
            catch (Exception)
            {

            }
            return default;
        }

        // DELETE: api/users/10
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(BaseUrl + endpoint);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Handle the exception as needed

            }
            return false;
        }
    }
}


