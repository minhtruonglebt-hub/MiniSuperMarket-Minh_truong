# Hệ thống quản lý bán hàng và tồn kho Thiên Ân

Đề tài môn Lập trình Ứng dụng .NET Core: xây dựng hệ thống quản lý bán hàng cho Cửa hàng Bách hóa Tổng hợp Thiên Ân bằng ASP.NET Core Web API, Windows Forms và Entity Framework Core.

## Thành phần

- `MiniSuperMarket.API`: API .NET 8, EF Core, SQL Server, JWT và Swagger.
- `MiniSuperMarket.WinForm`: ứng dụng WinForms .NET 8 theo kiến trúc một cửa sổ, có sidebar và vùng hiển thị màn hình nghiệp vụ.
- `MiniSuperMarket.API/Data/Migrations`: migration tạo sáu bảng cùng dữ liệu mẫu.

## Chức năng Buổi 4

- Đăng nhập nhân viên từ bảng `Users`; mật khẩu demo được lưu bằng ASP.NET Core Identity PasswordHasher.
- Khung `FormMainShell` nhúng các màn hình con và áp dụng quyền theo vai trò.
- Admin: danh mục, sản phẩm, khách hàng, POS, báo cáo doanh thu và quản lý tài khoản.
- Cashier: POS và khách hàng.
- Warehouse: danh mục và sản phẩm/tồn kho.
- POS tra cứu barcode, nhận diện khách hàng, xác nhận tiền khách đưa, ghi hóa đơn, giảm tồn kho và cộng điểm thưởng.
- Báo cáo doanh thu chỉ dành cho Admin; API cũng chặn truy cập sai vai trò.

## Tài khoản mẫu

Tất cả tài khoản seed trong Buổi 4 dùng mật khẩu `123456`.

| Tài khoản | Vai trò |
|---|---|
| `admin01` | Admin |
| `cashier01` | Cashier |
| `ware01` | Warehouse |

Migration tạo 15 tài khoản, 15 nhóm hàng, 20 sản phẩm, 15 khách hàng, 15 hóa đơn và 30 dòng chi tiết hóa đơn.

## Cấu hình và khởi chạy

1. Cài .NET 8 SDK và SQL Server hoặc SQL Server Express.
2. Mở `MiniSuperMarket.API/appsettings.json`, chỉnh `ConnectionStrings:DefaultConnection` theo SQL Server trên máy.
3. Đặt khóa JWT dài ít nhất 32 byte trong user secrets hoặc biến môi trường `JwtSettings__Secret`. Ví dụ ở thư mục API:

   ```powershell
   dotnet user-secrets init
   dotnet user-secrets set "JwtSettings:Secret" "ThayBangChuoiBiMatDaiHon32ByteCuaBan"
   ```

4. Mở Package Manager Console trong Visual Studio, chọn `MiniSuperMarket.API` làm Default project rồi chạy:

   ```powershell
   Update-Database
   ```

   Hoặc dùng CLI từ thư mục solution:

   ```powershell
   dotnet ef database update --project MiniSuperMarket.API
   ```

   Migration Buổi 4 là migration khởi tạo toàn bộ sáu bảng. Nếu database Buổi 3 đã có các bảng `Categories`, `Products` hoặc `Customers` mà chưa có migration history tương ứng, hãy sao lưu dữ liệu trước và tạo database mới/đổi tên database trong connection string trước khi áp dụng migration này. Không chạy migration khởi tạo trên database đang có bảng cùng tên.

5. Mở solution trong Visual Studio và đặt API cùng WinForms chạy trong Multiple Startup Projects. API dùng HTTPS `https://localhost:7123`.
6. Đăng nhập bằng một tài khoản mẫu ở trên.

## API đáng chú ý

- `POST /api/auth/login`
- `GET /api/products/with-category`
- `GET /api/products/barcode/{barcode}`
- `POST /api/orders` (Admin/Cashier)
- `GET /api/reports/quick?date=2026-10-01` (Admin)
- `GET/POST /api/users` (Admin)

Swagger chỉ bật trong môi trường Development.

## Chủ đề và tác giả

Lê Minh Trường — MSSV 2124110110 — lớp CCQ2411D.
