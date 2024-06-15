using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentManagement
{
    public partial class AddElectricBillWindow : Window
    {
        public AddElectricBillWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMaTienDien.Text) &&
                int.TryParse(txtMaHopDong.Text, out int maHopDong) &&
                int.TryParse(txtSoDien.Text, out int soDien) &&
                DateTime.TryParse(dpNgayTinhTien.Text, out DateTime ngayTinhTien) &&
                decimal.TryParse(txtThanhTien.Text, out decimal thanhTien))
            {
                string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
                string query = "INSERT INTO [StudentManagement].[dbo].[TienDien] (MaTienDien, MaHopDong, SoDien, NgayTinhTien, ThanhTien) VALUES (@MaTienDien, @MaHopDong, @SoDien, @NgayTinhTien, @ThanhTien)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaTienDien", txtMaTienDien.Text);
                    command.Parameters.AddWithValue("@MaHopDong", maHopDong);
                    command.Parameters.AddWithValue("@SoDien", soDien);
                    command.Parameters.AddWithValue("@NgayTinhTien", ngayTinhTien);
                    command.Parameters.AddWithValue("@ThanhTien", thanhTien);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Hóa đơn đã được thêm thành công!");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ và chính xác thông tin.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
