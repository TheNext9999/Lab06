using System;

namespace Lab06_ThreeLayerMemory.DTO
{
    public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public string TenDanhMuc { get; set; } // Thuộc tính bổ sung cho Bài 7

        public double ThanhTien => DonGia * SoLuong;

        public SanPham() { }

        public SanPham(string maSP, string tenSP, double donGia, int soLuong, string tenDanhMuc = "Điện thoại & Tablet")
        {
            MaSP = maSP;
            TenSP = tenSP;
            DonGia = donGia;
            SoLuong = soLuong;
            TenDanhMuc = tenDanhMuc;
        }
    }
}