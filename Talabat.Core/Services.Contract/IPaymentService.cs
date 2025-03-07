using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Services.Contract
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntentAsync(string basketId);
        Task<Order> UpdatePaymentIntentToSucceedOrFailedAsync(string paymentIntentId, bool isSucceeded);
    }
}
