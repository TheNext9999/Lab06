using System.Collections.Generic;
using System.Linq;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.DAL
{
    public class SinhVienDAL
    {
        private static readonly List<SinhVien> _data = new List<SinhVien>();

        public List<SinhVien> GetAll()
        {
            return new List<SinhVien>(_data);
        }

        public void Add(SinhVien sv)
        {
            _data.Add(sv);
        }

        public void Update(SinhVien sv)
        {
            var existing = _data.FirstOrDefault(x => x.MaSV == sv.MaSV);
            if (existing != null)
            {
                existing.HoTen = sv.HoTen;
                existing.NgaySinh = sv.NgaySinh;
                existing.DiemTB = sv.DiemTB;
                existing.TenKhoa = sv.TenKhoa;
            }
        }

        public void Delete(string maSV)
        {
            var sv = _data.FirstOrDefault(x => x.MaSV == maSV);
            if (sv != null)
            {
                _data.Remove(sv);
            }
        }
    }
}