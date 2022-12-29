using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Dto;
using Models.Mapping;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly URLContext _context;
        private readonly IMapper _mapper;
        public AuthService(URLContext context, IMapper mapper)
        {
            _context= context;
            _mapper= mapper;
        }

        public async Task<bool> AddUserToDB(UserAddDto userAddDto)
        {
            if (userAddDto == null)
                return false;

            await _context.Users.AddAsync(_mapper.Map<User>(userAddDto));
            await _context.SaveChangesAsync();

            return true;
        }

        public UserAddDto CreatePasswordHash(UserPasswordDto request)
        {
            UserAddDto user = new() { Usarname = request.Username };

            using (var hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
            }

            return user;
        }

        public async Task<string> CreateToken(UserPasswordDto request, IConfiguration config)
        {
            UserViewModel userViewModel = _mapper.Map<UserViewModel>(await _context.Users.FirstOrDefaultAsync(u => u.Usarname == request.Username));

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, userViewModel.Usarname),
                new Claim(ClaimTypes.Role, userViewModel.Role.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwtoken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtoken;
        }

        public async Task<LoginResultEnum> LoginCheck(UserPasswordDto request)
        {
            UserViewModel userViewModel = _mapper.Map<UserViewModel>(await _context.Users.FirstOrDefaultAsync(u => u.Usarname == request.Username));

            if (!_context.Users.AnyAsync(u => u.Usarname == userViewModel.Usarname).Result)
                return LoginResultEnum.USERNAME_NOT_FOUND;

            if (!VerifyPasswordHash(request.Password, userViewModel.PasswordHash, userViewModel.PasswordSalt))
                return LoginResultEnum.WRONG_PASSWORD;

            return LoginResultEnum.SUCCESSFULLY;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
