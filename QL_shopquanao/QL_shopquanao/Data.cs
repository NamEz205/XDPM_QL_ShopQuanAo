using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_shopquanao
{
    public class Data
    {
        private string nguon = @"Data Source=.;Initial Catalog=QL_CHQuanAo;Integrated Security=True;TrustServerCertificate=True";

        public DataTable getTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(nguon))
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
            return dt;
        }

        public void ExcuteNonQuery(string sql)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(nguon))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();                 
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi thực thi: " + ex.Message);
            }
        }
    }
}