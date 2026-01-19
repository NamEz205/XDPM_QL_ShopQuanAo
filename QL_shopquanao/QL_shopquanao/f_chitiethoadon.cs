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
                // 1. Lấy dữ liệu từ giao diện
                string maSP = cboSanPham.SelectedValue.ToString();
                int soLuongBan = int.Parse(txtSoLuong.Text);
                double donGia = double.Parse(txtDonGia.Text);

                // 2. Kiểm tra số lượng tồn kho trước khi bán
                DataTable dtKho = db.getTable($"SELECT SoLuongTon FROM Kho WHERE MaSanPham = '{maSP}'");
                if (dtKho.Rows.Count > 0)
                {
                    int tonKho = int.Parse(dtKho.Rows[0]["SoLuongTon"].ToString());
                    if (tonKho < soLuongBan)
                    {
                        MessageBox.Show($"Không đủ hàng trong kho! Hiện tại chỉ còn {tonKho} sản phẩm.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Sản phẩm này chưa có trong kho!");
                    return;
                }

                // 3. Thêm vào bảng ChiTietHoaDon (Bỏ cột ThanhTien vì là cột tính toán tự động)
                string sqlInsert = $@"INSERT INTO ChiTietHoaDon (HoaDonID, MaSanPham, SoLuong, DonGia) 
                              VALUES ('{MaHD_TruyenSang}', '{maSP}', {soLuongBan}, {donGia})";
                db.ExcuteNonQuery(sqlInsert);

                // 4. CẬP NHẬT KHO: Trừ số lượng tồn
                string sqlUpdateKho = $"UPDATE Kho SET SoLuongTon = SoLuongTon - {soLuongBan} WHERE MaSanPham = '{maSP}'";
                db.ExcuteNonQuery(sqlUpdateKho);

                // 5. Cập nhật lại tổng tiền cho bảng HoaDon
                CapNhatTongTien();

                // 6. Làm mới hiển thị
                MessageBox.Show("Thêm sản phẩm và cập nhật kho thành công!");
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.CurrentRow == null) return;

            try
            {
                // 1. Lấy thông tin từ dòng đang chọn trước khi xóa
                string maCT = dgvChiTiet.CurrentRow.Cells["ChiTietID"].Value.ToString();
                string maSP = dgvChiTiet.CurrentRow.Cells["MaSanPham"].Value.ToString();
                int soLuongTrongHD = int.Parse(dgvChiTiet.CurrentRow.Cells["SoLuong"].Value.ToString());

                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này khỏi hóa đơn?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // 2. Xóa trong bảng ChiTietHoaDon
                    db.ExcuteNonQuery($"DELETE FROM ChiTietHoaDon WHERE ChiTietID = '{maCT}'");

                    // 3. CẬP NHẬT KHO: Cộng trả lại số lượng
                    string sqlTraKho = $"UPDATE Kho SET SoLuongTon = SoLuongTon + {soLuongTrongHD} WHERE MaSanPham = '{maSP}'";
                    db.ExcuteNonQuery(sqlTraKho);

                    // 4. Cập nhật lại tổng tiền hóa đơn
                    CapNhatTongTien();

                    MessageBox.Show("Đã xóa sản phẩm và hoàn trả số lượng vào kho!");
                    LoadChiTiet();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
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
                // 1. Lấy mã chi tiết và thông tin cũ/mới
                string maCT = txtMaChiTiet.Text; // Ô chứa mã chi tiết (ẩn hoặc readonly)
                string maSP = cboSanPham.SelectedValue.ToString();
                int soLuongMoi = int.Parse(txtSoLuong.Text);
                double donGia = double.Parse(txtDonGia.Text);

                // 2. Lấy số lượng cũ đang có trong database của chi tiết này
                DataTable dtCu = db.getTable($"SELECT SoLuong FROM ChiTietHoaDon WHERE ChiTietID = '{maCT}'");
                int soLuongCu = int.Parse(dtCu.Rows[0]["SoLuong"].ToString());

                // 3. Tính toán chênh lệch
                int chenhLech = soLuongMoi - soLuongCu;

                // 4. Kiểm tra kho nếu trường hợp mua thêm (chenhLech > 0)
                if (chenhLech > 0)
                {
                    DataTable dtKho = db.getTable($"SELECT SoLuongTon FROM Kho WHERE MaSanPham = '{maSP}'");
                    int tonKho = int.Parse(dtKho.Rows[0]["SoLuongTon"].ToString());
                    if (tonKho < chenhLech)
                    {
                        MessageBox.Show("Kho không đủ hàng để tăng số lượng!");
                        return;
                    }
                }

                // 5. Cập nhật ChiTietHoaDon
                string sqlUpdateCT = $@"UPDATE ChiTietHoaDon 
                                SET SoLuong = {soLuongMoi}, DonGia = {donGia} 
                                WHERE ChiTietID = '{maCT}'";
                db.ExcuteNonQuery(sqlUpdateCT);

                // 6. CẬP NHẬT KHO: Trừ đi phần chênh lệch (nếu chenhLech âm thì trừ của âm sẽ thành cộng)
                string sqlUpdateKho = $"UPDATE Kho SET SoLuongTon = SoLuongTon - {chenhLech} WHERE MaSanPham = '{maSP}'";
                db.ExcuteNonQuery(sqlUpdateKho);

                // 7. Cập nhật tổng tiền hóa đơn
                CapNhatTongTien();

                MessageBox.Show("Cập nhật số lượng và kho thành công!");
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
        }
    }
}
