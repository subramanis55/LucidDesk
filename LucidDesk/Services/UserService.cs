using LucidDesk.DS.Models;
using LucidDesk.Models;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Services
{
    public static class UserService
    {
        public static ApiService ApiService { get; set; } = new ApiService();
        public static async Task<User> CreateUserAsync(UserCreateDTO user)
        {
            return await ApiService.PostAsync<UserCreateDTO, User>("api/user", user);
        }

        public static async Task<User> GetUserByIdAsync(string id)
        {
            return await ApiService.GetAsync<User>($"api/user/{id}");
        }

        public static async Task<User> GetUserByUserNumberAsync(string userNumber)
        {
            return await ApiService.GetAsync<User>($"api/user/{userNumber}");
        }
        public static async Task<PeerInfo> GetUserPeerInfoAsync(string userNumber)
        {
            return await ApiService.GetAsync<PeerInfo>($"api/user/{userNumber}/peerinfo");
        }

        public static async Task<User> UpdateUserAsync(string id, UserUpdateDTO user)
        {
            return await ApiService.PutAsync<UserUpdateDTO, User>($"api/user/{id}", user);
        }

    }
}


