using Microsoft.Extensions.Configuration;
using Models;
using Models.Dto;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        UserAddDto CreatePasswordHash(UserPasswordDto request);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        Task<string> CreateToken(UserPasswordDto request, IConfiguration config);
        Task<bool> AddUserToDB(UserAddDto userAddDto);
        Task<LoginResultEnum> LoginCheck(UserPasswordDto request);
    }
}
