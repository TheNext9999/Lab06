using System;
using System.Collections.Generic;
using System.Linq;
using Lab06_ThreeLayerMemory.DAL;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.BUS
{
    public class DanhMucBUS
    {
        private readonly DanhMucDAL _dal = new DanhMucDAL();

        public List<DanhMuc> GetAll()
        {
            return _dal.GetAll();
        }

        public void Add(DanhMuc dm)
        {
            if (string.IsNullOrWhiteSpace(dm.MaDM))
                throw new ArgumentException("Mã danh mục không được để trống!");

            if (_dal.GetAll().Any(x => x.MaDM == dm.MaDM))
                throw new InvalidOperationException("Mã danh mục đã tồn tại!");

            _dal.Add(dm);
        }

        public void Update(DanhMuc dm)
        {
            if (string.IsNullOrWhiteSpace(dm.MaDM))
                throw new ArgumentException("Mã danh mục không hợp lệ!");

            _dal.Update(dm);
        }

        public void Delete(string maDM)
        {
            if (string.IsNullOrWhiteSpace(maDM))
                throw new ArgumentException("Mã danh mục không hợp lệ!");

            _dal.Delete(maDM);
        }
    }
}