using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Mapping;
using Services.Interfaces;
using System.Text;

namespace Services
{
    public class URLService : IURLService
    {
        private readonly URLContext _context;
        private readonly IMapper _mapper;

        public URLService(URLContext uRLCOntext, IMapper mapper)
        {
            _context = uRLCOntext;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(URLAddDto uRLAddDto, string baseAdress)
        {
            if(_context.URLs.Any(u => u.Url == uRLAddDto.Url))
                return false;

            uRLAddDto.Identifier = URLShortener().Result;
            uRLAddDto.UrlShort = baseAdress + uRLAddDto.Identifier;
            await _context.URLs.AddAsync(_mapper.Map<URL>(uRLAddDto));
            await _context.SaveChangesAsync();

            return true;
        }

        public DeleteRequestResultEnum DeleteAllURL()
        {
            var deleteRequestResult = DeleteRequestResultEnum.SUCCESSFULLY;
            
            if(_context.URLs.Count() == 0)
            {
                deleteRequestResult = DeleteRequestResultEnum.NOT_FOUND;
                return deleteRequestResult;
            }
            
            foreach (var url in _context.URLs)
                _context.URLs.Remove(url);

            _context.SaveChanges();

            return deleteRequestResult;
        }

        public async Task<DeleteRequestResultEnum> DeleteURLAsync(int Id)
        {
            var deleteRequestResult = DeleteRequestResultEnum.SUCCESSFULLY;

            URL uRL = await _context.URLs.FirstAsync(u => u.Id == Id);

            if (uRL == null)
            {
                deleteRequestResult = DeleteRequestResultEnum.NOT_FOUND;
                return deleteRequestResult;
            }
 
            _context.URLs.Remove(uRL);
            await _context.SaveChangesAsync();

            return deleteRequestResult;
        }

        public async Task<List<URLViewModel>> GetAllAsync()
        {
            var uRLList = await _context.URLs.ToListAsync();

            return _mapper.Map<List<URLViewModel>>(uRLList);
        }

        public async Task<bool> UpdateURLAsync(URLEditDto uRLEditDto)
        {
            if (!_context.URLs.Any(u => u.Id == uRLEditDto.Id))
                return false;

            if(_context.URLs.Any(u => u.UrlShort == uRLEditDto.UrlShort))
                return false;

            URL currentURL = await _context.URLs.FirstAsync(u => u.Id == uRLEditDto.Id);

            currentURL.Id = uRLEditDto.Id;
            currentURL.Url = uRLEditDto.Url;
            currentURL.UrlShort = uRLEditDto.UrlShort;
            currentURL.Identifier = uRLEditDto.Identifier;

            _context.URLs.Update(currentURL);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> URLRedirect(string shortURL)
        {
            URL fullURL = await _context.URLs.FirstAsync(u => u.UrlShort == shortURL);

            return fullURL.Url;
        }

        public async Task<string> URLShortener()
        {
            int length = 5;
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            string shortURLString = "";
            StringBuilder shortURLBuilder = new();
            Random random = new();

            for (int i = 0; i < length; i++)
            {
                shortURLString = shortURLBuilder.Append(chars[random.Next(chars.Length)]).ToString();
            }

            if (await _context.URLs.FirstOrDefaultAsync(u => (u.UrlShort == shortURLBuilder.ToString())) != null)
            {
                shortURLString = URLShortener().Result;
            }

            return shortURLString.ToString();
        }
    }
}
