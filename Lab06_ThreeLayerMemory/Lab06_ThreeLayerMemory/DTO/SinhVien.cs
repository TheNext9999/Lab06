using System;

namespace Lab06_ThreeLayerMemory.DTO
{
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public double DiemTB { get; set; }
        public string TenKhoa { get; set; }

        public SinhVien() { }

        public SinhVien(string maSV, string hoTen, DateTime ngaySinh, double diemTB, string tenKhoa = "CNTT")
        {
            MaSV = maSV;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            DiemTB = diemTB;
            TenKhoa = tenKhoa;
        }
    }
}