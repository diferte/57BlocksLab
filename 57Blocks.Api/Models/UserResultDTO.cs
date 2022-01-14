using System.ComponentModel.DataAnnotations;

namespace _57Blocks.Api.Models
{
    public class UserResultDTO
    {
        public Guid ID { get; set; }

        public string Token { get; set; }
    }
}
