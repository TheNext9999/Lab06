using System;
using System.Collections.Generic;
using System.Linq;
using Lab06_ThreeLayerMemory.DAL;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.BUS
{
    public class SinhVienBUS
    {
        private readonly SinhVienDAL _dal = new SinhVienDAL();

        public List<SinhVien> GetAll()
        {
            return _dal.GetAll();
        }

        public void Add(SinhVien sv)
        {
            if (string.IsNullOrWhiteSpace(sv.MaSV))
                throw new ArgumentException("Mã sinh viên không được để trống!");

            if (string.IsNullOrWhiteSpace(sv.HoTen))
                throw new ArgumentException("Họ tên sinh viên không được để trống!");

            if (sv.DiemTB < 0 || sv.DiemTB > 10)
                throw new ArgumentOutOfRangeException("Điểm trung bình phải từ 0 đến 10!");

            if (_dal.GetAll().Any(x => x.MaSV == sv.MaSV))
                throw new InvalidOperationException("Mã sinh viên đã tồn tại!");

            _dal.Add(sv);
        }

        public void Update(SinhVien sv)
        {
            if (string.IsNullOrWhiteSpace(sv.MaSV))
                throw new ArgumentException("Mã sinh viên không hợp lệ!");

            if (sv.DiemTB < 0 || sv.DiemTB > 10)
                throw new ArgumentOutOfRangeException("Điểm trung bình phải từ 0 đến 10!");

            _dal.Delete(sv.MaSV);
            _dal.Add(sv);
        }

        public void Delete(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên không hợp lệ!");

            _dal.Delete(maSV);
        }

        // Logic xếp loại phục vụ ListView Bài 8
        public string LayXepLoai(double diemTB, out int iconIndex)
        {
            if (diemTB >= 8.0)
            {
                iconIndex = 0; // Giỏi - Xanh lá
                return "Giỏi";
            }
            if (diemTB >= 6.5)
            {
                iconIndex = 1; // Khá - Xanh dương
                return "Khá";
            }
            iconIndex = 2; // Trung bình - Cam
            return "Trung bình";
        }
    }
}