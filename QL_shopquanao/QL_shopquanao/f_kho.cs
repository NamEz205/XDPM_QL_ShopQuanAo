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
    public partial class f_kho : Form
    {
        public f_kho()
        {
            InitializeComponent();
        }
        Data db = new Data();

        void LoadComboBoxSP()
        {
            string sql = "SELECT MaSanPham, TenSanPham FROM SanPham";
            DataTable dt = db.getTable(sql);
            cboMaSP.DataSource = dt;
            cboMaSP.DisplayMember = "TenSanPham"; // Hiển thị tên sản phẩm
            cboMaSP.ValueMember = "MaSanPham";    // Giá trị thực tế là mã
        }

        void LoadKho()
        {
            // Join với bảng SanPham để hiển thị tên cho dễ nhìn
            string sql = @"SELECT k.ID, k.MaSanPham, s.TenSanPham, k.SoLuongTon 
                   FROM Kho k 
                   INNER JOIN SanPham s ON k.MaSanPham = s.MaSanPham";
            dgvKho.DataSource = db.getTable(sql);
            dgvKho.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btthem_Click(object sender, EventArgs e)
        {
            string maSP = cboMaSP.SelectedValue.ToString();
            string soLuong = txtSoLuong.Text;

            if (string.IsNullOrEmpty(soLuong))
            {
                MessageBox.Show("Vui lòng nhập số lượng tồn!");
                return;
            }

            string sql = $"INSERT INTO Kho (MaSanPham, SoLuongTon) VALUES ('{maSP}', {soLuong})";
            db.ExcuteNonQuery(sql);

            MessageBox.Show("Đã cập nhật kho thành công!");
            LoadKho();
        }

        private void f_kho_Load(object sender, EventArgs e)
        {
            LoadComboBoxSP();
            LoadKho();
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            // Lấy ID từ dòng đang chọn trong GridView (vì bảng Kho có khóa chính là ID)
            string id = dgvKho.CurrentRow.Cells["ID"].Value.ToString();
            string maSP = cboMaSP.SelectedValue.ToString();
            string soLuong = txtSoLuong.Text;

            string sql = $"UPDATE Kho SET MaSanPham = '{maSP}', SoLuongTon = {soLuong} WHERE ID = {id}";
            db.ExcuteNonQuery(sql);

            MessageBox.Show("Cập nhật kho thành công!");
            LoadKho();
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string id = dgvKho.CurrentRow.Cells["ID"].Value.ToString();

            if (MessageBox.Show("Bạn có muốn xóa bản ghi kho này không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = $"DELETE FROM Kho WHERE ID = {id}";
                db.ExcuteNonQuery(sql);
                LoadKho();
            }
        }

        private void dgvKho_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                cboMaSP.SelectedValue = dgvKho.Rows[i].Cells["MaSanPham"].Value.ToString();
                txtSoLuong.Text = dgvKho.Rows[i].Cells["SoLuongTon"].Value.ToString();
            }
        }
    }
}
