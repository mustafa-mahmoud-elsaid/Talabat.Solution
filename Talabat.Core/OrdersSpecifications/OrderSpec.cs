using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;
using Talabat.Core.Specifications;

namespace Talabat.Core.OrdersSpecifications
{
    public class OrderSpec : BaseSpecification<Order>
    {
        public OrderSpec(string email)
            : base(O => O.BuyerEmail == email)
        {
            Includes.Add(O => O.DeliveryMethod!);
            Includes.Add(O => O.OrderItem);
            OrderByDesc = O => O.OrderDate;
        }
        public OrderSpec(int orderId, string email)
            : base(O => O.Id == orderId && O.BuyerEmail == email)
        {
            Includes.Add(O => O.DeliveryMethod!);
            Includes.Add(O => O.OrderItem);
        }
    }
}
