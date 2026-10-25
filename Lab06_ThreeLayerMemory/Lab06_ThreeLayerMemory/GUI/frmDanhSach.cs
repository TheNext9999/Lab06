using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory
{
    public partial class frmDanhSach : Form
    {
        private readonly SinhVienBUS _bus = new SinhVienBUS();
        private BindingList<SinhVien> _dsSinhVien;
        private ImageList _imageListXepLoai;

        public frmDanhSach()
        {
            InitializeComponent();

            this.Load += frmDanhSach_Load;
            this.dgvSinhVien.CellClick += dgvSinhVien_CellClick;
        }

        private void frmDanhSach_Load(object sender, EventArgs e)
        {
            KhoiTaoImageList();

            if (cboKhoa.Items.Count > 0)
                cboKhoa.SelectedIndex = 0;

            CapNhatTatCaGiaoDien();
        }

        private void KhoiTaoImageList()
        {
            _imageListXepLoai = new ImageList();
            _imageListXepLoai.ImageSize = new Size(16, 16);
            _imageListXepLoai.ColorDepth = ColorDepth.Depth32Bit;

            _imageListXepLoai.Images.Add(TaoIconTron(Color.ForestGreen)); // 0: Giỏi
            _imageListXepLoai.Images.Add(TaoIconTron(Color.DodgerBlue));  // 1: Khá
            _imageListXepLoai.Images.Add(TaoIconTron(Color.OrangeRed));   // 2: Trung bình
        }

        private Bitmap TaoIconTron(Color color)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Brush b = new SolidBrush(color))
                {
                    g.FillEllipse(b, 1, 1, 13, 13);
                }
            }
            return bmp;
        }

        private void CapNhatTatCaGiaoDien()
        {
            List<SinhVien> list = _bus.GetAll();

            // 1. DataGridView
            _dsSinhVien = new BindingList<SinhVien>(list);
            dgvSinhVien.DataSource = _dsSinhVien;
            dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSinhVien.ReadOnly = true;

            // 2. ListView (Bài 8 - Biểu tượng Xếp loại)
            NapListView(list);

            // 3. TreeView (Phân cấp theo Khoa)
            NapTreeView(list);
        }

        private void NapListView(IEnumerable<SinhVien> data)
        {
            listView1.Items.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.SmallImageList = _imageListXepLoai;

            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Mã SV", 80);
                listView1.Columns.Add("Họ tên", 140);
                listView1.Columns.Add("Khoa", 90);
                listView1.Columns.Add("Điểm TB", 70);
                listView1.Columns.Add("Xếp loại", 90);
            }

            foreach (SinhVien sv in data)
            {
                string xepLoai = _bus.LayXepLoai(sv.DiemTB, out int imageIndex);

                ListViewItem item = new ListViewItem(sv.MaSV, imageIndex);
                item.SubItems.Add(sv.HoTen);
                item.SubItems.Add(sv.TenKhoa);
                item.SubItems.Add(sv.DiemTB.ToString("N2"));
                item.SubItems.Add(xepLoai);
                item.Tag = sv;

                listView1.Items.Add(item);
            }
        }

        private void NapTreeView(IEnumerable<SinhVien> data)
        {
            tvKhoa.Nodes.Clear();
            var grouped = System.Linq.Enumerable.GroupBy(data, sv => string.IsNullOrEmpty(sv.TenKhoa) ? "CNTT" : sv.TenKhoa);

            foreach (var group in grouped)
            {
                TreeNode khoaNode = new TreeNode(group.Key);
                foreach (SinhVien sv in group)
                {
                    TreeNode svNode = new TreeNode($"{sv.MaSV} - {sv.HoTen}") { Tag = sv };
                    khoaNode.Nodes.Add(svNode);
                }
                tvKhoa.Nodes.Add(khoaNode);
            }
            tvKhoa.ExpandAll();
        }

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
                MessageBox.Show("Thêm sinh viên thành công & Đã tự động lưu vào tệp CSV!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                    MessageBox.Show("Xóa sinh viên thành công & Đã cập nhật tệp CSV!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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

        private void XoaTrangForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();
            dtpNgaySinh.Value = DateTime.Today;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            txtMaSV.Focus();
        }

        private void txtMaSV_TextChanged(object sender, EventArgs e) { }
        private void txtHoTen_TextChanged(object sender, EventArgs e) { }
        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e) { }
        private void txtDiemTB_TextChanged(object sender, EventArgs e) { }
        private void cboKhoa_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}