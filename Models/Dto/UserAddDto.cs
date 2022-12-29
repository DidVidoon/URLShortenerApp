namespace Models.Dto
{
    public class UserAddDto
    {
        public string Usarname { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public RolsEnum Role { get; set; }
    }
}
