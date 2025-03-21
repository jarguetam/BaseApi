using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseApi.WebApi.Features.Users.Entities
{
    public class TypePermission
    {
        public int TypeId { get; set; }
        public string Description { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<TypePermission> builder)
            {
                builder.HasKey(x => x.TypeId);
                builder.Property(x => x.TypeId).HasColumnName("TypeId");
                builder.Property(x => x.Description).HasColumnName("Description");
                builder.ToTable("TypePermission");
            }
        }
    }
}
