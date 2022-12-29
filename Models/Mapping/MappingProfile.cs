using AutoMapper;
using Models.Dto;

namespace Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<URL, URLViewModel>().ReverseMap();
            CreateMap<URL, URLAddDto>().ReverseMap();
            CreateMap<URL, URLEditDto>().ReverseMap();

            CreateMap<User, UserAddDto>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
        }
    }
}
