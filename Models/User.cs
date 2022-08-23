using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class User
    {
        [Key]
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
    }
    
}