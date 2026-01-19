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
    public partial class f_nhanvien : Form
    {
        public f_nhanvien()
        {
            InitializeComponent();
        }
        Data db = new Data();

        void LoadNhanVien()
        {
            string sql = "SELECT * FROM NhanVien";
            dgvNhanVien.DataSource = db.getTable(sql);
            dgvNhanVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        void ClearText()
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            txtSDT.Clear();
            txtChucVu.Clear();
            dtpNgayVaoLam.Value = DateTime.Now; // Đưa về ngày hiện tại
            txtMaNV.Focus();
        }

        private void f_nhanvien_Load(object sender, EventArgs e)
        {
            LoadNhanVien();
           

        }

        private void btthem_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;
            string ten = txtHoTen.Text;
            string sdt = txtSDT.Text;
            string chucvu = txtChucVu.Text;
            // Định dạng ngày để SQL Server hiểu đúng
            string ngay = dtpNgayVaoLam.Value.ToString("yyyy-MM-dd");

            if ( string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Vui lòng nhập Mã và Họ tên nhân viên!");
                return;
            }

            string sql = $"INSERT INTO NhanVien VALUES (N'{ten}', '{sdt}', N'{chucvu}', '{ngay}')";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Thêm nhân viên thành công!");
            LoadNhanVien();
            ClearText();
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;
            string ten = txtHoTen.Text;
            string sdt = txtSDT.Text;
            string chucvu = txtChucVu.Text;
            string ngay = dtpNgayVaoLam.Value.ToString("yyyy-MM-dd");

            string sql = $"UPDATE NhanVien SET HoTen = N'{ten}', SoDienThoai = '{sdt}', ChucVu = N'{chucvu}', NgayVaoLam = '{ngay}' WHERE NhanVienID = '{ma}'";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Cập nhật nhân viên thành công!");
            LoadNhanVien();

        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = $"DELETE FROM NhanVien WHERE NhanVienID = '{ma}'";
                db.ExcuteNonQuery(sql);
                LoadNhanVien();
                ClearText();
            }
        }

        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                txtMaNV.Text = dgvNhanVien.Rows[i].Cells["NhanVienID"].Value.ToString();
                txtHoTen.Text = dgvNhanVien.Rows[i].Cells["HoTen"].Value.ToString();
                txtSDT.Text = dgvNhanVien.Rows[i].Cells["SoDienThoai"].Value.ToString();
                txtChucVu.Text = dgvNhanVien.Rows[i].Cells["ChucVu"].Value.ToString();

                // Xử lý hiển thị ngày tháng
                if (dgvNhanVien.Rows[i].Cells["NgayVaoLam"].Value != DBNull.Value)
                {
                    dtpNgayVaoLam.Value = Convert.ToDateTime(dgvNhanVien.Rows[i].Cells["NgayVaoLam"].Value);
                }
            }
        }
    }
}
