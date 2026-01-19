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
    public partial class f_danhmuc : Form
    {
        public f_danhmuc()
        {
            InitializeComponent();
        }
        Data db = new Data();

        
        void LoadDanhMuc()
        {
            string sql = "SELECT * FROM DanhMuc";
            dataGridView1.DataSource = db.getTable(sql);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Gọi hàm này trong sự kiện Form_Load
       

        private void f_danhmuc_Load(object sender, EventArgs e)
        {
            LoadDanhMuc();
        }

        private void btthem_Click(object sender, EventArgs e)
        {
            string ma = txtMaDM.Text;
            string ten = txtTenDM.Text;
            string mota = txtMoTa.Text;

            if (string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên danh mục!");
                return;
            }

            string sql = $"INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES ( N'{ten}', N'{mota}')";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Thêm danh mục thành công!");
            LoadDanhMuc();
            ClearText();
        }

        private void btsua_Click(object sender, EventArgs e)
        {
            string ma = txtMaDM.Text;
            string ten = txtTenDM.Text;
            string mota = txtMoTa.Text;

            string sql = $"UPDATE DanhMuc SET TenDanhMuc = N'{ten}', MoTa = N'{mota}' WHERE DanhMucID = '{ma}'";

            db.ExcuteNonQuery(sql);
            MessageBox.Show("Cập nhật danh mục thành công!");
            LoadDanhMuc();
            ClearText();
        }

        private void btxoa_Click(object sender, EventArgs e)
        {
            string ma = txtMaDM.Text;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa danh mục này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = $"DELETE FROM DanhMuc WHERE DanhMucID = '{ma}'";
                db.ExcuteNonQuery(sql);
                MessageBox.Show("Xóa danh mục thành công!");
                LoadDanhMuc();
                ClearText();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                txtMaDM.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
                txtTenDM.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                txtMoTa.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            }
        }
            void ClearText()
            {
                txtMaDM.Clear();
                txtTenDM.Clear();
                txtMoTa.Clear();

                // Đưa con trỏ chuột về ô Mã danh mục để người dùng nhập tiếp
                txtMaDM.Focus();
            }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
