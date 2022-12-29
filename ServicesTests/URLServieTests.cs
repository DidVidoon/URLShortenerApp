using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto;
using Models.Mapping;
using Services;
using System.Text;

namespace ServicesTests
{
    public class URLServieTests
    {
        const int countOfURLs = 10;

        [Fact]
        public void GetAllAsync_Test()
        {
            //arrange
            var options = new DbContextOptionsBuilder<URLContext>()
                .UseInMemoryDatabase(databaseName: "TestURLsDb")
                .Options;

            SetDatabase(countOfURLs, out List<string> URLs, out List<string> URLsShort, out List<string> Identifier, options);

            var configMapper = new MapperConfiguration(cfg => cfg.CreateMap<URL, URLViewModel>().ReverseMap());

            //act
            var acessToURLs = new URLService(new URLContext(options), new Mapper(configMapper));

            var actualTypes = acessToURLs.GetAllAsync().Result;

            //assert
            Assert.Equal(countOfURLs, actualTypes.Count);
        }

        [Fact]
        public void UpdateURLAsync_Test()
        {
            //arrange
            var options = new DbContextOptionsBuilder<URLContext>()
               .UseInMemoryDatabase(databaseName: "TestURLsDb")
               .Options;

            SetDatabase(countOfURLs, out List<string> URLs, out List<string> URLsShort, out List<string> Identifier, options);

            var configMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<URL, URLEditDto>().ReverseMap();
                cfg.CreateMap<URL, URLViewModel>().ReverseMap();
            });

            var expectedIdentifier = "test identifier";
            var expectedURL = "TestURL";
            var expectedURLShort = "TwstShortUrl";
            bool expetedUpdateSuccessful = true;

            //act
            var accessToURLs = new URLService(new URLContext(options), new Mapper(configMapper));

            URLEditDto uRLEditDto = new() { Id = 1, Identifier = expectedIdentifier, Url = expectedURL, UrlShort = expectedURLShort };

            var isUpdatedSuccessfully = accessToURLs.UpdateURLAsync(uRLEditDto).Result;

            var firstURL = accessToURLs.GetAllAsync().Result.FirstOrDefault(u => u.Id == uRLEditDto.Id);

            //assert
            Assert.Equal(firstURL.Identifier, expectedIdentifier);
            Assert.Equal(firstURL.Url, expectedURL);
            Assert.Equal(firstURL.UrlShort, expectedURLShort);
            Assert.Equal(isUpdatedSuccessfully, expetedUpdateSuccessful);
        }

        [Fact]
        public void DeleteURLAsync_Test()
        {
            //arrange
            var options = new DbContextOptionsBuilder<URLContext>()
                .UseInMemoryDatabase(databaseName: "TestURLsDb")
                .Options;

            SetDatabase(countOfURLs, out List<string> URLs, out List<string> URLsShort, out List<string> Identifier, options);

            var configMapper = new MapperConfiguration(cfg => cfg.CreateMap<URL, URLViewModel>().ReverseMap());
            DeleteRequestResultEnum expectedDeleteSuccessful = DeleteRequestResultEnum.SUCCESSFULLY;

            //act
            var acessToURLs = new URLService(new URLContext(options), new Mapper(configMapper));
            var isDeletedSuccessfully = acessToURLs.DeleteURLAsync(1).Result;
            var listOfURLs = acessToURLs.GetAllAsync().Result;

            //assert
            Assert.Equal(listOfURLs.Count, countOfURLs - 1);
            Assert.Equal(isDeletedSuccessfully, expectedDeleteSuccessful);
        }

        [Fact]
        public void DeleteAllURL()
        {
            //arrange
            var options = new DbContextOptionsBuilder<URLContext>()
                .UseInMemoryDatabase(databaseName: "TestURLsDb")
                .Options;

            SetDatabase(countOfURLs, out List<string> URLs, out List<string> URLsShort, out List<string> Identifier, options);

            var configMapper = new MapperConfiguration(cfg => cfg.CreateMap<URL, URLViewModel>().ReverseMap());
            DeleteRequestResultEnum expectedDeleteSuccessful = DeleteRequestResultEnum.SUCCESSFULLY;
            var expectedCount = 0;

            //act
            var acessToURLs = new URLService(new URLContext(options), new Mapper(configMapper));
            var isDeletedSuccessfully = acessToURLs.DeleteAllURL();
            var listOfURLs = acessToURLs.GetAllAsync().Result;

            //assert
            Assert.Equal(expectedCount, listOfURLs.Count);
            Assert.Equal(expectedDeleteSuccessful, isDeletedSuccessfully);
        }

        [Fact]
        public void Add_Test()
        {
            //arrange
            var options = new DbContextOptionsBuilder<URLContext>()
               .UseInMemoryDatabase(databaseName: "TestURLsDb")
               .Options;

            SetDatabase(countOfURLs, out List<string> URLs, out List<string> URLsShort, out List<string> Identifier, options);

            var configMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<URL, URLAddDto>().ReverseMap();
                cfg.CreateMap<URL, URLViewModel>().ReverseMap();
            });
            var expectedIdentifier = "TestIdentifier";
            var expectedUrl = "TestUrl";
            var expectedUrlShort = "TestUrlShort";
            var expectedURLSuccesful = true;
            //act
            var acessToURLs = new URLService(new URLContext(options), new Mapper(configMapper));

            URLAddDto uRLAddDto = new() { Identifier = expectedIdentifier, Url = expectedUrl, UrlShort = expectedUrlShort };
            var actualIsAddSucessful = acessToURLs.AddAsync(uRLAddDto, "testBaseAdress").Result;
            var listOfURLs = acessToURLs.GetAllAsync().Result;

            //assert
            Assert.Equal(expectedURLSuccesful, actualIsAddSucessful);
            Assert.Equal(listOfURLs.Count, countOfURLs + 1);
        }

        [Fact]
        public void URLShortener_Test()
        {
            //arrange
            var options = new DbContextOptionsBuilder<URLContext>()
               .UseInMemoryDatabase(databaseName: "TestURLsDb")
               .Options;
            var configMapper = new MapperConfiguration(cfg => cfg.CreateMap<URL, URLViewModel>().ReverseMap());
            var expectedLength = 5;

            //act
            var acessToURLs = new URLService(new URLContext(options), new Mapper(configMapper));

            var actualLength = acessToURLs.URLShortener().Result;

            //assert
            Assert.Equal(expectedLength, actualLength.Length);
        }

        private void SetDatabase(int countOfURLs, out List<string> URLs, out List<string> URLsShort, out List<string> Identifier, DbContextOptions<URLContext> options)
        {
            URLs = new List<string>();
            for (int i = 0; i < countOfURLs; i++)
            {
                URLs.Add(RandomString(12));
            }

            URLsShort = new List<string>();
            for (int i = 0; i < countOfURLs; i++)
            {
                URLsShort.Add(RandomString(8));
            }

            Identifier = new List<string>();
            for (int i = 0; i < countOfURLs; i++)
            {
                Identifier.Add(RandomString(8));
            }

            using (var context = new URLContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                for (int i = 0; i < countOfURLs; i++)
                {
                    context.URLs.Add(new URL { UrlShort = URLsShort[i], Url = URLs[i], Identifier = Identifier[i] });
                }

                context.SaveChanges();
                context.Dispose();
            }
        }

        private string RandomString(int l)
        {
            var str = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < l; i++)
            {
                var randomNumber = (byte)random.Next(0, 255);
                str.Append(Convert.ToChar(randomNumber));
            }
            return str.ToString();
        }
    }
}