using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepository;
        private readonly IGenericRepository<Order> _orderRepository;

        public OrderService(
            IBasketRepository basketRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<DeliveryMethod> deliveryMethodRepository,
            IGenericRepository<Order> orderRepository
            )
        {
            _basketRepository = basketRepository;
            _productRepository = productRepository;
            _deliveryMethodRepository = deliveryMethodRepository;
            _orderRepository = orderRepository;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. get basket by id
            var basket = await _basketRepository.GetBasketAsync(basketId);

            // 2. now we have the items(products) but we will get the products data from productRepo to be safe
            List<OrderItem> orderItems = new List<OrderItem>();
            if(basket?.Items?.Count() > 0)
            {
                foreach( var item in basket.Items)
                {
                    var product = await _productRepository.GetAsync(item.Id);
                    var productOrdered = new ProductItemOrdered(product!.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productOrdered, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
                    
            }

            // 3. calculate subtotal

            var subTotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

            // 4. get delivery method from deliveryMethodRepo

            var deliveryMethod = await _deliveryMethodRepository.GetAsync(deliveryMethodId);

            // 5. create an order object 

            var CreatedOrder = new Order(buyerEmail, shippingAddress, orderItems, deliveryMethod, subTotal);
            await _orderRepository.AddAsync(CreatedOrder);
            // 6. save to DB
            //-------//
            return CreatedOrder;
        }

        public Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}
