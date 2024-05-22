using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Specs
{
    public class OrderSpecifications :BaseSpecifications<Order>
    {
        public OrderSpecifications(string buyerEmail) 
            :base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
            AddOrderByDesc(o => o.OrderDate);
        }

        public OrderSpecifications(string buyerEmail , int id)
         : base(O => O.BuyerEmail == buyerEmail && O.Id == id)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
   
        }
     
    }
}
