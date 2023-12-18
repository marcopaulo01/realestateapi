namespace Realty_Connect.DTO
{
    public class Property
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double Price { get; set; }
        public int UserId { get; set; }
    }
}
