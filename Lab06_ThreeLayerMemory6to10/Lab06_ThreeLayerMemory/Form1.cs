using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Lab06_ThreeLayerMemory.BUS;
using Lab06_ThreeLayerMemory.DTO;

namespace Lab06_ThreeLayerMemory
{
    public partial class frmSanPham : Form
    {
        private readonly SanPhamBUS _bus = new SanPhamBUS();

        public frmSanPham()
        {
            InitializeComponent();
            this.Load += frmSanPham_Load;

            // Liên kết sự kiện CellClick với tên dgvSanPham
            if (dgvSanPham != null)
            {
                this.dgvSanPham.CellClick += dgvSanPham_CellClick;
            }
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            cboSapXep.Items.Clear();
            cboSapXep.Items.Add("Tên A-Z");
            cboSapXep.Items.Add("Giá tăng dần");
            cboSapXep.Items.Add("Giá giảm dần");
            cboSapXep.Items.Add("Số lượng giảm dần");
            cboSapXep.SelectedIndex = 0;

            if (_bus.GetAll().Count == 0)
            {
                _bus.Add(new SanPham("SP01", "Laptop Dell XPS", 25000000, 5));
                _bus.Add(new SanPham("SP02", "Chuột Logitech MX", 1800000, 20));
                _bus.Add(new SanPham("SP03", "Bàn phím Keychron", 2200000, 12));
            }

            HienThiDanhSach(_bus.GetAll());
        }

        private void HienThiDanhSach(List<SanPham> list)
        {
            dgvSanPham.DataSource = new BindingList<SanPham>(list);
            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPham.ReadOnly = true;

            double tongTien = _bus.GetTongGiaTriTonKho();
            lblTongTien.Text = $"Tổng giá trị hàng tồn kho: {tongTien:N0} VNĐ";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (KiemTraTrong()) return;

                SanPham sp = LayThongTinForm();
                _bus.Add(sp);
                HienThiDanhSach(_bus.GetAll());
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
                _bus.Update(sp);
                HienThiDanhSach(_bus.GetAll());
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
            if (dgvSanPham.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maSP = txtMaSP.Text.Trim();
            if (string.IsNullOrEmpty(maSP))
            {
                maSP = dgvSanPham.CurrentRow.Cells["MaSP"].Value.ToString();
            }

            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm [{maSP}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    _bus.Delete(maSP);
                    HienThiDanhSach(_bus.GetAll());
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
            txtTimKiem.Clear();
            if (cboSapXep.Items.Count > 0) cboSapXep.SelectedIndex = 0;
            HienThiDanhSach(_bus.GetAll());
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            var ketQua = _bus.Search(txtTimKiem.Text);
            HienThiDanhSach(ketQua);
        }

        private void cboSapXep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSapXep.SelectedItem != null)
            {
                var dsSapXep = _bus.Sort(cboSapXep.SelectedItem.ToString());
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
            if (rowIndex >= 0 && dgvSanPham.Rows[rowIndex].DataBoundItem is SanPham sp)
            {
                txtMaSP.Text = sp.MaSP;
                txtTenSP.Text = sp.TenSP;
                txtDonGia.Text = sp.DonGia.ToString();
                txtSoLuong.Text = sp.SoLuong.ToString();
            }
        }

        private SanPham LayThongTinForm()
        {
            return new SanPham(
                txtMaSP.Text.Trim(),
                txtTenSP.Text.Trim(),
                double.Parse(txtDonGia.Text.Trim()),
                int.Parse(txtSoLuong.Text.Trim())
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
            txtMaSP.Focus();
        }

        private void txtMaSP_TextChanged(object sender, EventArgs e) { }
        private void txtTenSP_TextChanged(object sender, EventArgs e) { }
        private void txtDonGia_TextChanged(object sender, EventArgs e) { }
        private void txtSoLuong_TextChanged(object sender, EventArgs e) { }
        private void lblTongTien_Click(object sender, EventArgs e) { }
    }
}