using Models;
using Models.Dto;
using Models.Mapping;

namespace Services.Interfaces
{
    public interface IURLService
    {
        public Task<List<URLViewModel>> GetAllAsync();
        public Task<bool> AddAsync(URLAddDto uRLAddDto, string baseAdress);
        public Task<bool> UpdateURLAsync(URLEditDto uRLEditDto);
        public Task<DeleteRequestResultEnum> DeleteURLAsync(int typeId);
        public DeleteRequestResultEnum DeleteAllURL();
        public Task<string> URLShortener();
        public Task<string> URLRedirect(string shortURL);
    }
}