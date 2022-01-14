using System.ComponentModel.DataAnnotations;

namespace _57Blocks.Api.Models
{
    public class Users
    {
        public Guid ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

    }
}
