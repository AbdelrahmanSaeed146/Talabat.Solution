using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;


namespace Talabat.Core.Services
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(/*string BuyerEmail,*/ string BasketId, int DeliveryMethodId, Adsress shippingaddress);

        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string BuyerEmail);
        Task<Order?> GetOrderByIdForUserAsync(string BuyerEmail , int OrderId);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();

        Task SaveOrderAsync(Order order);



    }
}
