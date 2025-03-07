using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;
using Talabat.Core.Specifications;

namespace Talabat.Core.OrdersSpecifications
{
    public class OrderWithPaymentIntentIdSpec : BaseSpecification<Order>
    {
        public OrderWithPaymentIntentIdSpec(string paymentIntentId)
            : base(O => O.PaymentIntentId == paymentIntentId)
        {

        }
    }
}
