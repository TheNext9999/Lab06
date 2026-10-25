using System;
using System.Collections.Generic;
using System.Linq;
using Lab06_ThreeLayerMemory.DAL;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.BUS
{
    public class SanPhamBUS
    {
        private readonly SanPhamDAL _dal = new SanPhamDAL();

        public List<SanPham> GetAll()
        {
            return _dal.GetAll();
        }

        public void Add(SanPham sp)
        {
            if (string.IsNullOrWhiteSpace(sp.MaSP))
                throw new ArgumentException("Mã sản phẩm không được để trống!");

            if (_dal.GetAll().Any(x => x.MaSP == sp.MaSP))
                throw new InvalidOperationException("Mã sản phẩm đã tồn tại!");

            _dal.Add(sp);
        }

        public void Update(SanPham sp)
        {
            if (string.IsNullOrWhiteSpace(sp.MaSP))
                throw new ArgumentException("Mã sản phẩm không hợp lệ!");

            _dal.Update(sp);
        }

        public void Delete(string maSP)
        {
            if (string.IsNullOrWhiteSpace(maSP))
                throw new ArgumentException("Mã sản phẩm không hợp lệ!");

            _dal.Delete(maSP);
        }

        public List<SanPham> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            string kw = keyword.ToLower().Trim();
            return _dal.GetAll()
                       .Where(x => x.MaSP.ToLower().Contains(kw)
                                || x.TenSP.ToLower().Contains(kw)
                                || (x.TenDanhMuc != null && x.TenDanhMuc.ToLower().Contains(kw)))
                       .ToList();
        }

        public double GetTongGiaTriTonKho()
        {
            return _dal.GetAll().Sum(x => x.ThanhTien);
        }

        public List<SanPham> Sort(string criterion)
        {
            var list = GetAll();
            switch (criterion)
            {
                case "Giá tăng dần":
                    return list.OrderBy(x => x.DonGia).ToList();
                case "Giá giảm dần":
                    return list.OrderByDescending(x => x.DonGia).ToList();
                case "Số lượng giảm dần":
                    return list.OrderByDescending(x => x.SoLuong).ToList();
                default:
                    return list.OrderBy(x => x.TenSP).ToList();
            }
        }
    }
}