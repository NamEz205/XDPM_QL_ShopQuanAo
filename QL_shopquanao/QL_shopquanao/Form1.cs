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
    public partial class f_main : Form
    {
        public f_main()
        {
            InitializeComponent();
        }

        private void quảnLýToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Form frm = new Form();
            switch (e.ClickedItem.Name)
            {
                case "m_kh":
                    frm = new f_khachhang();
                    break;
                case "m_nv":
                    frm = new f_nhanvien();
                    break;
                case "m_hd":
                    frm = new f_hoadon();
                    break;

                case "m_sp":
                    frm = new f_sanpham();
                    break;
                case "m_kho":
                    frm = new f_kho();
                    break;
                case "m_dm":
                    frm = new f_danhmuc();
                    break;


            }
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
            frm.BringToFront();

        }
    }
}
