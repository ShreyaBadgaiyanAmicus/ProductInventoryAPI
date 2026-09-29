namespace ProductInventoryAPI.DTO
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<Supplierdto> Suppliers { get; set; } = new List<Supplierdto>();
    }
}