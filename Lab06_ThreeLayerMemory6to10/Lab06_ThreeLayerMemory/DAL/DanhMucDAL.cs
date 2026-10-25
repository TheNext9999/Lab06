using System.Collections.Generic;
using System.Linq;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.DAL
{
    public class DanhMucDAL
    {
        private static readonly List<DanhMuc> _listDM = new List<DanhMuc>();

        public List<DanhMuc> GetAll()
        {
            return new List<DanhMuc>(_listDM);
        }

        public void Add(DanhMuc dm)
        {
            _listDM.Add(dm);
        }

        public void Update(DanhMuc dm)
        {
            var item = _listDM.FirstOrDefault(x => x.MaDM == dm.MaDM);
            if (item != null)
            {
                item.TenDM = dm.TenDM;
            }
        }

        public void Delete(string maDM)
        {
            var item = _listDM.FirstOrDefault(x => x.MaDM == maDM);
            if (item != null)
            {
                _listDM.Remove(item);
            }
        }
    }
}