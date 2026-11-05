# Hệ thống quản lý bán hàng và tồn kho Thiên Ân

> Đề tài môn **Lập trình Ứng dụng .NET Core**: xây dựng hệ thống quản lý bán hàng cho **Cửa hàng Bách hóa Tổng hợp Thiên Ân** bằng ASP.NET Core Web API, Windows Forms và Entity Framework Core.

> **Lưu ý:** các phần đánh dấu *(gợi ý)* là nội dung mở rộng mang tính tham khảo. Bạn hãy đối chiếu với mã nguồn thực tế (tên cột, tên lớp, đường dẫn thư mục) và chỉnh lại cho khớp trước khi nộp.

---

## Mục lục

1. [Giới thiệu và mục tiêu](#1-giới-thiệu-và-mục-tiêu)
2. [Công nghệ sử dụng](#2-công-nghệ-sử-dụng)
3. [Kiến trúc tổng thể](#3-kiến-trúc-tổng-thể)
4. [Cấu trúc thư mục (gợi ý)](#4-cấu-trúc-thư-mục-gợi-ý)
5. [Thiết kế cơ sở dữ liệu](#5-thiết-kế-cơ-sở-dữ-liệu)
6. [Thành phần của hệ thống](#6-thành-phần-của-hệ-thống)
7. [Chức năng Buổi 4](#7-chức-năng-buổi-4)
8. [Chức năng Buổi 5](#8-chức-năng-buổi-5)
9. [Phân quyền theo vai trò](#9-phân-quyền-theo-vai-trò)
10. [Luồng nghiệp vụ chính](#10-luồng-nghiệp-vụ-chính)
11. [Cấu hình và khởi chạy](#11-cấu-hình-và-khởi-chạy)
12. [Tài khoản mẫu](#12-tài-khoản-mẫu)
13. [Danh sách API](#13-danh-sách-api)
14. [Ví dụ gọi API (gợi ý)](#14-ví-dụ-gọi-api-gợi-ý)
15. [Bảo mật](#15-bảo-mật)
16. [Hướng dẫn sử dụng ứng dụng WinForms](#16-hướng-dẫn-sử-dụng-ứng-dụng-winforms)
17. [Kịch bản kiểm thử thủ công](#17-kịch-bản-kiểm-thử-thủ-công)
18. [Xử lý sự cố thường gặp](#18-xử-lý-sự-cố-thường-gặp)
19. [Câu hỏi thường gặp](#19-câu-hỏi-thường-gặp)
20. [Hạn chế và hướng phát triển](#20-hạn-chế-và-hướng-phát-triển)
21. [Quy ước làm việc với Git (gợi ý)](#21-quy-ước-làm-việc-với-git-gợi-ý)
22. [Tác giả](#22-tác-giả)

---

## 1. Giới thiệu và mục tiêu

Cửa hàng Bách hóa Tổng hợp Thiên Ân cần một hệ thống giúp nhân viên bán hàng nhanh, chính xác và giúp quản lý theo dõi được tồn kho, doanh thu. Dự án gồm một **máy chủ API** xử lý nghiệp vụ và dữ liệu, cùng một **ứng dụng desktop WinForms** dành cho nhân viên sử dụng hằng ngày.

**Mục tiêu học tập**

- Thiết kế và xây dựng Web API bằng ASP.NET Core (.NET 8).
- Làm việc với cơ sở dữ liệu bằng **Entity Framework Core** (Code First, Migration, Seed Data).
- Làm việc với **ADO.NET thuần** (connected và disconnected), **Stored Procedure** và **Transaction**.
- Xác thực bằng **JWT** và phân quyền theo vai trò (Role-based Authorization).
- Xây dựng ứng dụng WinForms theo mô hình một cửa sổ (shell) nhúng nhiều màn hình con.

**Mục tiêu nghiệp vụ**

- Bán hàng tại quầy (POS) nhanh bằng barcode.
- Quản lý danh mục, sản phẩm, tồn kho, khách hàng.
- Tích điểm thưởng cho khách hàng.
- Báo cáo doanh thu cho quản trị viên.
- Quản lý tài khoản và phân quyền nhân viên.

---

## 2. Công nghệ sử dụng

| Lớp | Công nghệ |
|---|---|
| Ngôn ngữ / Nền tảng | C# 12, .NET 8 |
| Web API | ASP.NET Core Web API |
| ORM | Entity Framework Core (Code First) |
| Truy cập dữ liệu thuần | ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter`, `DataTable`, `SqlTransaction`) |
| Cơ sở dữ liệu | SQL Server / SQL Server Express / LocalDB |
| Xác thực | JWT Bearer |
| Băm mật khẩu | ASP.NET Core Identity `PasswordHasher` |
| Tài liệu API | Swagger (chỉ môi trường Development) |
| Giao diện desktop | Windows Forms (.NET 8) |
| IDE khuyến nghị | Visual Studio 2022 trở lên |

---

## 3. Kiến trúc tổng thể

```
┌──────────────────────────┐        HTTPS + JWT         ┌────────────────────────────┐
│  MiniSuperMarket.WinForm │  ───────────────────────►  │   MiniSuperMarket.API      │
│  (FormMainShell + các    │  ◄───────────────────────  │  Controllers / Services    │
│   màn hình con)          │        JSON                │  EF Core + ADO.NET         │
└──────────────────────────┘                            └─────────────┬──────────────┘
                                                                      │
                                                                      ▼
                                                        ┌────────────────────────────┐
                                                        │ SQL Server                 │
                                                        │ ThienAnSupermarketDb       │
                                                        │ (6 bảng + 3 Stored Proc.)  │
                                                        └────────────────────────────┘
```

**Nguyên tắc thiết kế**

- WinForms **không truy cập trực tiếp** cơ sở dữ liệu; mọi thao tác đều đi qua API.
- Phân quyền được kiểm tra **hai lớp**: ẩn/hiện chức năng trên giao diện và chặn bằng `[Authorize(Roles = ...)]` ở API. Nhờ vậy dù ai đó gọi API trực tiếp cũng không vượt quyền được.
- Nghiệp vụ quan trọng (thanh toán, hủy hóa đơn) chạy trong **một transaction** để đảm bảo toàn vẹn dữ liệu.
- Hai cách truy cập dữ liệu cùng tồn tại: EF Core cho CRUD thông thường, ADO.NET cho các bài minh họa và nghiệp vụ cần kiểm soát chi tiết.

---

## 4. Cấu trúc thư mục (gợi ý)

```
ThienAnSupermarket/
├── MiniSuperMarket.sln
├── ThienAnSupermarketDb.sql              # Script SQL gồm cả 3 stored procedure
├── MiniSuperMarket.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── OrdersController.cs
│   │   ├── UsersController.cs
│   │   ├── AdoProductController.cs       # Minh họa ADO.NET
│   │   ├── CheckoutController.cs         # Thanh toán bằng SqlTransaction
│   │   └── ReportsController.cs          # Báo cáo, Stored Procedure
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/                   # Migration tạo 6 bảng + dữ liệu mẫu
│   ├── Models/ DTOs/ Services/
│   ├── appsettings.json
│   └── Program.cs
└── MiniSuperMarket.WinForm/
    ├── FormLogin.cs
    ├── FormMainShell.cs                  # Khung chính: sidebar + vùng nội dung
    ├── Screens/                          # UserControl cho từng nghiệp vụ
    │   ├── CategoryScreen / ProductScreen / CustomerScreen
    │   ├── PosScreen / RevenueReportScreen / UserScreen
    ├── Services/                         # Lớp gọi API (HttpClient)
    └── Program.cs
```

---

## 5. Thiết kế cơ sở dữ liệu

Cơ sở dữ liệu `ThienAnSupermarketDb` gồm **sáu bảng**. Buổi 5 không thêm bảng mới.

| Bảng | Vai trò | Dữ liệu mẫu |
|---|---|---|
| `Users` | Tài khoản nhân viên, vai trò, mật khẩu đã băm | 15 |
| `Categories` | Nhóm hàng | 15 |
| `Products` | Sản phẩm, barcode, giá, tồn kho | 20 |
| `Customers` | Khách hàng, điểm thưởng | 15 |
| `Orders` | Hóa đơn (đầu phiếu) | 15 |
| `OrderDetails` | Chi tiết hóa đơn (từng dòng sản phẩm) | 30 |

### Quan hệ giữa các bảng

```
Categories 1 ──── n Products
Customers  1 ──── n Orders
Users      1 ──── n Orders          (nhân viên lập hóa đơn)
Orders     1 ──── n OrderDetails
Products   1 ──── n OrderDetails
```

### Ràng buộc nghiệp vụ nên có *(gợi ý)*

- `Products.Barcode` là duy nhất.
- `Users.Username` là duy nhất.
- Tồn kho (`Stock`) không âm.
- Số lượng và đơn giá trong `OrderDetails` lớn hơn 0.
- Đơn giá được **sao chép tại thời điểm bán** vào chi tiết hóa đơn để thay đổi giá sau này không làm sai hóa đơn cũ.

### Ba Stored Procedure (Buổi 5)

| Stored Procedure | Mục đích | API sử dụng |
|---|---|---|
| `sp_GetRevenueByCashier` | Doanh thu theo thu ngân | `/api/reports/revenue-by-cashier` và `/range` |
| Báo cáo theo khoảng ngày | Doanh thu trong khoảng thời gian | `/api/reports/revenue-by-cashier/range` |
| Top 5 sản phẩm bán chạy | Thống kê sản phẩm | `/api/reports/top-selling-products` |

---

## 6. Thành phần của hệ thống

- **MiniSuperMarket.API**: API .NET 8, EF Core, SQL Server, JWT và Swagger.
- **MiniSuperMarket.WinForm**: ứng dụng WinForms .NET 8 theo kiến trúc một cửa sổ, có sidebar và vùng hiển thị màn hình nghiệp vụ.
- **MiniSuperMarket.API/Data/Migrations**: migration tạo sáu bảng cùng dữ liệu mẫu.

---

## 7. Chức năng Buổi 4

- Đăng nhập nhân viên từ bảng `Users`; mật khẩu demo được lưu bằng ASP.NET Core Identity `PasswordHasher`.
- Khung `FormMainShell` nhúng các màn hình con và áp dụng quyền theo vai trò.
- **Admin**: danh mục, sản phẩm, khách hàng, POS, báo cáo doanh thu và quản lý tài khoản.
- **Cashier**: POS và khách hàng.
- **Warehouse**: danh mục và sản phẩm/tồn kho.
- POS tra cứu barcode, nhận diện khách hàng, xác nhận tiền khách đưa, ghi hóa đơn, giảm tồn kho và cộng điểm thưởng.
- Báo cáo doanh thu chỉ dành cho Admin; API cũng chặn truy cập sai vai trò.

---

## 8. Chức năng Buổi 5

- `AdoProductController` minh họa `ExecuteScalar`, `ExecuteNonQuery`, `SqlDataReader`, `SqlParameter`, `SqlDataAdapter` và `DataTable`.
- API ADO.NET tra cứu barcode, xem snapshot disconnected, lọc hàng tồn thấp, đếm sản phẩm theo nhóm và cập nhật tồn kho.
- `ReportsController` chạy `sp_GetRevenueByCashier`, báo cáo theo khoảng ngày và top 5 sản phẩm bán chạy.
- POS dùng `POST /api/checkout`: một `SqlTransaction` ghi hóa đơn/chi tiết, trừ tồn kho và cộng điểm; lỗi thiếu hàng kích hoạt rollback toàn bộ.
- `DELETE /api/checkout/cancel/{orderId}` (Admin) hủy hóa đơn, hoàn kho và điều chỉnh điểm trong transaction.
- Không thêm bảng mới; Buổi 5 dùng lại sáu bảng của Buổi 4. Ba stored procedure đã được thêm vào `ThienAnSupermarketDb.sql`.

### So sánh connected và disconnected trong ADO.NET

| Tiêu chí | Connected (`SqlDataReader`) | Disconnected (`SqlDataAdapter` + `DataTable`) |
|---|---|---|
| Kết nối | Giữ mở trong lúc đọc | Mở rồi đóng ngay sau khi nạp dữ liệu |
| Truy cập | Chỉ tiến, chỉ đọc | Truy cập ngẫu nhiên, có thể sửa trong bộ nhớ |
| Bộ nhớ | Ít | Nhiều hơn (giữ toàn bộ dữ liệu) |
| Phù hợp | Đọc nhanh tập kết quả lớn | Cache, lọc/sắp xếp tại chỗ, thao tác nhiều lần |
| Trong dự án | Tra cứu barcode, lọc tồn thấp | `/disconnected-cache` |

---

## 9. Phân quyền theo vai trò

| Chức năng | Admin | Cashier | Warehouse |
|---|:---:|:---:|:---:|
| Đăng nhập | ✅ | ✅ | ✅ |
| Danh mục | ✅ | ❌ | ✅ |
| Sản phẩm / tồn kho | ✅ | ❌ | ✅ |
| Khách hàng | ✅ | ✅ | ❌ |
| POS (bán hàng) | ✅ | ✅ | ❌ |
| Hủy hóa đơn | ✅ | ❌ | ❌ |
| Báo cáo doanh thu | ✅ | ❌ | ❌ |
| Quản lý tài khoản | ✅ | ❌ | ❌ |

Khi gọi API sai vai trò, hệ thống trả về **401** (chưa đăng nhập / token không hợp lệ) hoặc **403** (đã đăng nhập nhưng không đủ quyền).

---

## 10. Luồng nghiệp vụ chính

### 10.1. Đăng nhập

1. Nhân viên nhập tên đăng nhập và mật khẩu trên `FormLogin`.
2. WinForms gọi `POST /api/auth/login`.
3. API tìm người dùng, kiểm tra mật khẩu bằng `PasswordHasher.VerifyHashedPassword`.
4. Nếu đúng, API phát hành JWT chứa tên đăng nhập và vai trò.
5. WinForms lưu token trong bộ nhớ và đính kèm vào header `Authorization: Bearer <token>` ở các lần gọi sau.
6. `FormMainShell` đọc vai trò để hiển thị đúng các mục trên sidebar.

### 10.2. Bán hàng tại POS

1. Thu ngân quét hoặc nhập barcode, API trả thông tin sản phẩm.
2. Sản phẩm được thêm vào giỏ; trùng barcode thì tăng số lượng.
3. (Tuỳ chọn) nhập số điện thoại để nhận diện khách hàng và tích điểm.
4. Hệ thống tính tổng tiền; thu ngân nhập số tiền khách đưa, hệ thống tính tiền thối.
5. Bấm thanh toán, ứng dụng gọi `POST /api/checkout`.
6. API mở **một transaction** và thực hiện tuần tự:
   - Kiểm tra tồn kho từng sản phẩm.
   - Ghi đầu hóa đơn và các dòng chi tiết.
   - Trừ tồn kho.
   - Cộng điểm thưởng cho khách.
   - `Commit`.
7. Nếu bất kỳ bước nào lỗi (ví dụ thiếu hàng) thì `Rollback`, không có dữ liệu nào bị ghi dở.

### 10.3. Hủy hóa đơn (Admin)

1. Admin gọi `DELETE /api/checkout/cancel/{orderId}`.
2. Trong một transaction: hoàn lại tồn kho theo từng dòng chi tiết, điều chỉnh lại điểm thưởng của khách, cập nhật/xóa hóa đơn theo quy ước của dự án.
3. Lỗi giữa chừng thì rollback toàn bộ.

### 10.4. Báo cáo doanh thu (Admin)

- Xem nhanh theo ngày: `GET /api/reports/quick?date=...`.
- Doanh thu theo thu ngân, theo khoảng ngày (Stored Procedure).
- Top 5 sản phẩm bán chạy (Stored Procedure).

---

## 11. Cấu hình và khởi chạy

### Yêu cầu

- .NET 8 SDK.
- SQL Server hoặc SQL Server Express (hoặc dùng LocalDB đi kèm Visual Studio).
- Visual Studio 2022 (workload *.NET desktop development* và *ASP.NET and web development*).
- Công cụ EF Core CLI nếu dùng dòng lệnh: `dotnet tool install --global dotnet-ef`.

### Bước 1. Chuỗi kết nối

Mặc định `MiniSuperMarket.API/appsettings.json` dùng SQL Server LocalDB `(localdb)\\MSSQLLocalDB`. Nếu dùng SQL Server khác, sửa `ConnectionStrings:DefaultConnection`, ví dụ:

```
Server=localhost;Database=ThienAnSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Với SQL Express, thay `localhost` bằng `.` (hoặc `.\SQLEXPRESS`).

### Bước 2. Khóa JWT

Đặt khóa JWT dài ít nhất 32 byte trong user secrets hoặc biến môi trường `JwtSettings__Secret`. Dự án đã có `UserSecretsId`; tại thư mục API, chạy:

```bash
dotnet user-secrets set "JwtSettings:Secret" "ThayBangChuoiBiMatDaiHon32ByteCuaBan"
```

### Bước 3. Tạo cơ sở dữ liệu

Trong Visual Studio, mở Package Manager Console, chọn `MiniSuperMarket.API` làm Default project rồi chạy:

```
Update-Database
```

Hoặc dùng CLI từ thư mục solution:

```bash
dotnet ef database update --project MiniSuperMarket.API
```

> **Cảnh báo về migration:** Migration Buổi 4 là migration khởi tạo toàn bộ sáu bảng. Nếu database Buổi 3 đã có các bảng `Categories`, `Products` hoặc `Customers` mà chưa có migration history tương ứng, hãy **sao lưu dữ liệu trước** và tạo database mới/đổi tên database trong connection string trước khi áp dụng migration này. **Không chạy migration khởi tạo trên database đang có bảng cùng tên.**

### Bước 4. Nạp Stored Procedure (nếu chưa có)

Mở `ThienAnSupermarketDb.sql` trong SSMS hoặc Azure Data Studio và chạy phần tạo ba stored procedure của Buổi 5 trên database `ThienAnSupermarketDb`.

### Bước 5. Chạy ứng dụng

1. Mở solution trong Visual Studio.
2. Chuột phải solution → *Configure Startup Projects* → **Multiple startup projects**, đặt API và WinForms đều là *Start*, (API nên khởi động trước).
3. API chạy tại HTTPS `https://localhost:7123`.
4. Đăng nhập bằng một tài khoản mẫu bên dưới.

---

## 12. Tài khoản mẫu

Tất cả tài khoản seed trong Buổi 4 dùng mật khẩu `123456`.

| Tài khoản | Vai trò |
|---|---|
| `admin01` | Admin |
| `cashier01` | Cashier |
| `ware01` | Warehouse |

Migration tạo 15 tài khoản, 15 nhóm hàng, 20 sản phẩm, 15 khách hàng, 15 hóa đơn và 30 dòng chi tiết hóa đơn.

> Mật khẩu `123456` chỉ phục vụ demo. **Không dùng cho môi trường thật.**

---

## 13. Danh sách API

| Phương thức | Đường dẫn | Quyền | Ghi chú |
|---|---|---|---|
| POST | `/api/auth/login` | Công khai | Đăng nhập, nhận JWT |
| GET | `/api/products/with-category` | Đã đăng nhập | Sản phẩm kèm tên nhóm hàng |
| GET | `/api/products/barcode/{barcode}` | Đã đăng nhập | Tra cứu theo barcode |
| POST | `/api/orders` | Admin, Cashier | Tạo hóa đơn |
| GET | `/api/ado-products/barcode/{barcode}` | Theo cấu hình | ADO.NET |
| GET | `/api/ado-products/disconnected-cache` | Theo cấu hình | ADO.NET, snapshot disconnected |
| GET | `/api/ado-products/filter-stock?minStock=50` | Theo cấu hình | Lọc hàng tồn thấp |
| GET | `/api/ado-products/category-counts` | Theo cấu hình | Đếm sản phẩm theo nhóm |
| POST | `/api/checkout` | Admin, Cashier | Thanh toán bằng transaction |
| DELETE | `/api/checkout/cancel/{orderId}` | Admin | Hủy hóa đơn, hoàn kho |
| GET | `/api/reports/quick?date=2026-10-01` | Admin | Báo cáo nhanh theo ngày |
| GET | `/api/reports/revenue-by-cashier` | Admin | Stored Procedure |
| GET | `/api/reports/revenue-by-cashier/range` | Admin | Stored Procedure, theo khoảng ngày |
| GET | `/api/reports/top-selling-products` | Admin | Stored Procedure, top 5 |
| GET / POST | `/api/users` | Admin | Quản lý tài khoản |

Swagger chỉ bật trong môi trường Development.

---

## 14. Ví dụ gọi API (gợi ý)

> Tên trường JSON có thể khác với dự án của bạn; hãy xem Swagger để lấy đúng schema.

### Đăng nhập

```http
POST https://localhost:7123/api/auth/login
Content-Type: application/json

{
  "username": "admin01",
  "password": "123456"
}
```

Phản hồi mẫu:

```json
{
  "token": "eyJhbGciOi...",
  "username": "admin01",
  "role": "Admin"
}
```

### Tra cứu sản phẩm theo barcode

```http
GET https://localhost:7123/api/products/barcode/8934567890123
Authorization: Bearer <token>
```

### Thanh toán

```http
POST https://localhost:7123/api/checkout
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": 3,
  "amountPaid": 200000,
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 5, "quantity": 1 }
  ]
}
```

Trường hợp thiếu hàng, API trả lỗi (ví dụ `400 Bad Request`) kèm thông báo và **không** thay đổi dữ liệu nào.

### Hủy hóa đơn

```http
DELETE https://localhost:7123/api/checkout/cancel/15
Authorization: Bearer <token của Admin>
```

### Báo cáo theo khoảng ngày

```http
GET https://localhost:7123/api/reports/revenue-by-cashier/range?from=2026-10-01&to=2026-10-31
Authorization: Bearer <token của Admin>
```

### Dùng Swagger

1. Chạy API ở môi trường Development, mở `https://localhost:7123/swagger`.
2. Gọi `/api/auth/login`, sao chép giá trị `token`.
3. Bấm **Authorize**, nhập `Bearer <token>` (hoặc chỉ `<token>` tuỳ cấu hình).
4. Thử các endpoint.

---

## 15. Bảo mật

- **Băm mật khẩu**: dùng `PasswordHasher` của ASP.NET Core Identity (PBKDF2 kèm salt), không lưu mật khẩu dạng thô.
- **JWT**: ký bằng khóa tối thiểu 32 byte; khóa lưu trong user secrets hoặc biến môi trường, **không commit lên Git**.
- **Phân quyền ở API**: dùng `[Authorize(Roles = "...")]` nên không thể vượt quyền bằng cách gọi trực tiếp.
- **Chống SQL Injection**: mọi câu lệnh ADO.NET đều dùng `SqlParameter`, không ghép chuỗi từ dữ liệu người dùng.
- **HTTPS**: API chạy qua HTTPS.
- **Swagger** chỉ bật ở Development để tránh lộ mô tả API ở môi trường thật.
- **Khuyến nghị thêm** *(gợi ý)*: giới hạn số lần đăng nhập sai, đặt thời gian hết hạn token hợp lý, ghi nhật ký thao tác nhạy cảm (hủy hóa đơn, đổi quyền), bắt buộc đổi mật khẩu mặc định ở lần đăng nhập đầu.

---

## 16. Hướng dẫn sử dụng ứng dụng WinForms

### Màn hình đăng nhập

Nhập tên đăng nhập và mật khẩu rồi bấm **Đăng nhập**. Sai thông tin sẽ có thông báo lỗi; đúng thì mở `FormMainShell`.

### Khung chính (`FormMainShell`)

- **Sidebar bên trái**: các mục chức năng, chỉ hiện những mục mà vai trò được phép.
- **Vùng nội dung bên phải**: nơi nhúng màn hình con tương ứng.
- Chuyển mục là thay màn hình con, không mở cửa sổ mới.

### Danh mục và sản phẩm *(Admin, Warehouse)*

Thêm, sửa, xóa, tìm kiếm nhóm hàng và sản phẩm; theo dõi tồn kho, nhận biết hàng sắp hết.

### Khách hàng *(Admin, Cashier)*

Quản lý thông tin khách hàng và xem điểm thưởng tích lũy.

### POS *(Admin, Cashier)*

Quét/nhập barcode → thêm vào giỏ → chọn khách hàng → nhập tiền khách đưa → thanh toán. Xem mục [10.2](#102-bán-hàng-tại-pos).

### Báo cáo doanh thu *(Admin)*

Chọn ngày hoặc khoảng ngày để xem doanh thu theo thu ngân và danh sách top 5 sản phẩm bán chạy.

### Quản lý tài khoản *(Admin)*

Xem danh sách và tạo tài khoản nhân viên, gán vai trò.

---

## 17. Kịch bản kiểm thử thủ công

| # | Kịch bản | Kết quả mong đợi |
|---|---|---|
| 1 | Đăng nhập `admin01` / `123456` | Vào được shell, thấy đủ mục chức năng |
| 2 | Đăng nhập sai mật khẩu | Thông báo lỗi, không vào được |
| 3 | Đăng nhập `cashier01` | Chỉ thấy POS và Khách hàng |
| 4 | Đăng nhập `ware01` | Chỉ thấy Danh mục và Sản phẩm |
| 5 | Cashier gọi `/api/reports/quick` | `403 Forbidden` |
| 6 | Quét barcode hợp lệ ở POS | Hiện đúng sản phẩm và giá |
| 7 | Quét barcode không tồn tại | Thông báo không tìm thấy |
| 8 | Thanh toán đủ hàng | Có hóa đơn, tồn giảm, điểm khách tăng |
| 9 | Thanh toán với số lượng vượt tồn | Báo lỗi, rollback, tồn kho **không đổi** |
| 10 | Tiền khách đưa nhỏ hơn tổng tiền | Không cho thanh toán |
| 11 | Admin hủy hóa đơn | Tồn kho hoàn lại, điểm điều chỉnh |
| 12 | Cashier hủy hóa đơn | `403 Forbidden` |
| 13 | Gọi API không kèm token | `401 Unauthorized` |
| 14 | Xem báo cáo theo khoảng ngày | Số liệu khớp dữ liệu hóa đơn |
| 15 | Top 5 sản phẩm bán chạy | Đúng thứ tự giảm dần theo số lượng bán |

---

## 18. Xử lý sự cố thường gặp

| Hiện tượng | Nguyên nhân có thể | Cách xử lý |
|---|---|---|
| `Update-Database` báo bảng đã tồn tại | Database cũ đã có bảng cùng tên | Sao lưu rồi đổi tên database trong connection string |
| Không kết nối được SQL Server | Sai chuỗi kết nối hoặc dịch vụ chưa chạy | Kiểm tra `appsettings.json`, bật dịch vụ SQL Server |
| LocalDB không khởi động | Chưa cài LocalDB | Cài qua Visual Studio Installer hoặc đổi sang SQL Express |
| API báo thiếu `JwtSettings:Secret` | Chưa đặt user secrets | Chạy lệnh `dotnet user-secrets set` |
| Lỗi khóa JWT quá ngắn | Khóa dưới 32 byte | Dùng chuỗi dài hơn |
| WinForms không gọi được API | API chưa chạy hoặc sai cổng | Chạy Multiple Startup Projects, kiểm tra `https://localhost:7123` |
| Lỗi chứng chỉ HTTPS | Chưa tin cậy chứng chỉ dev | Chạy `dotnet dev-certs https --trust` |
| Đăng nhập luôn thất bại | Chưa chạy migration/seed | Chạy lại `Update-Database` |
| Báo cáo lỗi `Could not find stored procedure` | Chưa tạo stored procedure | Chạy phần SP trong `ThienAnSupermarketDb.sql` |
| `403` dù đã đăng nhập | Vai trò không đủ quyền | Đăng nhập bằng tài khoản đúng vai trò |

---

## 19. Câu hỏi thường gặp

**Vì sao vừa dùng EF Core vừa dùng ADO.NET?**
EF Core tiện cho CRUD; ADO.NET dùng để học và minh họa kiểm soát chi tiết, hiệu năng, stored procedure và transaction.

**Vì sao thanh toán phải dùng transaction?**
Một hóa đơn gồm nhiều thao tác (ghi đầu phiếu, chi tiết, trừ kho, cộng điểm). Nếu một bước lỗi mà các bước trước đã ghi thì dữ liệu sẽ sai. Transaction đảm bảo hoặc tất cả thành công hoặc không có gì thay đổi.

**Vì sao đã ẩn menu trên WinForms mà API vẫn phải kiểm tra quyền?**
Giao diện chỉ là lớp tiện dụng. Người dùng có thể gọi API trực tiếp, nên quyền phải được thực thi tại máy chủ.

**Có thể dùng database khác SQL Server không?**
Mã ADO.NET và stored procedure của dự án gắn với SQL Server, nên cần chỉnh lại nếu đổi hệ quản trị.

**Quên mật khẩu tài khoản mẫu?**
Mật khẩu seed là `123456`. Nếu đã đổi, hãy tạo lại tài khoản bằng Admin hoặc chạy lại migration trên database mới.

---

## 20. Hạn chế và hướng phát triển

**Hạn chế hiện tại**

- Chưa có refresh token; token hết hạn phải đăng nhập lại.
- Chưa in hóa đơn, chưa tích hợp máy quét barcode phần cứng và máy in nhiệt.
- Chưa có nhập hàng / phiếu nhập kho và kiểm kê.
- Báo cáo mới dừng ở doanh thu theo thu ngân và top sản phẩm.

**Hướng phát triển**

1. Phiếu nhập kho, nhà cung cấp, cảnh báo hàng sắp hết hạn/tồn thấp.
2. Xuất hóa đơn PDF, in hóa đơn nhiệt.
3. Khuyến mãi, giảm giá theo hạng thành viên, đổi điểm thưởng.
4. Báo cáo biểu đồ (doanh thu theo ngày/tháng, lợi nhuận, hàng bán chậm).
5. Refresh token, nhật ký hoạt động (audit log), khóa tài khoản khi đăng nhập sai nhiều lần.
6. Viết unit test và integration test cho dịch vụ thanh toán.
7. Đóng gói bằng Docker, triển khai API lên máy chủ và CI/CD bằng GitHub Actions.
8. Phiên bản web hoặc mobile cho quản lý xem báo cáo từ xa.

---

## 21. Quy ước làm việc với Git (gợi ý)

- Nhánh: `main` (ổn định), `feature/<tên-chức-năng>` cho từng chức năng.
- Commit rõ ràng, ví dụ: `feat: thêm API hủy hóa đơn`, `fix: sửa lỗi trừ tồn kho`, `docs: cập nhật README`.
- Không commit: `appsettings.Development.json` chứa bí mật, user secrets, thư mục `bin/`, `obj/`, `.vs/`.
- Gợi ý `.gitignore` chuẩn cho Visual Studio / .NET.

---

## 22. Tác giả

**Lê Minh Trường** — MSSV 2124110110 — lớp CCQ2411D.

Đề tài môn Lập trình Ứng dụng .NET Core.
