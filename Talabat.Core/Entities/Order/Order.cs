using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order
{
    public class Order : BaseEntity
    {
        public Order()
        {
            
        }

        public Order(string buyerEmail, Address shippingAddress, ICollection<OrderItem> orderItem, DeliveryMethod? deliveryMethod, decimal subtotal)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            OrderItem = orderItem;
            DeliveryMethod = deliveryMethod;
            Subtotal = subtotal;
        }

        public string BuyerEmail { get; set; } = null!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; } = null!;
        public ICollection<OrderItem> OrderItem { get; set; } = new HashSet<OrderItem>();
        public DeliveryMethod? DeliveryMethod { get; set; }
        public decimal Subtotal{ get; set; }

        //[NotMapped]
        //public decimal Total => Subtotal + DeliveryMethod.Cost;

        //Or Using Method
        public decimal GetTotal()
            => Subtotal + DeliveryMethod.Cost;
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
