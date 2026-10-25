# BÀI THỰC HÀNH LAB 06: DATAGRIDVIEW, LISTVIEW, TREEVIEW VÀ MÔ HÌNH 3 LỚP CHƯA KẾT NỐI CSDL

**Học phần:** Lập trình .NET (IT3212)  
**Đơn vị:** Bộ môn Công nghệ Phần mềm - Khoa CNTT - Trường Đại học Công nghệ Đông Á  
**Tên dự án:** `Lab06_ThreeLayerMemory`  

---

## 1. TỔNG QUAN DỰ ÁN

Dự án **Lab06_ThreeLayerMemory** xây dựng ứng dụng Windows Forms Quản lý Sinh viên chạy hoàn toàn trong bộ nhớ kết hợp lưu trữ tệp văn bản CSV. Ứng dụng tuân thủ chuẩn kiến trúc 3 lớp (**3-Layer Architecture**):
- **DTO (Data Transfer Object):** Định nghĩa đối tượng `SinhVien`.
- **DAL (Data Access Layer):** Quản lý tập hợp sinh viên trong bộ nhớ và tự động đồng bộ đọc/ghi tệp `sinhvien.csv`.
- **BUS (Business Logic Layer):** Xử lý quy tắc nghiệp vụ, kiểm tra ràng buộc hợp lệ và phân loại dữ liệu.
- **GUI (Graphical User Interface):** Form giao diện người dùng chứa `DataGridView`, `ListView` (kèm `ImageList`) và `TreeView`.

---

## 2. CẤU TRÚC THƯ MỤC NGUỒN

```text
Lab06_ThreeLayerMemory/
│
├── DTO/
│   └── SinhVien.cs             # Lớp đối tượng SinhVien (MaSV, HoTen, NgaySinh, DiemTB, TenKhoa)
│
├── DAL/
│   └── SinhVienDAL.cs          # Quản lý List<SinhVien>, đọc/ghi tệp CSV từ đĩa
│
├── BUS/
│   └── SinhVienBUS.cs          # Logic nghiệp vụ, validation điểm [0, 10], kiểm tra trùng Mã SV, xếp loại
│
├── GUI/ (Form)
│   ├── frmDanhSach.cs          # Giao diện chính xử lý sự kiện DataGridView, ListView, TreeView
│   └── frmDanhSach.Designer.cs # Mã thiết kế giao diện Form
│
├── bin/Debug/
│   └── sinhvien.csv            # Tệp dữ liệu CSV tự động khởi tạo và cập nhật
│
├── README.md                   # Tệp hướng dẫn vận hành dự án
└── BaoCao_Lab06.docx           # Tài liệu báo cáo kết quả thực hành
```

---

## 3. BẢNG TIẾN ĐỘ & PHẦN ĐÃ HOÀN THÀNH

| STT | Bài tập | Yêu cầu | Trạng thái | Ghi chú triển khai |
| :---: | :--- | :--- | :---: | :--- |
| **Bài 1** | Hiển thị BindingList trên DataGridView | Đổ dữ liệu từ `BindingList<SinhVien>` lên DataGridView | **Hoàn thành** | Tải và liên kết dữ liệu mượt mà, định dạng cột rõ ràng. |
| **Bài 2** | CRUD trong bộ nhớ | Thực hiện Thêm, Sửa, Xóa sinh viên | **Hoàn thành** | Bắt lỗi trùng mã, kiểm tra dữ liệu đầu vào. |
| **Bài 3** | Danh sách chi tiết bằng ListView | Hiển thị dữ liệu dạng View Details | **Hoàn thành** | Hiển thị 5 cột chi tiết: Mã, Họ tên, Khoa, Điểm TB, Xếp loại. |
| **Bài 4** | Cây Khoa - Sinh viên bằng TreeView | Phân nhóm Node gốc là Khoa, Node con là Sinh viên | **Hoàn thành** | Click vào Node con hiển thị ngược lên các ô nhập liệu. |
| **Bài 5** | Tách mô hình GUI - BUS - DAL | Chuẩn hóa dòng dữ liệu qua 3 lớp | **Hoàn thành** | Form chỉ gọi BUS, BUS gọi DAL, DAL quản lý dữ liệu. |
| **Bài 6** | Quản lý dữ liệu trên DataGridView | Tìm kiếm, lọc và sắp xếp dữ liệu sinh viên | **Hoàn thành** | Cập nhật đồng bộ tức thì trên DataGridView. |
| **Bài 7** | TreeView quản lý danh mục | Phân cấp quản lý danh mục Khoa & Sinh viên | **Hoàn thành** | Gom nhóm linh hoạt theo `TenKhoa`, lưu trữ đối tượng qua `Tag`. |
| **Bài 8** | ListView có biểu tượng | Tích hợp `ImageList` hiển thị icon xếp loại | **Hoàn thành** | Màu xanh lá (Giỏi), xanh dương (Khá), cam (Trung bình). |
| **Bài 9** | Xuất / Đọc tệp văn bản (CSV) | Tự động đọc/lưu danh sách sinh viên ra tệp CSV | **Hoàn thành** | Xử lý 100% tại tầng DAL khi ứng dụng khởi chạy / thêm / xóa. |
| **Bài 10** | Hoàn thiện kiến trúc 3 lớp | Chuẩn hóa tách biệt hoàn toàn logic | **Hoàn thành** | Loại bỏ hoàn toàn logic nghiệp vụ và đọc file ra khỏi Form GUI. |

---

## 4. HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN

### Yêu cầu hệ thống:
* **Hệ điều hành:** Windows 10 / 11.
* **Môi trường:** Visual Studio 2022 (phiên bản 17.0 trở lên) có cài đặt Workload **.NET desktop development**.
* **Target Framework:** .NET Framework 4.7.2 hoặc .NET 6.0/8.0 Windows Forms App.

### Các bước thực hiện:
1. Mở Visual Studio 2022.
2. Chọn **File > Open > Project/Solution**, tìm đến tệp `Lab06_ThreeLayerMemory.sln`.
3. Nhấn tổ hợp phím `Ctrl + Shift + B` để Rebuild Solution và kiểm tra không có lỗi biên dịch.
4. Nhấn `F5` hoặc nút **Start** để chạy ứng dụng.

---

## 5. DỮ LIỆU THỬ NGHIỆM (SAMPLE DATA)

Tệp `sinhvien.csv` sẽ tự động tạo tại thư mục đầu ra `bin/Debug/sinhvien.csv` với định dạng UTF-8:

```csv
MaSV,HoTen,NgaySinh,DiemTB,TenKhoa
SV01,Nguyễn An,2006-10-24,8.2,CNTT
SV02,Trần Bình,2005-10-24,7,CNTT
SV03,Lê Cường,2004-10-24,8.8,Kinh tế
SV04,Phạm Dung,2006-10-24,5.5,Ngoại ngữ
```

---

## 6. QUY TRÌNH KIỂM THỬ GIAO DIỆN

1. **Khởi động ứng dụng:** Giao diện đồng thời hiển thị danh sách trên DataGridView, ListView (có biểu tượng màu xếp loại), và TreeView phân cấp theo Khoa.
2. **Thêm Sinh viên:** 
   - Nhập thông tin: `MaSV = SV05`, `HoTen = Hoàng Văn Em`, `DiemTB = 9.0`, chọn `Khoa = CNTT`.
   - Bấm **Thêm**. Hệ thống hiển thị thông báo thành công và dữ liệu lập tức cập nhật ở 3 View đồng thời lưu vào file CSV.
3. **Xem Chi Tiết:** Click chọn 1 dòng trên DataGridView hoặc 1 Node con trên TreeView, dữ liệu tự động nạp lên các TextBox tương ứng.
4. **Xóa Sinh viên:** Chọn dòng cần xóa trên DataGridView, bấm **Xóa**, xác nhận Dialog. Dữ liệu bị loại bỏ khỏi danh sách và file CSV được cập nhật lại.