using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.OrdersSpecifications;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration,
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            // we want to get the amount 
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket is null) return null;
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                if (deliveryMethod is not null)
                {
                    shippingPrice = deliveryMethod.Cost;
                    basket.ShippingPrice = shippingPrice;
                }
            }
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                if (product is not null)
                {
                    if (product.Price != item.Price)
                        item.Price = product.Price;

                }
            }
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))// Create paymentIntent
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(basket.Items.Sum(I => I.Price * 100 * I.Quantity) + shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await paymentIntentService.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // Update paymentIntent
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(basket.Items.Sum(I => I.Price * 100 * I.Quantity) + shippingPrice * 100),
                };
                paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailedAsync(string paymentIntentId, bool isSucceeded)
        {
            var spec = new OrderWithPaymentIntentIdSpec(paymentIntentId);
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetEntityWithSpecAsync(spec);
            if(isSucceeded) 
                order!.Status = OrderStatus.PaymentReceived;
            else
                order!.Status = OrderStatus.PaymentFailed;
            orderRepo.Update(order);
            await _unitOfWork.CompleteAsync();
            return order;
        }
    }
}
