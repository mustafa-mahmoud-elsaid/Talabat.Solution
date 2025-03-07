using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data.Configurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(O => O.ShippingAddress, X => X.WithOwner());

            builder.Property(O => O.Status)
                .HasConversion(
                OrderStatus => OrderStatus.ToString(),
                OrderStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), OrderStatus)
                );
            builder.HasOne(O => O.DeliveryMethod).WithMany()
                .OnDelete(DeleteBehavior.SetNull);
            builder.Property(O => O.Subtotal).HasColumnType("decimal(18, 2)");

            builder.HasMany(O=>O.OrderItem)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
