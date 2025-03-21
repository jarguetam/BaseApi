using ApiChatbot.WebApi.Features.Users.Entities;
using ApiChatbot.WebApi.Features.Common.Entities;
using Microsoft.EntityFrameworkCore;
using ApiChatbot.WebApi.Features.TypeDocuments.Entities;
using ApiChatbot.WebApi.Features.DataSellers.Entitie;

namespace ApiChatbot.WebApi.Infraestructure
{
    public class ApiChatbotDbContext : DbContext
    {
        public ApiChatbotDbContext(DbContextOptions<ApiChatbotDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Theme> Theme { get; set; }
        public DbSet<TypePermission> TypePermission { get; set; }
        public DbSet<TypeDocument> TypeDocument { get; set; }
        public DbSet<Seller> Seller { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new User.Map(modelBuilder.Entity<User>());
            new Permission.Map(modelBuilder.Entity<Permission>());
            new RolePermission.Map(modelBuilder.Entity<RolePermission>());
            new Role.Map(modelBuilder.Entity<Role>());
            new Theme.Map(modelBuilder.Entity<Theme>());
            new TypePermission.Map(modelBuilder.Entity<TypePermission>());
            new TypeDocument.Map(modelBuilder.Entity<TypeDocument>());
            new Seller.Map(modelBuilder.Entity<Seller>());
            base.OnModelCreating(modelBuilder);
        }
    }
}