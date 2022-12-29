using System.ComponentModel.DataAnnotations;

namespace Models.Mapping
{
    public class URLViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string UrlShort { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
    }
}
