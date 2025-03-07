using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Events;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        //private readonly ILogger _logger;
        private const string _endpointSecret = "whsec_60c8dd633ea4a8d0450c608fc975dd9b4e17627ba68f90b6cbf9aa1dd1c000c2";
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), 200)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{id}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string id)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntentAsync(id);
            if (basket is null) return BadRequest(new ApiResponse(400, "Error With Your Basket"));
            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _endpointSecret);
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            Order order;
            // Handle the event
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    order = await _paymentService.UpdatePaymentIntentToSucceedOrFailedAsync(paymentIntent!.Id, true);
                   // _logger.LogInformation("Payment is succeeded :)", paymentIntent.Id);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdatePaymentIntentToSucceedOrFailedAsync(paymentIntent!.Id, false);
                   // _logger.LogInformation("Payment is failed :(", paymentIntent.Id);
                    break;
            }

            return Ok();

        }
    }
}

