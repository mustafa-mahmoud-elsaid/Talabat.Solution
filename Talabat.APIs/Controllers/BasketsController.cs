using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Talabat.APIs.Controllers
{
    public class BasketsController : BaseController
    {
        private readonly IBasketRepository _basketRepo;

        public BasketsController(IBasketRepository basketRepo)
        {
            _basketRepo = basketRepo;
        }
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string basketId)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            return basket is null ? new CustomerBasket() { Id = basketId } : basket; 
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket basket)
        {
            var updatedOrCreatedBasket = await _basketRepo.UpdateBasketAsync(basket);
            return updatedOrCreatedBasket is null ? BadRequest(new ApiResponse(400)) : updatedOrCreatedBasket;
        }
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string basketId)
        {
            return Ok(await _basketRepo.DeleteBasketAsync(basketId));
        }
    }
}
