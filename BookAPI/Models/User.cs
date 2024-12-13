namespace BookAPI.Models
{
    public class User
    {
        public int Id { get; set; }  // Primary Key
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
