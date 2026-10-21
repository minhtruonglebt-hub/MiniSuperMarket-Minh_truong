using Microsoft.EntityFrameworkCore;
using MiniSuperMarket.API.Models;

namespace MiniSuperMarket.API.Data
{
    // DbContext đại diện cho phiên làm việc với cơ sở dữ liệu SQL Server
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options) { }

        // Các bảng dữ liệu ánh xạ từ Model
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Categories: nạp sẵn 5 nhóm hàng (Data Seeding) =====
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Bánh kẹo & Đồ ăn vặt", Description = "Snack, bánh quy, kẹo dẻo" },
                new Category { CategoryId = 2, CategoryName = "Nước giải khát & Trà", Description = "Nước ngọt, nước khoáng, trà" },
                new Category { CategoryId = 3, CategoryName = "Sữa & Sản phẩm từ sữa", Description = "Sữa tươi, sữa chua, phô mai" },
                new Category { CategoryId = 4, CategoryName = "Mì gói & Thực phẩm ăn liền", Description = "Mì ăn liền, phở khô, cháo gói" },
                new Category { CategoryId = 5, CategoryName = "Gia vị & Dầu ăn", Description = "Nước mắm, hạt nêm, dầu thực vật" }
            );

            // ===== Products: mã vạch không trùng, không cho xóa nhóm hàng còn sản phẩm =====
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Barcode).IsUnique();
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Customers: giá trị mặc định, SĐT không trùng, 3 khách hàng mẫu =====
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.RewardPoints).HasDefaultValue(0);
                entity.Property(c => c.MembershipRank).HasDefaultValue("Chuẩn");
                entity.HasIndex(c => c.PhoneNumber).IsUnique();

                entity.HasData(
                    new Customer { CustomerId = 1, CustomerName = "Nguyễn Văn A", PhoneNumber = "0901122334", MembershipRank = "Vàng", RewardPoints = 150 },
                    new Customer { CustomerId = 2, CustomerName = "Trần Thị B", PhoneNumber = "0918877665", MembershipRank = "Bạc", RewardPoints = 50 },
                    new Customer { CustomerId = 3, CustomerName = "Lê Văn C", PhoneNumber = "0983344556", MembershipRank = "Chuẩn", RewardPoints = 10 }
                );
            });
        }
    }
}
