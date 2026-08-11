using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ApiService
{
    private readonly HttpClient _httpClient;

    private const string BaseUrl = "https://localhost:5001/";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // GET: api/users
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(json);
    }

    // POST: api/users
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request)
    {
        var json = JsonConvert.SerializeObject(request);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TResponse>(responseJson);
    }

    // PUT: api/users/10
    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request)
    {
        var json = JsonConvert.SerializeObject(request);

        var content = new StringContent(
            json,
            Encoding.UTF8,

            "application/json");

        var response = await _httpClient.PutAsync(endpoint, content);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TResponse>(responseJson);
    }

    // DELETE: api/users/10
    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);

        return response.IsSuccessStatusCode;
    }
}