using System.Net.Http;
using System.Net.Http.Json;

namespace LucidDesk.Services
{
    public static class UserService
    {
        public static async bool CreateUserAsync(User user)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync("https://localhost:5001/api/users", user);
                return response.IsSuccessStatusCode;
            }
        }
    }
}


