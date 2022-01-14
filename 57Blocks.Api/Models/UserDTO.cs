using System.ComponentModel.DataAnnotations;

namespace _57Blocks.Api.Models
{
    public class UserDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string LastName { get; set; }

    }
}
