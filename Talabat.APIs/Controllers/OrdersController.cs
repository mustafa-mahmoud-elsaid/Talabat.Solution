using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        [ProducesResponseType(typeof(OrderToReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder(OrderDTO model)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDTO, Address>(model.ShipToAddress);
            var result = await _orderService
                .CreateOrderAsync(buyerEmail!, model.BasketId, model.DeliveryMethodId,address);
            if (result is null) return BadRequest(new ApiResponse(400));
            return Ok(_mapper.Map<Order, OrderToReturnDTO>(result));
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _orderService.GetOrderForUserAsync(buyerEmail!);
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDTO>>(result));
        }
        [ProducesResponseType(typeof(OrderToReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrderForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForUserAsync(id, buyerEmail!);
            if (order is null) return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<Order, OrderToReturnDTO>(order));
        }
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await _orderService.GetDeliveryMethodsAsync());
        }
    }
}
