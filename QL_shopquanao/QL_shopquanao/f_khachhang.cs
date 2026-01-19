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
    public partial class f_khachhang : Form
    {
        public f_khachhang()
        {
            InitializeComponent();
        }
        Data db = new Data();

        void LoadKhachHang()
        {
            string sql = "SELECT * FROM KhachHang";
            dataGridView1.DataSource = db.getTable(sql);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
        }

        void ClearText()
        {
            txtMaKH.Clear();
            txtHoTen.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            txtDiem.Clear();
            txtMaKH.Focus();
        }

    
        private void f_khachhang_Load(object sender, EventArgs e)
        {
            LoadKhachHang();

        }

        private void btthem_Click(object sender, EventArgs e)
        {
            string ma = txtMaKH.Text;
            string ten = txtHoTen.Text;
            string sdt = txtSDT.Text;
            string diachi = txtDiaChi.Text;
            string diem = string.IsNullOrEmpty(txtDiem.Text) ? "0" : txtDiem.Text;

            if (string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Vui lòng nhập Mã và Họ tên khách hàng!");
                return;
            }

            string sql = $"INSERT INTO KhachHang VALUES (N'{ten}', '{sdt}', N'{diachi}', {diem})";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Thêm khách hàng thành công!");
            LoadKhachHang();
            ClearText();

        }

        private void btsua_Click(object sender, EventArgs e)
        {
            string ma = txtMaKH.Text;
            string ten = txtHoTen.Text;
            string sdt = txtSDT.Text;
            string diachi = txtDiaChi.Text;
            string diem = string.IsNullOrEmpty(txtDiem.Text) ? "0" : txtDiem.Text;

            string sql = $"UPDATE KhachHang SET HoTen = N'{ten}', SoDienThoai = '{sdt}', DiaChi = N'{diachi}', DiemTichLuy = {diem} WHERE KhachHangID = '{ma}'";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Cập nhật thông tin thành công!");
            LoadKhachHang();
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string ma = txtMaKH.Text;

            if (MessageBox.Show("Bạn có muốn xóa khách hàng này không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = $"DELETE FROM KhachHang WHERE KhachHangID = '{ma}'";
                db.ExcuteNonQuery(sql);
                LoadKhachHang();
                ClearText();
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                txtMaKH.Text = dataGridView1.Rows[i].Cells["KhachHangID"].Value.ToString();
                txtHoTen.Text = dataGridView1.Rows[i].Cells["HoTen"].Value.ToString();
                txtSDT.Text = dataGridView1.Rows[i].Cells["SoDienThoai"].Value.ToString();
                txtDiaChi.Text = dataGridView1.Rows[i].Cells["DiaChi"].Value.ToString();
                txtDiem.Text = dataGridView1.Rows[i].Cells["DiemTichLuy"].Value.ToString();
            }
        }
    }
}
