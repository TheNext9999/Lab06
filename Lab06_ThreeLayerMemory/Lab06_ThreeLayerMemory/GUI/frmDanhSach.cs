using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory
{
    public partial class frmDanhSach : Form
    {
        // Khai báo đối tượng tầng BUS để tương tác dữ liệu
        private readonly SinhVienBUS _bus = new SinhVienBUS();
        private BindingList<SinhVien> _dsSinhVien;

        public frmDanhSach()
        {
            InitializeComponent();

            // Gán thủ công sự kiện Load và Click dòng dữ liệu
            this.Load += frmDanhSach_Load;
            this.dgvSinhVien.CellClick += dgvSinhVien_CellClick;
        }

        // Sự kiện khi Form tải lên
        private void frmDanhSach_Load(object sender, EventArgs e)
        {
            // Chọn mặc định mục đầu tiên trong ComboBox nếu có
            if (cboKhoa.Items.Count > 0)
                cboKhoa.SelectedIndex = 0;

            try
            {
                // Thêm dữ liệu mẫu ban đầu nếu danh sách đang trống
                if (_bus.GetAll().Count == 0)
                {
                    _bus.Add(new SinhVien("SV01", "Nguyễn An", DateTime.Today.AddYears(-20), 8.2, "CNTT"));
                    _bus.Add(new SinhVien("SV02", "Trần Bình", DateTime.Today.AddYears(-21), 7.0, "CNTT"));
                    _bus.Add(new SinhVien("SV03", "Lê Cường", DateTime.Today.AddYears(-22), 8.8, "Kinh tế"));
                }
            }
            catch { /* Bỏ qua nếu dữ liệu mẫu đã tồn tại */ }

            CapNhatTatCaGiaoDien();
        }

        // Hàm nạp và đồng bộ dữ liệu lên cả DataGridView, ListView và TreeView
        private void CapNhatTatCaGiaoDien()
        {
            List<SinhVien> list = _bus.GetAll();

            // 1. Đồng bộ DataGridView
            _dsSinhVien = new BindingList<SinhVien>(list);
            dgvSinhVien.DataSource = _dsSinhVien;
            dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSinhVien.ReadOnly = true;

            // 2. Đồng bộ ListView (Bài 3)
            NapListView(list);

            // 3. Đồng bộ TreeView (Bài 4)
            NapTreeView(list);
        }

        // Nạp danh sách sinh viên lên ListView
        private void NapListView(IEnumerable<SinhVien> data)
        {
            listView1.Items.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Mã SV", 80);
                listView1.Columns.Add("Họ tên", 150);
                listView1.Columns.Add("Khoa", 100);
                listView1.Columns.Add("Điểm TB", 80);
            }

            foreach (SinhVien sv in data)
            {
                ListViewItem item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HoTen);
                item.SubItems.Add(sv.TenKhoa);
                item.SubItems.Add(sv.DiemTB.ToString("N2"));
                item.Tag = sv;
                listView1.Items.Add(item);
            }
        }

        // Nạp danh sách sinh viên lên TreeView theo từng Khoa
        private void NapTreeView(IEnumerable<SinhVien> data)
        {
            tvKhoa.Nodes.Clear();

            var grouped = data.GroupBy(sv => string.IsNullOrEmpty(sv.TenKhoa) ? "CNTT" : sv.TenKhoa);

            foreach (var group in grouped)
            {
                TreeNode khoaNode = new TreeNode(group.Key);
                foreach (SinhVien sv in group)
                {
                    TreeNode svNode = new TreeNode($"{sv.MaSV} - {sv.HoTen}");
                    svNode.Tag = sv;
                    khoaNode.Nodes.Add(svNode);
                }
                tvKhoa.Nodes.Add(khoaNode);
            }
            tvKhoa.ExpandAll();
        }

        // Xử lý sự kiện bấm nút Thêm
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                    string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                    string.IsNullOrWhiteSpace(txtDiemTB.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtDiemTB.Text.Trim(), out double diemTB) || diemTB < 0 || diemTB > 10)
                {
                    MessageBox.Show("Điểm trung bình phải là số thuộc khoảng [0.0 - 10.0]!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string tenKhoa = cboKhoa.SelectedItem != null ? cboKhoa.SelectedItem.ToString() : "CNTT";

                SinhVien sv = new SinhVien(
                    txtMaSV.Text.Trim(),
                    txtHoTen.Text.Trim(),
                    dtpNgaySinh.Value,
                    diemTB,
                    tenKhoa
                );

                _bus.Add(sv);
                CapNhatTatCaGiaoDien();
                XoaTrangForm();
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện bấm nút Xóa
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maSV = dgvSinhVien.CurrentRow.Cells["MaSV"].Value.ToString();
            string hoTen = dgvSinhVien.CurrentRow.Cells["HoTen"].Value.ToString();

            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên [{maSV} - {hoTen}] không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    _bus.Delete(maSV);
                    CapNhatTatCaGiaoDien();
                    XoaTrangForm();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Chọn node trên TreeView để hiển thị chi tiết sinh viên
        private void tvKhoa_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is SinhVien sv)
            {
                txtMaSV.Text = sv.MaSV;
                txtHoTen.Text = sv.HoTen;
                dtpNgaySinh.Value = sv.NgaySinh;
                txtDiemTB.Text = sv.DiemTB.ToString();
                if (cboKhoa.Items.Contains(sv.TenKhoa))
                    cboKhoa.SelectedItem = sv.TenKhoa;
            }
        }

        // Chọn dòng trên DataGridView
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            HienThiChiTiet(e.RowIndex);
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            HienThiChiTiet(e.RowIndex);
        }

        private void HienThiChiTiet(int rowIndex)
        {
            if (rowIndex >= 0 && dgvSinhVien.Rows[rowIndex].DataBoundItem is SinhVien sv)
            {
                txtMaSV.Text = sv.MaSV;
                txtHoTen.Text = sv.HoTen;
                dtpNgaySinh.Value = sv.NgaySinh;
                txtDiemTB.Text = sv.DiemTB.ToString();
                if (cboKhoa.Items.Contains(sv.TenKhoa))
                    cboKhoa.SelectedItem = sv.TenKhoa;
            }
        }

        // Xóa trắng form nhập liệu
        private void XoaTrangForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();
            dtpNgaySinh.Value = DateTime.Today;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            txtMaSV.Focus();
        }

        // Các sự kiện thay đổi dữ liệu phụ
        private void txtMaSV_TextChanged(object sender, EventArgs e) { }
        private void txtHoTen_TextChanged(object sender, EventArgs e) { }
        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e) { }
        private void txtDiemTB_TextChanged(object sender, EventArgs e) { }
        private void cboKhoa_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}