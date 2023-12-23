using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Complain
    {
        public int Id { get; set; }
        [StringLength(500)]
        public string TitleAr { get; set; }
        [StringLength(500)]
        public string TitleEn { get; set; }
        [StringLength(500)]
        public string DescriptionAr { get; set; }
        [StringLength(500)]
        public string DescriptionEn { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Createddate { get; set; }
    }
}
