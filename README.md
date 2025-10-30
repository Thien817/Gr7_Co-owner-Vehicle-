# Co-owner Vehicle Management System

## 🚗 Hệ thống quản lý đồng sở hữu xe

Hệ thống quản lý đồng sở hữu xe cho phép nhiều người cùng sở hữu và sử dụng một chiếc xe, với các tính năng quản lý lịch đặt xe, chi phí, thanh toán và báo cáo tài chính.

## 🛠️ Công nghệ sử dụng

- **.NET 8.0** - Framework chính
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **Bootstrap** - UI framework

## 📋 Yêu cầu hệ thống

- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code

## 🚀 Cài đặt và chạy dự án

### 1. Clone repository
```bash
git clone <repository-url>
cd "Co-owner Vehicle"
```

### 2. Cài đặt dependencies
```bash
dotnet restore
```

### 3. Cấu hình database
Chỉnh sửa connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CoOwnerVehicleDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Migration và tạo database với dữ liệu mẫu

**🎯 2 LỆNH QUAN TRỌNG (CHẠY ĐƯỢC TRÊN TẤT CẢ MÁY):**

```bash
# Chuyển đến thư mục project (thay đổi đường dẫn theo máy của bạn)
cd "Co-owner Vehicle"

# Lệnh 1: Tạo migration (nếu có thay đổi model)
dotnet ef migrations add InitialCreate

# Lệnh 2: Update database (TỰ ĐỘNG SEED DỮ LIỆU)
dotnet ef database update
```

**📝 Lưu ý:** 
- Thay đổi đường dẫn `cd` theo thư mục project trên máy của bạn
- **Migration đã được tích hợp sẵn seed data** - chỉ cần chạy 2 lệnh trên là có đầy đủ dữ liệu
- **Không cần chạy ứng dụng** để seed dữ liệu - migration tự động làm việc này
- Chạy được trên mọi máy Windows/Linux/macOS
- **Encoding tiếng Việt:** Migration đã được tối ưu để hiển thị đúng font chữ tiếng Việt

### 5. Chạy ứng dụng
```bash
dotnet run
```

Truy cập: `https://localhost:7000` hoặc `http://localhost:5000`

## 👥 Tài khoản đăng nhập mặc định

### Admin
- **Email:** `admin@coowner.com`
- **Password:** `admin123`
- **Quyền:** Quản trị viên hệ thống

### Staff
- **Email:** `staff@coowner.com`
- **Password:** `staff123`
- **Quyền:** Nhân viên vận hành

### Co-owners (Đồng sở hữu)
- **Email:** `nguyenvana@email.com` | **Password:** `user123`
- **Email:** `tranthib@email.com` | **Password:** `user123`
- **Email:** `levanc@email.com` | **Password:** `user123`
- **Email:** `phamthid@email.com` | **Password:** `user123`
- **Email:** `hoangvane@email.com` | **Password:** `user123`
- **Email:** `vothif@email.com` | **Password:** `user123`

## 🚗 Dữ liệu mẫu

Sau khi chạy migration, hệ thống sẽ tự động tạo:

### Vehicles (Xe)
- **Tesla Model 3** - 30A-12345 (Xe điện)
- **Toyota Camry** - 30B-67890 (Hybrid)
- **Honda Civic** - 30C-11111 (Xăng)
- **VinFast VF8** - 30D-22222 (Xe điện)

### Roles (Vai trò)
- **Admin** - Quản trị viên hệ thống
- **Staff** - Nhân viên vận hành
- **Co-owner** - Đồng sở hữu xe

### Expense Categories (Danh mục chi phí)
- Nhiên liệu
- Bảo hiểm
- Bảo dưỡng định kỳ
- Sửa chữa
- Phí đỗ xe
- Phí sạc điện
- Phí rửa xe
- Phí đăng kiểm
- Phí phạt
- Chi phí khác

## 🔧 Tính năng chính

- **Quản lý người dùng:** Đăng ký, xác thực, phân quyền
- **Quản lý xe:** Thông tin xe, lịch sử sử dụng
- **Đặt lịch:** Đặt lịch sử dụng xe
- **Quản lý chi phí:** Theo dõi và chia sẻ chi phí
- **Thanh toán:** Quản lý thanh toán và công nợ
- **Báo cáo:** Báo cáo tài chính và sử dụng
- **Bỏ phiếu:** Quyết định các vấn đề quan trọng

## 📁 Cấu trúc dự án

```
Co-owner Vehicle/
├── Data/                    # Database context và seeding
├── Models/                  # Entity models
├── Services/                # Business logic
├── Pages/                   # Razor pages
├── wwwroot/                 # Static files
├── Migrations/              # Database migrations
└── Program.cs               # Application entry point
```

## 🐛 Troubleshooting

### Lỗi "No project was found"
```bash
# Đảm bảo đang ở đúng thư mục
cd "C:\Users\lehuy\source\repos\Co-owner Vehicle\Co-owner Vehicle"
```

### Lỗi database connection
- Kiểm tra SQL Server đang chạy
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo database name không trùng với database khác

### Lỗi migration
```bash
# Xóa database và tạo lại
dotnet ef database drop --force
dotnet ef database update
```

## 📝 Ghi chú

- **Password:** Tất cả password đều là plain text cho môi trường development
- **Database:** Sử dụng LocalDB mặc định
- **Migration:** Tự động seed dữ liệu khi update database

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

**Lưu ý:** Đây là phiên bản development với password plain text. Trong production, cần implement password hashing và các biện pháp bảo mật khác.
