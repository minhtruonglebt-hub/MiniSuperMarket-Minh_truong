# 🛒 HỆ THỐNG QUẢN LÝ BÁN HÀNG — CỬA HÀNG BÁCH HÓA TỔNG HỢP THIÊN ÂN

> **Môn học:** Lập trình Ứng dụng .NET Core (Mã môn: 229162)
> **Tiến độ:** Buổi 1 (CRUD) → Buổi 2 (JWT & phân quyền) → **Buổi 3 (SQL Server + EF Core Code-First)**

## 1. Kiến trúc (Client - Server)
* **`MiniSuperMarket.API`** (Backend): ASP.NET Core Web API (.NET 8), EF Core, JWT Bearer, Swagger.
* **`MiniSuperMarket.WinForm`** (Frontend): Windows Forms (.NET 8), `HttpClient` gọi API, tự gắn Bearer Token.

## 2. Cấu trúc Solution
```text
ThienAn-Supermarket/
├── MiniSuperMarket.API/
│   ├── Controllers/   AuthController, CategoriesController (EF Core), CustomersController (EF Core), RolesController (In-Memory)
│   ├── Models/        Category, Product, Customer, Role
│   ├── Data/          SupermarketDbContext (DbSet + Data Seeding)
│   ├── Program.cs     JWT + Swagger + AddDbContext
│   └── appsettings.json   ConnectionStrings + JwtSettings
└── MiniSuperMarket.WinForm/
    ├── FormLogin                 Đăng nhập, lưu token vào SessionManager
    ├── FormCategoryManagement    CRUD + Tìm kiếm nhóm hàng (mở được Khách hàng, Vai trò)
    ├── FormCustomerManagement    CRUD + Tìm kiếm khách hàng (Admin, Cashier)
    ├── FormRoleManagement        CRUD vai trò (chỉ Admin)
    ├── ApiClientService          HttpClient dùng chung + xử lý lỗi 401/403
    └── SessionManager            JwtToken, CurrentUsername, CurrentRole, ApiBaseUrl
```

## 3. Tạo Database (Buổi 3)
1. Kiểm tra chuỗi kết nối `DefaultConnection` trong `MiniSuperMarket.API/appsettings.json`.
   * Máy cá nhân (Windows Authentication): giữ nguyên mặc định.
   * Máy phòng thực hành dùng tài khoản `sa`: trong SSMS chạy `ALTER LOGIN sa WITH PASSWORD = '123456';` rồi đổi chuỗi thành:
     `Server=.;Database=ThienAnSupermarketDb;User Id=sa;Password=123456;MultipleActiveResultSets=true;TrustServerCertificate=True`
   * SQL Express: đổi `Server=.\\SQLEXPRESS`.
2. Mở **Tools → NuGet Package Manager → Package Manager Console**, chọn *Default project* = `MiniSuperMarket.API`, chạy:
   ```powershell
   Add-Migration InitialCreateDatabase
   Update-Database
   ```
   Kết quả: tạo CSDL `ThienAnSupermarketDb` với 3 bảng `Categories` (5 dòng mẫu), `Products`, `Customers` (3 dòng mẫu).
3. Nếu lỗi gói NuGet: `dotnet nuget locals all --clear` rồi restore lại.

## 4. Chạy và kiểm thử
1. Chuột phải Solution → *Configure Startup Projects* → chọn **Multiple startup projects**: API và WinForm đều *Start* (API chạy trước).
2. Swagger: `POST /api/auth/login` (`admin`/`123456` hoặc `cashier`/`123456`) → copy token → nút **Authorize** → dán `Bearer <token>`.
3. WinForm: đăng nhập, thử Thêm/Sửa/Xóa/Tìm kiếm nhóm hàng và khách hàng. Tắt cả hai ứng dụng rồi mở lại: dữ liệu vẫn còn.

| Chức năng | Admin | Cashier |
|---|---|---|
| Xem/Thêm/Sửa nhóm hàng | ✅ | ✅ |
| Xóa nhóm hàng | ✅ | ❌ 403 |
| Khách hàng (CRUD) | ✅ | ✅ |
| Vai trò (CRUD) | ✅ | ❌ 403 |

## 5. Tác giả
Họ tên sinh viên: Lê Minh Trường — Mã sinh viên: 2124110110 — Lớp học phần: CCQ2411D
