using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSuperMarket.API.Models
{
    // Thực thể Khách hàng thân thiết (bảng Customers) - Bài tập mở rộng Buổi 3
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống!")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [StringLength(15)]
        [Column(TypeName = "varchar(15)")]
        [RegularExpression(@"^\d{9,15}$", ErrorMessage = "Số điện thoại chỉ gồm từ 9 đến 15 chữ số")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Điểm tích lũy không được âm")]
        public int RewardPoints { get; set; } = 0; // Mặc định 0

        [StringLength(50)]
        public string MembershipRank { get; set; } = "Chuẩn"; // Mặc định 'Chuẩn'
    }
}
