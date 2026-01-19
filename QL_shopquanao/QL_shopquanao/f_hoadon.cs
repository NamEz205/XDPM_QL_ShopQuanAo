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
    public partial class f_hoadon : Form
    {
        public f_hoadon()
        {
            InitializeComponent();
        }
        Data db = new Data();

        void LoadComboBox()
        {
            // Load Khách hàng
            DataTable dt= db.getTable("SELECT * FROM KhachHang");
            cboKhachHang.DisplayMember = "HoTen";
            cboKhachHang.ValueMember = "KhachHangID";
            cboKhachHang.DataSource = dt;

            // Load Nhân viên
            DataTable da = db.getTable("SELECT * FROM NhanVien");
            cboNhanVien.DisplayMember = "HoTen";
            cboNhanVien.ValueMember = "NhanVienID";
            cboNhanVien.DataSource = da;
        }

        void LoadHoaDon()
        {
            // Join để hiển thị tên khách hàng và nhân viên thay vì chỉ hiển thị mã
            string sql = @"SELECT h.HoaDonID, h.NgayLap, k.HoTen AS TenKhachHang, 
                          n.HoTen AS TenNhanVien, h.TongTien, h.GhiChu
                   FROM HoaDon h
                   LEFT JOIN KhachHang k ON h.KhachHangID = k.KhachHangID
                   LEFT JOIN NhanVien n ON h.NhanVienID = n.NhanVienID";
            dgvHoaDon.DataSource = db.getTable(sql);
            dgvHoaDon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void f_hoadon_Load(object sender, EventArgs e)
        {
            LoadComboBox();
            LoadHoaDon();
        }

        void clearText()
        {
            txtMaHD.Clear();
            dtpNgayLap.Value = DateTime.Now;
            cboKhachHang.SelectedIndex = 0;
            cboNhanVien.SelectedIndex = 0;
            txtTongTien.Clear();
            txtGhiChu.Clear();
            txtMaHD.Focus();
        }

        private void btthem_Click(object sender, EventArgs e)
        {
            string maHD = txtMaHD.Text;
            string ngayLap = dtpNgayLap.Value.ToString("yyyy-MM-dd");
            string maKH = cboKhachHang.SelectedValue.ToString();
            string maNV = cboNhanVien.SelectedValue.ToString();
            string ghiChu = txtGhiChu.Text;
            // Mặc định tổng tiền mới tạo là 0 nếu chưa có chi tiết
            string tongTien = string.IsNullOrEmpty(txtTongTien.Text) ? "0" : txtTongTien.Text;

          

            string sql = $"INSERT INTO HoaDon VALUES ('{ngayLap}', '{maKH}', '{maNV}', {tongTien}, N'{ghiChu}')";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Thêm hóa đơn thành công!");
            LoadHoaDon();
            clearText();
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            string sql = $@"UPDATE HoaDon SET 
                    NgayLap = '{dtpNgayLap.Value:yyyy-MM-dd}', 
                    KhachHangID = '{cboKhachHang.SelectedValue}', 
                    NhanVienID = '{cboNhanVien.SelectedValue}', 
                    GhiChu = N'{txtGhiChu.Text}' 
                    WHERE HoaDonID = '{txtMaHD.Text}'";
            db.ExcuteNonQuery(sql);
            LoadHoaDon();
            clearText();
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa hóa đơn sẽ xóa tất cả chi tiết hóa đơn liên quan. Bạn có chắc chắn?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Xóa chi tiết trước để tránh lỗi khóa ngoại
                db.ExcuteNonQuery($"DELETE FROM ChiTietHoaDon WHERE HoaDonID = '{txtMaHD.Text}'");
                // Sau đó xóa hóa đơn
                db.ExcuteNonQuery($"DELETE FROM HoaDon WHERE HoaDonID = '{txtMaHD.Text}'");
                LoadHoaDon();
                clearText();
            }
        }

        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        

        private void dgvHoaDon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                // Lấy mã trực tiếp từ GridView là cách an toàn nhất
                string maHD = dgvHoaDon.Rows[i].Cells["HoaDonID"].Value.ToString();

                // Mở form chi tiết
                f_chitiethoadon f = new f_chitiethoadon(maHD);
                f.ShowDialog();

                // Sau khi đóng form chi tiết, tải lại Grid hóa đơn để cập nhật tổng tiền
                LoadHoaDon();
            }

        }
    }
}
