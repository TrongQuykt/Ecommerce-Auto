# Website Thương Mại Điện Tử Quản Lý Cửa Hàng Ô Tô

![ASP.NET](https://img.shields.io/badge/ASP.NET%20MVC-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoft-sql-server&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?logo=bootstrap&logoColor=white)

## Giới thiệu

Dự án **Website Thương Mại Điện Tử Quản Lý Cửa Hàng Ô Tô** được xây dựng bằng **ASP.NET MVC** với ngôn ngữ **C#**. Hệ thống cung cấp nền tảng mua sắm trực tuyến chuyên nghiệp cho khách hàng và công cụ quản trị toàn diện cho admin.

Người dùng có thể dễ dàng xem thông tin xe, thêm vào giỏ hàng, đặt hàng và thanh toán trực tuyến. Phía quản trị hỗ trợ quản lý sản phẩm, đơn hàng, kho hàng, mã giảm giá, doanh thu và hỗ trợ khách hàng qua **chat realtime**.

## Công nghệ sử dụng

### Backend
- **ASP.NET MVC Framework**
- **C#**
- **Entity Framework** (Code First)
- **Microsoft SQL Server**
- **ASP.NET Identity** (xác thực & phân quyền)
- **SignalR** (chat realtime)
- **VNPay** (thanh toán trực tuyến)

### Frontend
- **Razor View Engine (CSHTML)**
- **Bootstrap 5**
- **CSS3 & JavaScript**
- **jQuery**

### Công cụ phát triển
- Visual Studio
- Microsoft SQL Server Management Studio (SSMS)
- Visio (thiết kế Use Case & Database)

## Tính năng chính

### Dành cho Khách hàng (User)
- Đăng ký / Đăng nhập / Đăng xuất
- Quên mật khẩu & khôi phục qua email (mã xác thực)
- Cập nhật thông tin cá nhân & địa chỉ giao hàng
- Xem danh sách sản phẩm, tìm kiếm, xem chi tiết xe
- Thêm/xóa/sửa sản phẩm trong giỏ hàng
- Áp dụng mã giảm giá
- Đặt hàng & thanh toán (VNPay hoặc COD)
- Xem lịch sử đơn hàng và trạng thái đơn hàng
- Chat hỗ trợ realtime với nhân viên

### Dành cho Quản trị viên (Admin)
- Quản lý sản phẩm (CRUD)
- Quản lý loại sản phẩm (ProductType)
- Quản lý phiếu nhập kho (Receipt_Products)
- Quản lý tài khoản khách hàng & quản trị viên
- Quản lý mã giảm giá
- Quản lý đơn hàng (xác nhận, vận chuyển, hoàn thành, hủy)
- Thống kê doanh thu (theo ngày/tháng/tổng)
- Xuất báo cáo Excel
- Quản lý giao dịch VNPay
- Hỗ trợ khách hàng qua chatbox realtime

## Cơ sở dữ liệu

Hệ thống sử dụng **Microsoft SQL Server** với các bảng chính:
- `Accounts`, `Roles`, `AccountType`, `Address`
- `Products`, `ProductTypes`, `Products_Inventorys`
- `Orders`, `Order_Items`
- `Payments`, `PaymentMethods`, `Shipments`, `ShippingMethods`
- `Discounts`, `Receipt_Products`
- `Messages` (hỗ trợ chat)

Chi tiết schema nằm trong thư mục `Database/` hoặc script SQL.

## Hướng dẫn cài đặt & chạy dự án

### Yêu cầu hệ thống
- Visual Studio 2022 (hoặc mới hơn)
- .NET Framework 4.8+
- Microsoft SQL Server 2019+
- IIS (nếu deploy)

### Các bước thực hiện

1. **Clone repository**
   ```bash
   git clone https://github.com/TrongQuykt/Ecommerce-Auto.git
   cd StoreCars
## Ảnh dự án
![Picture14](https://github.com/user-attachments/assets/f0c0ec64-2c3a-4abb-a513-e6be0eaa0b65)
![Picture15](https://github.com/user-attachments/assets/07032773-cae8-4251-8978-925dd9cf19e6)
![Picture1](https://github.com/user-attachments/assets/e10c75a7-c70a-4df2-b081-af50346193ec)
![Picture2](https://github.com/user-attachments/assets/b787c9c4-31ec-424b-a78d-fd93d86c2b60)
![Picture3](https://github.com/user-attachments/assets/355767e1-f4d8-451a-aa26-5357c1b89256)
![Picture4](https://github.com/user-attachments/assets/5c5db648-566a-4bb0-8d9e-81dcbb45ec5f)
![Picture5](https://github.com/user-attachments/assets/e8004a93-2f7c-4895-988b-57d367e22e12)
![Picture6](https://github.com/user-attachments/assets/67f893c1-9f8e-4198-970c-1441e588b945)
![Picture7](https://github.com/user-attachments/assets/7310664d-cc95-4562-8828-b3c6412c5618)
![Picture8](https://github.com/user-attachments/assets/fa1b2bec-048f-4b20-b042-07f71d92e9ae)
![Picture9](https://github.com/user-attachments/assets/2be042a8-c160-4e07-8cc9-ce03916c0bba)
![Picture10](https://github.com/user-attachments/assets/99b0ef9e-09a0-4779-af8e-9678d1802e33)
![Picture11](https://github.com/user-attachments/assets/078739e8-e808-429a-9dc6-3b78ced0b21d)
![Picture12](https://github.com/user-attachments/assets/48b17e18-8065-4240-b60c-f935c28bf5fb)
![Picture13](https://github.com/user-attachments/assets/a0dee5dd-6d0a-4b08-bcdb-1efd6aa01490)
