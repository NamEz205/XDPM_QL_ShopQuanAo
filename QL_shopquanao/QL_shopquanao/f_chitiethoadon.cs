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
    public partial class f_chitiethoadon : Form
    {
        string MaHD_TruyenSang;

        public f_chitiethoadon(string maHD_TruyenSang)
        {
            InitializeComponent();
            MaHD_TruyenSang = maHD_TruyenSang;
        }
        Data db = new Data();

        void LoadSanPham()
        {
            DataTable dt = db.getTable("SELECT * FROM SanPham");
            cboSanPham.DisplayMember = "TenSanPham";
            cboSanPham.ValueMember = "MaSanPham";
            cboSanPham.DataSource = dt;
        }

        void LoadChiTiet()
        {
            // Chỉ load những sản phẩm thuộc hóa đơn đang chọn
            string sql = $@"SELECT ct.MaSanPham, sp.TenSanPham, ct.SoLuong, ct.DonGia, ct.ThanhTien 
                    FROM ChiTietHoaDon ct 
                    JOIN SanPham sp ON ct.MaSanPham = sp.MaSanPham 
                    WHERE ct.HoaDonID = '{MaHD_TruyenSang}'";
            dgvChiTiet.DataSource = db.getTable(sql);

            // Gán mã hóa đơn lên TextBox (nếu có) để người dùng biết
            txtMaHD.Text = MaHD_TruyenSang;
        }

        void loadgrid()
        {
            string sql = $@"select * from ChiTietHoaDon";
            dgvChiTiet.DataSource = db.getTable(sql);
            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

      
        private void f_chitiethoadon_Load(object sender, EventArgs e)
        {
            LoadSanPham();
            LoadChiTiet();
            loadgrid();
        }

        private void cboSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSanPham.SelectedValue != null)
            {
                string maSP = cboSanPham.SelectedValue.ToString();
                // Lấy giá bán từ bảng SanPham dựa trên mã vừa chọn
                DataTable dt = db.getTable($"SELECT GiaBan FROM SanPham WHERE MaSanPham = '{maSP}'");
                if (dt.Rows.Count > 0)
                {
                    txtDonGia.Text = dt.Rows[0]["GiaBan"].ToString();
                }
            }
        }

        void CapNhatTongTien()
        {
            // Tính tổng tiền mới từ các chi tiết
            string sqlSum = $"SELECT SUM(ThanhTien) FROM ChiTietHoaDon WHERE HoaDonID = '{MaHD_TruyenSang}'";
            DataTable dt = db.getTable(sqlSum);
            string tongTienMoi = dt.Rows[0][0].ToString();
            if (string.IsNullOrEmpty(tongTienMoi)) tongTienMoi = "0";

            // Update ngược lại bảng HoaDon
            db.ExcuteNonQuery($"UPDATE HoaDon SET TongTien = {tongTienMoi} WHERE HoaDonID = '{MaHD_TruyenSang}'");
        }
        private void btthem_Click(object sender, EventArgs e)
        {
            try
            {
                string maSP = cboSanPham.SelectedValue.ToString();
                int soLuong = int.Parse(txtSoLuong.Text);
                double donGia = double.Parse(txtDonGia.Text);

                // BỎ 'ThanhTien' ra khỏi danh sách cột và danh sách giá trị
                string sqlInsert = $@"INSERT INTO ChiTietHoaDon (HoaDonID, MaSanPham, SoLuong, DonGia) 
                              VALUES ('{MaHD_TruyenSang}', '{maSP}', {soLuong}, {donGia})";

                db.ExcuteNonQuery(sqlInsert);

                // Sau khi chèn xong, tính lại tổng tiền cho bảng HoaDon như cũ
                CapNhatTongTien();

                MessageBox.Show("Thêm sản phẩm thành công!");
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string maSP = cboSanPham.SelectedValue.ToString(); // Hoặc lấy từ GridView

            if (MessageBox.Show("Xóa sản phẩm này khỏi hóa đơn?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = $"DELETE FROM ChiTietHoaDon WHERE HoaDonID = '{MaHD_TruyenSang}' AND MaSanPham = '{maSP}'";
                db.ExcuteNonQuery(sql);

                CapNhatTongTien(); // Tính lại tổng tiền sau khi xóa
                LoadChiTiet();
            }
        }

        private void dgvChiTiet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                cboSanPham.SelectedValue = dgvChiTiet.Rows[i].Cells["MaSanPham"].Value.ToString();
                txtSoLuong.Text = dgvChiTiet.Rows[i].Cells["SoLuong"].Value.ToString();
                txtDonGia.Text = dgvChiTiet.Rows[i].Cells["DonGia"].Value.ToString();
            }
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy mã ChiTietID từ ô nhập liệu hoặc GridView
                string maCT = txtMaChiTiet.Text; // Giả sử bạn có ô chứa mã chi tiết
                if (string.IsNullOrEmpty(maCT))
                {
                    MessageBox.Show("Vui lòng chọn bản ghi cần sửa từ danh sách!");
                    return;
                }

                // 2. Lấy dữ liệu mới từ giao diện
                int soLuong = int.Parse(txtSoLuong.Text);
                double donGia = double.Parse(txtDonGia.Text);

                // 3. Thực hiện câu lệnh UPDATE (Không update ThanhTien)
                string sqlUpdate = $@"UPDATE ChiTietHoaDon 
                             SET SoLuong = {soLuong}, 
                                 DonGia = {donGia} 
                             WHERE ChiTietID = '{maCT}'";

                db.ExcuteNonQuery(sqlUpdate);

                // 4. Cập nhật lại tổng tiền cho bảng HoaDon
                CapNhatTongTien();

                MessageBox.Show("Cập nhật chi tiết hóa đơn thành công!");
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
        }
    }
}
