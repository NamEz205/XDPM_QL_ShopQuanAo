using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_shopquanao
{
    public partial class f_sanpham : Form
    {
        public f_sanpham()
        {
            InitializeComponent();
        }

        Data db = new Data();
        void LoadComboBox()
        {
            string sql = "SELECT * FROM DanhMuc";
            DataTable dt = db.getTable(sql);
            cboDanhMuc.DataSource = dt;
            cboDanhMuc.DisplayMember = "TenDanhMuc"; // Hiển thị tên
            cboDanhMuc.ValueMember = "DanhMucID";    // Giá trị lấy để lưu là mã
        }
        void LoadSanPham()
        {
            // Join với bảng DanhMuc để lấy cột TenDanhMuc
            string sql = @"SELECT s.*, d.TenDanhMuc 
                   FROM SanPham s 
                   INNER JOIN DanhMuc d ON s.DanhMucID = d.DanhMucID";
            dgvSanPham.DataSource = db.getTable(sql);
            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        void ClearText()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            txtGiaNhap.Clear();
            txtGiaBan.Clear();
            txtChatLieu.Clear();
            // Reset các combo box
            cboDanhMuc.SelectedIndex = 0;
            cboKichThuoc.SelectedIndex = -1;
            cboMauSac.SelectedIndex = -1;
            dtpNgayNhap.Value = DateTime.Now;
            txtMaSP.Focus();
        }
        private void f_sanpham_Load(object sender, EventArgs e)
        {
            LoadComboBox();
            LoadSanPham();

        }

        private void btthem_Click(object sender, EventArgs e)
        {
            string sql = $@"INSERT INTO SanPham 
        VALUES (N'{txtTenSP.Text}', '{cboDanhMuc.SelectedValue}', 
        N'{cboKichThuoc.Text}', N'{cboMauSac.Text}', N'{txtChatLieu.Text}', 
        {txtGiaNhap.Text}, {txtGiaBan.Text}, '{dtpNgayNhap.Value:yyyy-MM-dd}')";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Thêm sản phẩm thành công!");
            LoadSanPham();
            ClearText();
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            string sql = $@"UPDATE SanPham SET 
        TenSanPham = N'{txtTenSP.Text}', 
        DanhMucID = '{cboDanhMuc.SelectedValue}', 
        KichThuoc = N'{cboKichThuoc.Text}', 
        MauSac = N'{cboMauSac.Text}', 
        ChatLieu = N'{txtChatLieu.Text}', 
        Gianhap = {txtGiaNhap.Text}, 
        GiaBan = {txtGiaBan.Text} 
        WHERE MaSanPham = '{txtMaSP.Text}'";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Cập nhật thành công!");
            LoadSanPham();
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text;

            // 2. Kiểm tra xem người dùng đã chọn sản phẩm chưa
            if (string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa từ danh sách!", "Thông báo");
                return;
            }

            // 3. Hiển thị hộp thoại xác nhận xóa
            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm có mã: {maSP} không?\nLưu ý: Hành động này không thể hoàn tác!",
                                              "Xác nhận xóa",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    // 4. Thực hiện câu lệnh SQL xóa
                    string sql = $"DELETE FROM SanPham WHERE MaSanPham = '{maSP}'";
                    db.ExcuteNonQuery(sql);

                    // 5. Thông báo và làm mới giao diện
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo");
                    LoadSanPham(); // Hàm load lại DataGridView đã viết ở bước trước
                    ClearText();   // Hàm xóa trống các ô nhập liệu đã viết ở bước trước
                }
                catch (Exception ex)
                {
                    // Trường hợp lỗi khóa ngoại (sản phẩm đã nằm trong hóa đơn)
                    MessageBox.Show("Không thể xóa sản phẩm này vì nó đã tồn tại trong lịch sử hóa đơn.\nChi tiết lỗi: " + ex.Message,
                                    "Lỗi hệ thống",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }

        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                txtMaSP.Text = dgvSanPham.Rows[i].Cells["MaSanPham"].Value.ToString();
                txtTenSP.Text = dgvSanPham.Rows[i].Cells["TenSanPham"].Value.ToString();

                // Gán giá trị cho ComboBox dựa trên mã ID trong bảng
                cboDanhMuc.SelectedValue = dgvSanPham.Rows[i].Cells["DanhMucID"].Value.ToString();

                cboKichThuoc.Text = dgvSanPham.Rows[i].Cells["KichThuoc"].Value.ToString();
                cboMauSac.Text = dgvSanPham.Rows[i].Cells["MauSac"].Value.ToString();
                txtChatLieu.Text = dgvSanPham.Rows[i].Cells["ChatLieu"].Value.ToString();
                txtGiaNhap.Text = dgvSanPham.Rows[i].Cells["Gianhap"].Value.ToString();
                txtGiaBan.Text = dgvSanPham.Rows[i].Cells["GiaBan"].Value.ToString();
            }
        }
    }
}
