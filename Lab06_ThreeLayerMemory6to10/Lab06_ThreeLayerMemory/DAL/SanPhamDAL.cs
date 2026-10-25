using System.Collections.Generic;
using System.Linq;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.DAL
{
    public class SanPhamDAL
    {
        private static readonly List<SanPham> _list = new List<SanPham>();

        public List<SanPham> GetAll()
        {
            return new List<SanPham>(_list);
        }

        public void Add(SanPham sp)
        {
            _list.Add(sp);
        }

        public void Update(SanPham sp)
        {
            var item = _list.FirstOrDefault(x => x.MaSP == sp.MaSP);
            if (item != null)
            {
                item.TenSP = sp.TenSP;
                item.DonGia = sp.DonGia;
                item.SoLuong = sp.SoLuong;
            }
        }

        public void Delete(string maSP)
        {
            var item = _list.FirstOrDefault(x => x.MaSP == maSP);
            if (item != null)
            {
                _list.Remove(item);
            }
        }
    }
}