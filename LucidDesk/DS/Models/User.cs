using System;

namespace LucidDesk.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string UserNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PCName { get; set; }
        public string MacAddress { get; set; } = string.Empty;
        public string DesktopImageString { get; set; } = string.Empty;
        public string profileImageString { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
    public class UserCreateDTO
    {
        public UserCreateDTO() { }
        public UserCreateDTO(string firstName, string pcName, string macAddress, string password, string desktopImageString)
        {
            this.DesktopImageString = desktopImageString;
            this.FirstName = firstName;
            this.PcName = pcName;
            this.MacAddress = macAddress;
            this.Password = password;
        }
        public string FirstName { get; set; }
        public string PcName { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string DesktopImageString { get; set; } = string.Empty;
        public string Password { get; set; }
    }

    public class UserUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImageString { get; set; }
        public string DesktopImageString { get; set; }
    }
}
