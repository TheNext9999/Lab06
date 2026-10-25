using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory
{
    public partial class frmSanPham : Form
    {
        private readonly SanPhamBUS _spBUS = new SanPhamBUS();
        private readonly DanhMucBUS _dmBUS = new DanhMucBUS();

        public frmSanPham()
        {
            InitializeComponent();
            this.Load += frmSanPham_Load;

            if (dgvSanPham != null)
            {
                this.dgvSanPham.CellClick += dgvSanPham_CellClick;
            }

            if (tvDanhMuc != null && cmsTree != null)
            {
                tvDanhMuc.ContextMenuStrip = cmsTree;
                tvDanhMuc.NodeMouseClick += tvDanhMuc_NodeMouseClick;
            }
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            if (cboSapXep != null)
            {
                cboSapXep.Items.Clear();
                cboSapXep.Items.Add("Tên A-Z");
                cboSapXep.Items.Add("Giá tăng dần");
                cboSapXep.Items.Add("Giá giảm dần");
                cboSapXep.Items.Add("Số lượng giảm dần");
                cboSapXep.SelectedIndex = 0;
            }

            if (_dmBUS.GetAll().Count == 0)
            {
                _dmBUS.Add(new DanhMuc("DM01", "Điện thoại & Tablet"));
                _dmBUS.Add(new DanhMuc("DM02", "Laptop & Phụ kiện"));
            }

            if (_spBUS.GetAll().Count == 0)
            {
                _spBUS.Add(new SanPham("SP01", "Laptop Dell XPS", 25000000, 5, "Laptop & Phụ kiện"));
                _spBUS.Add(new SanPham("SP02", "Chuột Logitech MX", 1800000, 20, "Laptop & Phụ kiện"));
                _spBUS.Add(new SanPham("SP03", "Bàn phím Keychron", 2200000, 12, "Laptop & Phụ kiện"));
                _spBUS.Add(new SanPham("SP04", "iPhone 15 Pro Max", 28000000, 8, "Điện thoại & Tablet"));
            }

            KhoiTaoContextMenu();
            HienThiDanhSach(_spBUS.GetAll());
            NapTreeView();
            NapComboboxDanhMuc();
        }

        private void NapComboboxDanhMuc()
        {
            if (cboDanhMuc == null) return;

            cboDanhMuc.Items.Clear();
            foreach (var dm in _dmBUS.GetAll())
            {
                cboDanhMuc.Items.Add(dm.TenDM);
            }

            if (cboDanhMuc.Items.Count > 0)
                cboDanhMuc.SelectedIndex = 0;
        }

        private void HienThiDanhSach(List<SanPham> list)
        {
            if (dgvSanPham == null) return;

            dgvSanPham.DataSource = new BindingList<SanPham>(list);
            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPham.ReadOnly = true;

            if (lblTongTien != null)
            {
                double tongTien = _spBUS.GetTongGiaTriTonKho();
                lblTongTien.Text = $"Tổng giá trị hàng tồn kho: {tongTien:N0} VNĐ";
            }
        }

        private void NapTreeView()
        {
            if (tvDanhMuc == null) return;

            tvDanhMuc.Nodes.Clear();
            var dsDM = _dmBUS.GetAll();
            var dsSP = _spBUS.GetAll();

            foreach (var dm in dsDM)
            {
                TreeNode nodeDM = new TreeNode(dm.TenDM) { Tag = dm };
                var spThuocDM = dsSP.Where(x => x.TenDanhMuc == dm.TenDM);

                foreach (var sp in spThuocDM)
                {
                    TreeNode nodeSP = new TreeNode($"{sp.MaSP} - {sp.TenSP}") { Tag = sp };
                    nodeDM.Nodes.Add(nodeSP);
                }

                tvDanhMuc.Nodes.Add(nodeDM);
            }

            tvDanhMuc.ExpandAll();
        }

        private void KhoiTaoContextMenu()
        {
            if (cmsTree == null) return;

            cmsTree.Items.Clear();
            cmsTree.Items.Add("➕ Thêm Danh mục", null, Context_ThemDanhMuc_Click);
            cmsTree.Items.Add("➕ Thêm Sản phẩm", null, Context_ThemSanPham_Click);
            cmsTree.Items.Add("-");
            cmsTree.Items.Add("✏️ Đổi tên Node", null, Context_DoiTenNode_Click);
            cmsTree.Items.Add("❌ Xóa Node", null, Context_XoaNode_Click);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (KiemTraTrong()) return;

                SanPham sp = LayThongTinForm();
                _spBUS.Add(sp);

                HienThiDanhSach(_spBUS.GetAll());
                NapTreeView();
                XoaTrangForm();
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (KiemTraTrong()) return;

                SanPham sp = LayThongTinForm();
                _spBUS.Update(sp);

                HienThiDanhSach(_spBUS.GetAll());
                NapTreeView();
                XoaTrangForm();
                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSanPham != null && dgvSanPham.CurrentRow == null && string.IsNullOrEmpty(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maSP = txtMaSP.Text.Trim();
            if (string.IsNullOrEmpty(maSP) && dgvSanPham?.CurrentRow != null)
            {
                maSP = dgvSanPham.CurrentRow.Cells["MaSP"].Value.ToString();
            }

            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm [{maSP}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    _spBUS.Delete(maSP);
                    HienThiDanhSach(_spBUS.GetAll());
                    NapTreeView();
                    XoaTrangForm();
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrangForm();
            if (txtTimKiem != null) txtTimKiem.Clear();
            if (cboSapXep != null && cboSapXep.Items.Count > 0) cboSapXep.SelectedIndex = 0;
            HienThiDanhSach(_spBUS.GetAll());
            NapTreeView();
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            if (txtTimKiem == null) return;
            var ketQua = _spBUS.Search(txtTimKiem.Text);
            HienThiDanhSach(ketQua);
        }

        private void cboSapXep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSapXep?.SelectedItem != null)
            {
                var dsSapXep = _spBUS.Sort(cboSapXep.SelectedItem.ToString());
                HienThiDanhSach(dsSapXep);
            }
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            HienThiChiTiet(e.RowIndex);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            HienThiChiTiet(e.RowIndex);
        }

        private void HienThiChiTiet(int rowIndex)
        {
            if (dgvSanPham != null && rowIndex >= 0 && dgvSanPham.Rows[rowIndex].DataBoundItem is SanPham sp)
            {
                txtMaSP.Text = sp.MaSP;
                txtTenSP.Text = sp.TenSP;
                txtDonGia.Text = sp.DonGia.ToString();
                txtSoLuong.Text = sp.SoLuong.ToString();
                if (cboDanhMuc != null && cboDanhMuc.Items.Contains(sp.TenDanhMuc))
                    cboDanhMuc.SelectedItem = sp.TenDanhMuc;
            }
        }

        private void tvDanhMuc_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is SanPham sp)
            {
                txtMaSP.Text = sp.MaSP;
                txtTenSP.Text = sp.TenSP;
                txtDonGia.Text = sp.DonGia.ToString();
                txtSoLuong.Text = sp.SoLuong.ToString();
                if (cboDanhMuc != null && cboDanhMuc.Items.Contains(sp.TenDanhMuc))
                    cboDanhMuc.SelectedItem = sp.TenDanhMuc;
            }
            else if (e.Node?.Tag is DanhMuc dm)
            {
                if (cboDanhMuc != null && cboDanhMuc.Items.Contains(dm.TenDM))
                    cboDanhMuc.SelectedItem = dm.TenDM;
            }
        }

        private void tvDanhMuc_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tvDanhMuc != null)
            {
                tvDanhMuc.SelectedNode = e.Node;
            }
        }

        // --- CÁC THAO TÁC CONTEXTMENUSTRIP SỬ DỤNG HÀM HỖ TRỢ InputBox ---
        private void Context_ThemDanhMuc_Click(object sender, EventArgs e)
        {
            string tenDM = InputBox("Nhập tên Danh mục mới:", "Thêm Danh mục");
            if (!string.IsNullOrWhiteSpace(tenDM))
            {
                _dmBUS.Add(new DanhMuc(Guid.NewGuid().ToString("N").Substring(0, 5), tenDM.Trim()));
                NapComboboxDanhMuc();
                NapTreeView();
            }
        }

        private void Context_ThemSanPham_Click(object sender, EventArgs e)
        {
            if (tvDanhMuc?.SelectedNode?.Tag is DanhMuc dm)
            {
                txtMaSP.Text = "SP" + (new Random().Next(100, 999));
                txtTenSP.Focus();
                if (cboDanhMuc != null && cboDanhMuc.Items.Contains(dm.TenDM))
                    cboDanhMuc.SelectedItem = dm.TenDM;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một Danh mục gốc để thêm sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Context_DoiTenNode_Click(object sender, EventArgs e)
        {
            if (tvDanhMuc?.SelectedNode?.Tag is DanhMuc dm)
            {
                string tenMoi = InputBox("Nhập tên mới cho Danh mục:", "Đổi tên Node", dm.TenDM);
                if (!string.IsNullOrWhiteSpace(tenMoi))
                {
                    dm.TenDM = tenMoi.Trim();
                    _dmBUS.Update(dm);
                    NapComboboxDanhMuc();
                    NapTreeView();
                }
            }
            else if (tvDanhMuc?.SelectedNode?.Tag is SanPham sp)
            {
                string tenMoi = InputBox("Nhập tên mới cho Sản phẩm:", "Đổi tên Node", sp.TenSP);
                if (!string.IsNullOrWhiteSpace(tenMoi))
                {
                    sp.TenSP = tenMoi.Trim();
                    _spBUS.Update(sp);
                    HienThiDanhSach(_spBUS.GetAll());
                    NapTreeView();
                }
            }
        }

        private void Context_XoaNode_Click(object sender, EventArgs e)
        {
            if (tvDanhMuc?.SelectedNode?.Tag is DanhMuc dm)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa Danh mục [{dm.TenDM}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _dmBUS.Delete(dm.MaDM);
                    NapComboboxDanhMuc();
                    NapTreeView();
                }
            }
            else if (tvDanhMuc?.SelectedNode?.Tag is SanPham sp)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa Sản phẩm [{sp.TenSP}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _spBUS.Delete(sp.MaSP);
                    HienThiDanhSach(_spBUS.GetAll());
                    NapTreeView();
                }
            }
        }

        // HÀM TẠO INPUTBOX TỰ DỊCH DÙNG THUẦN WINFORMS
        private string InputBox(string prompt, string title, string defaultValue = "")
        {
            Form form = new Form()
            {
                Width = 380,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label label = new Label() { Left = 20, Top = 15, Text = prompt, AutoSize = true };
            TextBox textBox = new TextBox() { Left = 20, Top = 40, Width = 320, Text = defaultValue };
            Button buttonOk = new Button() { Text = "OK", Left = 170, Width = 80, Top = 75, DialogResult = DialogResult.OK };
            Button buttonCancel = new Button() { Text = "Hủy", Left = 260, Width = 80, Top = 75, DialogResult = DialogResult.Cancel };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(buttonOk);
            form.Controls.Add(buttonCancel);
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private SanPham LayThongTinForm()
        {
            string tenDM = cboDanhMuc != null && cboDanhMuc.SelectedItem != null
                         ? cboDanhMuc.SelectedItem.ToString()
                         : "Điện thoại & Tablet";

            return new SanPham(
                txtMaSP.Text.Trim(),
                txtTenSP.Text.Trim(),
                double.Parse(txtDonGia.Text.Trim()),
                int.Parse(txtSoLuong.Text.Trim()),
                tenDM
            );
        }

        private bool KiemTraTrong()
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                string.IsNullOrWhiteSpace(txtDonGia.Text) ||
                string.IsNullOrWhiteSpace(txtSoLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            if (!double.TryParse(txtDonGia.Text.Trim(), out double donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số không âm!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong < 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên không âm!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            return false;
        }

        private void XoaTrangForm()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtDonGia.Clear();
            txtSoLuong.Clear();
            if (cboDanhMuc != null && cboDanhMuc.Items.Count > 0) cboDanhMuc.SelectedIndex = 0;
            txtMaSP.Focus();
        }

        private void txtMaSP_TextChanged(object sender, EventArgs e) { }
        private void txtTenSP_TextChanged(object sender, EventArgs e) { }
        private void txtDonGia_TextChanged(object sender, EventArgs e) { }
        private void txtSoLuong_TextChanged(object sender, EventArgs e) { }
        private void lblTongTien_Click(object sender, EventArgs e) { }
        private void frmSanPham_Load_1(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmsTree_Opening(object sender, CancelEventArgs e) { }
    }
}