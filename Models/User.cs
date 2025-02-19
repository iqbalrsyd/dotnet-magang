using System.ComponentModel.DataAnnotations.Schema;

namespace be_magang.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegister
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}