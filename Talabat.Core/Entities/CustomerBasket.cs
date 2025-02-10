namespace Talabat.Core.Entities
{
    public class CustomerBasket
    {
        public string Id { get; set; } = null!;
        public IEnumerable<BasketItem> Items { get; set; } = null!;
    }
}
