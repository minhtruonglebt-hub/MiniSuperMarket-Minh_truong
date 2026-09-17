namespace MiniSuperMarket.WinForm.Models
{
    // Lớp biểu diễn thực thể Nhóm hàng hóa - tương ứng với model bên MiniSuperMarket.API
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
