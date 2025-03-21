using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseApi.WebApi.Features.Common.Entities
{
    public class Theme
    {
        public int ThemeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<Theme> builder)
            {
                builder.HasKey(x => x.ThemeId);
                builder.Property(x => x.ThemeId).HasColumnName("ThemeId");
                builder.Property(x => x.Code).HasColumnName("Code");
                builder.Property(x => x.Description).HasColumnName("Description");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("Theme");
            }
        }
    }
}
