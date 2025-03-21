using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseApi.WebApi.Features.Users.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int ThemeId { get; set; }
        public bool Active { get; set; }


        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Name)) throw new System.Exception("Debe ingresar un nombre");
            if (string.IsNullOrEmpty(this.UserName)) throw new System.Exception("Debe ingresar un nombre");
            if (string.IsNullOrEmpty(this.Email)) throw new System.Exception("Debe ingresar un correo");
            if (this.RoleId ==0) throw new System.Exception("Debe ingresar un rol");
            if (this.ThemeId == 0) throw new System.Exception("Debe ingresar un tema");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<User> builder)
            {
                builder.HasKey(x => x.UserId);
                builder.Property(x => x.UserId).HasColumnName("UserId");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.Property(x => x.UserName).HasColumnName("UserName");
                builder.Property(x => x.Password).HasColumnName("Password");
                builder.Property(x => x.Email).HasColumnName("Email");
                builder.Property(x => x.RoleId).HasColumnName("RoleId");
                builder.Property(x => x.ThemeId).HasColumnName("ThemeId");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("User");
            }
        }
    }
}
