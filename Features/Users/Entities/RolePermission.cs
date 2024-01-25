using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseApi.WebApi.Features.Users.Entities
{
    public class RolePermission
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool Active { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<RolePermission> builder)
            {
                builder.HasKey(x => x.RolePermissionId);
                builder.Property(x => x.RolePermissionId).HasColumnName("RolePermissionId");
                builder.Property(x => x.RoleId).HasColumnName("RoleId");
                builder.Property(x => x.PermissionId).HasColumnName("PermissionId");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("RolePermission");
            }
        }
    }
}
