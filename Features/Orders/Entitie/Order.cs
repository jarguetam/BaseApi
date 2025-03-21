using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.Orders.Entitie
{
	public class Order
	{
		public int Id { get; set; }
		public int DocNum { get; set; }
		public int DocEntry { get; set; }
		public DateTime DocDate { get; set; }
		public string CardCode { get; set; }
		public decimal DocTotal { get; set; }
		public string Reference { get; set; }
		public int CreatedBy { get; set; }
		public List<OrderDetail> Detail { get; set; }
		public Order()
		{
			Detail = new List<OrderDetail>();
		}

		public bool IsValid()
		{
			var existWithZero = Detail.Count(x => x.Quantity == 0);
			if (existWithZero > 0) throw new Exception("Cantidad debe ser mayor a 0");
			if (string.IsNullOrEmpty(this.CardCode)) throw new Exception("Debe seleccionar el proveedor");
			if (this.DocTotal == 0) throw new Exception("Total debe ser mayor a 0");
			return true;
		}

		public class Map
		{
			public Map(EntityTypeBuilder<Order> builder)
			{
				builder.HasKey(x => x.Id);
				builder.Property(x => x.DocNum).HasColumnName("DocNum");
				builder.Property(x => x.DocEntry).HasColumnName("DocEntry");
				builder.Property(x => x.DocDate).HasColumnName("DocDate");
				builder.Property(x => x.CardCode).HasColumnName("CardCode");
				builder.Property(x => x.DocTotal).HasColumnName("DocTotal");
				builder.Property(x => x.Reference).HasColumnName("Reference");
				builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
				builder.HasMany(x => x.Detail).WithOne(x => x.Order).HasForeignKey(x => x.IdOrder);
				builder.ToTable("Orders");
			}
		}
	}
}
