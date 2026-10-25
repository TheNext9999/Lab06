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
                throw new ArgumentException("Thiếu mã sinh viên!");

            if (_dal.GetAll().Any(x => x.MaSV == sv.MaSV))
                throw new InvalidOperationException("Mã sinh viên đã tồn tại!");

            _dal.Add(sv);
        }

        public void Update(SinhVien sv)
        {
            if (string.IsNullOrWhiteSpace(sv.MaSV))
                throw new ArgumentException("Thiếu mã sinh viên!");

            _dal.Update(sv);
        }

        public void Delete(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                throw new ArgumentException("Mã sinh viên không hợp lệ!");

            _dal.Delete(maSV);
        }
    }
}