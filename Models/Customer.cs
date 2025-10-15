using System.ComponentModel.DataAnnotations;

namespace KiranaStore.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Navigation property: one customer can have many orders
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
