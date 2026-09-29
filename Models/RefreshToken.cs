namespace ProductInventoryAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = "";

        public int ProductInventoryUserId { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;
    }
}