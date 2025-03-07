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

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. get basket by id
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if(basket == null) return null;
            // 2. now we have the items(products) but we will get the products data from productRepo to be safe
            List<OrderItem> orderItems = new List<OrderItem>();
            if(basket.Items?.Count() > 0)
            {
                var productRepo = _unitOfWork.Repository<Product>();
                foreach ( var item in basket.Items)
                {
                    var product = await productRepo.GetAsync(item.Id);
                    var productOrdered = new ProductItemOrdered(product!.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productOrdered, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
                    
            }

            // 3. calculate subtotal

            var subTotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

            // 4. get delivery method from deliveryMethodRepo

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);
            //-----------                  ----------------------
            var spec = new OrderWithPaymentIntentIdSpec(basket!.PaymentIntentId ?? "");
            var orderRepo = _unitOfWork.Repository<Order>();
            var existingOrder = await orderRepo.GetEntityWithSpecAsync(spec);
            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
            }
            
            // 5. create an order object 
            var CreatedOrder = new Order(buyerEmail, shippingAddress, orderItems, deliveryMethod, subTotal, basket.PaymentIntentId!);
            await orderRepo.AddAsync(CreatedOrder);
            // 6. save to DB
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return CreatedOrder;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        public async Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderSpec(orderId, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpec(buyerEmail);
            return await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
        }
    }
}
