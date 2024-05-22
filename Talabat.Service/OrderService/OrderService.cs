using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService
            (
                IBasketRepository basketRepo,
                IUnitOfWork unitOfWork,
                IPaymentService paymentService

            )
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(/*string BuyerEmail,*/ string BasketId, int DeliveryMethodId, Adsress shippingaddress)
        {
            // 1.Get Basket From Baskets Repo
            var basket = await _basketRepo.GetBasketAsync(BasketId);

            // 2. Get Selected Items at Basket From Products Repo

            var orderItems = new List<OrderItem>();
            if (basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var productItemOrderd = new ProductItemOrdered(item.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productItemOrderd, product.Price, item.Quantity);
                    orderItems.Add(orderItem);

                }
            }

            // 3. Calculate SubTotal

            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4. Get Delivery Method From DeliveryMethods Repo

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(basket?.PaymentIntentId);
            var existingOrder = await orderRepo.GetByIdWithSpecAsync(spec);
            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
               await _paymentService.CraeteOrUpdatePaymentIntent(BasketId);
            }
            // 5. Create Order

            var order = new Order
                (
                buyerEmail: "",
                shippingAddress: shippingaddress,
                deliveryMethod: deliveryMethod,
                items: orderItems,
                subtotal: subtotal,
                paymentIntentId:basket?.PaymentIntentId ?? ""
                );

             _unitOfWork.Repository<Order>().Add(order);

            // 6. Save To Database [TODO]

           var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string BuyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications(BuyerEmail);
            var orders = await orderRepo.GetAllWithSpecAsync(spec);
            return orders;

        }

        public async Task<Order?> GetOrderByIdForUserAsync(string BuyerEmail, int OrderId)
        {
            var orderRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecifications(BuyerEmail , OrderId);

            var orders = await orderRepo.GetByIdWithSpecAsync(spec);

            return orders;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
       => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        public async Task SaveOrderAsync(Order order)
        {
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
        }

    }
}
