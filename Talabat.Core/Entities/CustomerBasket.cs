namespace Talabat.Core.Entities
{
    public class CustomerBasket
    {
        public string Id { get; set; } = null!;
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal ShippingPrice { get; set; }
        public IEnumerable<BasketItem> Items { get; set; } = null!;
    }
}
