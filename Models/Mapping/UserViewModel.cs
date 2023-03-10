using System.ComponentModel.DataAnnotations;

namespace Models.Mapping
{
    public class UserViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Usarname { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public RolsEnum Role { get; set; }
    }
}
