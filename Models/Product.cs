using System.ComponentModel.DataAnnotations;

namespace ProductInventoryAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0,1000000)]
        public decimal Price { get; set; }

        [Range(0, 1000)]
        public int Quantity { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<Supplier> Suppliers { get; set; }= new List<Supplier>();
    }
}
