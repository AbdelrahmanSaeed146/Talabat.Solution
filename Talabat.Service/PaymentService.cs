using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Specs;


namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration,IBasketRepository basketRepository,IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
           _basketRepository = basketRepository;
           _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CraeteOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket is null) return null;
            var ShippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var delviery = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                ShippingPrice = delviery.Cost;
                basket.ShippingPrice = ShippingPrice;
            }

            if (basket.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);

                    if(item.Price != product.Price)
                        item.Price = product.Price;
                }
            }

            PaymentIntent paymentIntent;
            PaymentIntentService paymentIntentService = new PaymentIntentService();

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)ShippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes =  new List<string>() {"card"}
                    
                };

                paymentIntent = await paymentIntentService.CreateAsync(options);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }

            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)ShippingPrice * 100
                };
                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid)
        {
            var _orderRepo =  _unitOfWork.Repository<Order>();

            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await _orderRepo.GetByIdWithSpecAsync(spec);

            if (order is null) return null;
            if (isPaid)
                order.Status = OrderStatus.PaymentReceived;
            else
                order.Status = OrderStatus.PaymentFaild;

             _orderRepo.Update(order);
            await _unitOfWork.CompleteAsync();
            return order;


        }
    }
}
