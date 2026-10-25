namespace Lab06_ThreeLayerMemory.DTO
{
    public class DanhMuc
    {
        public string MaDM { get; set; }
        public string TenDM { get; set; }

        public DanhMuc() { }

        public DanhMuc(string maDM, string tenDM)
        {
            MaDM = maDM;
            TenDM = tenDM;
        }
    }
}