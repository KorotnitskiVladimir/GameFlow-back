using Microsoft.EntityFrameworkCore;

namespace GameFlow.Data;

public class DataContext: DbContext // создаем базу данных
{
    public DbSet<UserData> UsersData { get; private set; }
    public DbSet<UserRole> UserRoles { get; private set; }
    public DbSet<UserAccess> UserAccesses { get; private set; }
    public DbSet<AccessToken> AccessTokens { get; private set; }
    public DbSet<Category> Categories { get; private set; }
    
    public DbSet<Product> Products { get; private set; }

    public DataContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("GameFlow");

        modelBuilder.Entity<UserAccess>()
            .HasIndex(ua => ua.Login)
            .IsUnique();

        modelBuilder.Entity<UserAccess>()
            .HasOne(ua => ua.UserData)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .HasPrincipalKey(ud => ud.Id);

        modelBuilder.Entity<UserAccess>()
            .HasOne(ua => ua.UserRole)
            .WithMany()
            .HasForeignKey(ua => ua.RoleId);

        modelBuilder.Entity<AccessToken>()
            .HasKey(t => t.Jti);


        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany()
            .HasForeignKey(c => c.ParentId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .HasPrincipalKey(c => c.Id);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole()
            {
                Id = "guest", Description = "solely registered user", CanCreate = 0, CanRead = 0, CanUpdate = 0,
                CanDelete = 0
            },
            new UserRole()
            {
                Id = "editor", Description = "has authority to edit content", CanCreate = 0, CanRead = 1, CanUpdate = 1,
                CanDelete = 0
            },
            new UserRole()
            {
                Id = "admin", Description = "full access to DB", CanCreate = 1, CanRead = 1, CanUpdate = 1,
                CanDelete = 1
            },
            new UserRole()
            {
                Id = "moderator", Description = "has authority to block", CanCreate = 0, CanRead = 1, CanUpdate = 0,
                CanDelete = 1
            });
    }
}