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

**🎯 2 LỆNH QUAN TRỌNG 

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


