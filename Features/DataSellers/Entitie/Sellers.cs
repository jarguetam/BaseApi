using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ApiChatbot.WebApi.Features.DataSellers.Entitie
{
    public class Seller
    {
        public int Id { get; set; }
        public string CodigoVendedor { get; set; }
        public string NombreVendedor { get; set; }
        public string TelefonoVendedor { get; set; }
        public DateTime FechaConfirmacion { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Confirmado { get; set; }
        public bool Activo { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.CodigoVendedor)) throw new System.Exception("Debe ingresar un código de vendedor");
            if (string.IsNullOrEmpty(this.NombreVendedor)) throw new System.Exception("Debe ingresar un nombre de vendedor");
            if (string.IsNullOrEmpty(this.TelefonoVendedor)) throw new System.Exception("Debe ingresar un teléfono de vendedor");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Seller> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("Id");
                builder.Property(x => x.CodigoVendedor).HasColumnName("CodigoVendedor").HasMaxLength(10).IsRequired();
                builder.Property(x => x.NombreVendedor).HasColumnName("NombreVendedor").HasMaxLength(100).IsRequired();
                builder.Property(x => x.TelefonoVendedor).HasColumnName("TelefonoVendedor").HasMaxLength(10).IsRequired();
                builder.Property(x => x.FechaConfirmacion).HasColumnName("FechaConfirmacion").HasDefaultValueSql("getdate()");
                builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("getdate()");
                builder.Property(x => x.Confirmado).HasColumnName("Confirmado").HasDefaultValue(false);
                builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
                builder.ToTable("Sellers");
            }
        }
    }
}
