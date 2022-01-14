using System.ComponentModel.DataAnnotations;

namespace _57Blocks.Api.Models
{
    public class Sales
    {
        public Guid ID { get; set; }
        public Guid CreatorID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Desciption { get; set; }
        public decimal Value { get; set; }
        public bool IsPublic { get; set; }

    }
}
