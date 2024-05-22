using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Config.Order_Config
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());

            builder.Property(o => o.Status)
                   .HasConversion
                   (
                        (OStatus) => OStatus.ToString(),
                        (OStatus) => (OrderStatus) Enum.Parse(typeof(OrderStatus) , OStatus)
                   );

           
            builder.Property(O => O.Subtotal)
                .HasColumnType("decimal(12,2)");

            builder.HasOne(order => order.DeliveryMethod)
                 .WithMany().OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(order => order.Items)
              .WithOne().OnDelete(DeleteBehavior.Cascade);

        }
    }
}
