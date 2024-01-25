using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseApi.WebApi.Features.Users.Entities
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int FatherId { get; set; }
        public int TypeId { get; set; }
        public bool Active { get; set; }

        public int Position { get; set; }


        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Path)) throw new System.Exception("Debe ingresar una ruta");
            if (string.IsNullOrEmpty(this.Description)) throw new System.Exception("Debe ingresar una descripción");
            if (string.IsNullOrEmpty(this.Icon)) throw new System.Exception("Debe ingresar un icono");
            if ((this.Position) ==0) throw new System.Exception("Debe ingresar la posicion");
            if (this.TypeId == 0) throw new System.Exception("Debe ingresar un tipo");
            return true;
        }
        public class Map
        {
            public Map(EntityTypeBuilder<Permission> builder)
            {
                builder.HasKey(x => x.PermissionId);
                builder.Property(x => x.PermissionId).HasColumnName("PermissionId");
                builder.Property(x => x.Path).HasColumnName("Path");
                builder.Property(x => x.Description).HasColumnName("Description");
                builder.Property(x => x.Icon).HasColumnName("Icon");
                builder.Property(x => x.FatherId).HasColumnName("FatherId");
                builder.Property(x => x.TypeId).HasColumnName("TypeId");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.Property(x => x.Position).HasColumnName("Position");
                builder.ToTable("Permission");
            }
        }
    }
}
