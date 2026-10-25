using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory.DAL
{
    public class SinhVienDAL
    {
        private static readonly List<SinhVien> _data = new List<SinhVien>();
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sinhvien.csv");

        public SinhVienDAL()
        {
            // Tự động nạp dữ liệu từ CSV khi khởi tạo
            if (_data.Count == 0)
            {
                LoadFromCsv();
            }
        }

        public List<SinhVien> GetAll()
        {
            return new List<SinhVien>(_data);
        }

        public void Add(SinhVien sv)
        {
            _data.Add(sv);
            SaveToCsv();
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
                SaveToCsv();
            }
        }

        public void Delete(string maSV)
        {
            var sv = _data.FirstOrDefault(x => x.MaSV == maSV);
            if (sv != null)
            {
                _data.Remove(sv);
                SaveToCsv();
            }
        }

        // --- XỬ LÝ ĐỌC / GHI TỆP CSV (BÀI 9) ---

        public void SaveToCsv()
        {
            try
            {
                List<string> lines = new List<string>
                {
                    "MaSV,HoTen,NgaySinh,DiemTB,TenKhoa" // Dòng Tiêu đề
                };

                foreach (var sv in _data)
                {
                    lines.Add($"{sv.MaSV},{sv.HoTen},{sv.NgaySinh:yyyy-MM-dd},{sv.DiemTB},{sv.TenKhoa}");
                }

                File.WriteAllLines(_filePath, lines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ghi tệp CSV từ DAL: " + ex.Message);
            }
        }

        public void LoadFromCsv()
        {
            if (!File.Exists(_filePath))
            {
                // Dữ liệu mặc định nếu chưa có file CSV
                _data.Add(new SinhVien("SV01", "Nguyễn An", new DateTime(2006, 10, 24), 8.2, "CNTT"));
                _data.Add(new SinhVien("SV02", "Trần Bình", new DateTime(2005, 10, 24), 7.0, "CNTT"));
                _data.Add(new SinhVien("SV03", "Lê Cường", new DateTime(2004, 10, 24), 8.8, "Kinh tế"));
                SaveToCsv();
                return;
            }

            try
            {
                _data.Clear();
                string[] lines = File.ReadAllLines(_filePath, Encoding.UTF8);

                for (int i = 1; i < lines.Length; i++) // Bỏ qua tiêu đề
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] parts = line.Split(',');
                    if (parts.Length >= 5)
                    {
                        _data.Add(new SinhVien
                        {
                            MaSV = parts[0].Trim(),
                            HoTen = parts[1].Trim(),
                            NgaySinh = DateTime.TryParse(parts[2].Trim(), out DateTime ns) ? ns : DateTime.Today,
                            DiemTB = double.TryParse(parts[3].Trim(), out double dtb) ? dtb : 0.0,
                            TenKhoa = parts[4].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đọc tệp CSV từ DAL: " + ex.Message);
            }
        }
    }
}