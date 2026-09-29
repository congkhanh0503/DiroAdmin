# 🛡️ Hướng Dẫn Sử Dụng DiroAdmin (Cổng Quản Trị Khách Hàng)

Hệ thống **DiroAdmin** là trung tâm kiểm soát bản quyền, khách hàng và doanh thu phần mềm độc lập dành riêng cho bạn.

---

## 🌐 1. Địa Chỉ Truy Cập

- **Giao diện Web Quản Trị DiroAdmin**: [http://localhost:8080](http://localhost:8080)
- **Tài liệu API Swagger DiroAdmin**: [http://localhost:5020/swagger](http://localhost:5020/swagger)
- **Hệ thống bán hàng DiroPos (Máy khách)**: [http://localhost:80](http://localhost:80)

---

## ⚡ 2. Quy Trình Cấp Bản Quyền Cho Khách Hàng Mới

Khi có tiệm tóc / spa muốn mua hoặc dùng thử phần mềm:

1. Mở [http://localhost:8080](http://localhost:8080) -> Bấm **"+ Thêm Quán Khách Hàng"**.
2. Nhập:
   - **Tên Quán**: (VD: *Hải Barber Shop*)
   - **Mã Quán**: (VD: `DP-HAI-01` hoặc để trống hệ thống tự sinh `DP-xxxx`)
   - **Họ tên chủ quán & Số điện thoại (Zalo)**
   - **Gói cước**: 1 tháng dùng thử, 6 tháng hoặc 1 năm.
3. Bấm **"Tạo Quán & Cấp Key Ngay"**:
   - Hệ thống tự động tạo mã quán và sinh ra một chuỗi **Mã Bản Quyền (License Key)** duy nhất.
4. Gửi chuỗi License Key này cho khách qua tin nhắn Zalo/SMS.
5. Hướng dẫn khách hàng:
   - Mở phần mềm DiroPos của họ -> Vào **"Cài Đặt & Đóng Ca"** -> Bấm **"Nhập Mã Kích Hoạt / Gia Hạn"** -> Dán chuỗi Key vào -> Bấm **Kích Hoạt**.
   - Máy khách sẽ mở khóa ngay lập tức và hiển thị đúng hạn sử dụng!

---

## 🔒 3. Cách Khóa Quán Khi Khách Bùng Tiền / Hết Hạn

Nếu khách hàng chưa thanh toán tiền cước hoặc vi phạm hợp đồng:
- Trên bảng danh sách của DiroAdmin ([http://localhost:8080](http://localhost:8080)), tìm đến tên quán đó -> Bấm vào biểu tượng **Ổ Khóa 🔒**.
- Trạng thái quán sẽ chuyển thành **"Đã Tạm Khóa"**.
- Ngay khi máy khách DiroPos của quán đó hoạt động, giao diện POS sẽ bị phong tỏa hoàn toàn bởi màn hình khóa đỏ và chặn mọi thao tác tạo đơn hàng cho đến khi bạn bấm **Mở Khóa 🔓**!

---

## 🔄 4. Khởi Động & Dừng Hệ Thống

- **Bật DiroAdmin**:
  ```bash
  cd d:\Project\pos\DiroAdmin
  docker compose up -d
  ```
- **Tắt DiroAdmin**:
  ```bash
  cd d:\Project\pos\DiroAdmin
  docker compose down
  ```
