using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{

    public class PaymentController : BaseApiController
    {
  
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        // This is your Stripe CLI webhook secret for testing your endpoint locally.
        private const string endpointSecret = "whsec_fe565e1c7e2ad966fe8a2f5183e46d69b108d0c88292b88cc68d224c5d4489b1";

        public PaymentController(IPaymentService paymentService ,IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        [ProducesResponseType(typeof(CustomerBasketDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost("{basketid}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CraeteOrUpdatePaymentIntent(basketId);
            if (basket is null) return BadRequest(new ApiResponse(400, "An Error With Your Basket"));
            return Ok(basket);
        }

        [HttpPost("webhook")]

        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
           
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var paymenyIntent = (PaymentIntent)stripeEvent.Data.Object;

               if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdateOrderStatus(paymenyIntent.Id , true);
                }
               else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await _paymentService.UpdateOrderStatus(paymenyIntent.Id, false);
                }
                return Ok();
        }
    }
}
