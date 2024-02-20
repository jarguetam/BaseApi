using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.Orders.Entitie
{
    public class OrderDetail
    {
		public int IdDetail { get; set; }
		public int IdOrder { get; set; }
		public string ItemCode { get; set; }
		public string ItemName { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal LineTotal { get; set; }
		[JsonIgnore]
		public Order Order { get; set; }
		public class Map
		{
			public Map(EntityTypeBuilder<OrderDetail> builder)
			{
				builder.HasKey(x => x.IdDetail);
				builder.Property(x => x.IdOrder).HasColumnName("IdOrder");
				builder.Property(x => x.ItemCode).HasColumnName("ItemCode");
				builder.Property(x => x.ItemName).HasColumnName("ItemName");
				builder.Property(x => x.Quantity).HasColumnName("Quantity");
				builder.Property(x => x.Price).HasColumnName("Price");
				builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
				builder.HasOne(x => x.Order).WithMany(x => x.Detail).HasForeignKey(x => x.IdOrder);
				builder.ToTable("OrderDetail");
			}
		}
	}
}
