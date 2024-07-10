using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace StudentManagement
{
    public partial class EditContractWindow : Window
    {
        public Contract UpdatedContract { get; private set; }

        public EditContractWindow(Contract contract)
        {
            InitializeComponent();
            UpdatedContract = contract;
            DataContext = UpdatedContract;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra ngày bắt đầu không lớn hơn ngày kết thúc
            if (UpdatedContract.NgayBatDau > UpdatedContract.NgayKetThuc)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }

            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

            string query = "UPDATE HopDong SET MaSinhVien = @MaSinhVien, SoPhong = @SoPhong, NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc WHERE MaHopDong = @MaHopDong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", UpdatedContract.MaSinhVien);
                command.Parameters.AddWithValue("@SoPhong", UpdatedContract.SoPhong);
                command.Parameters.AddWithValue("@NgayBatDau", UpdatedContract.NgayBatDau);
                command.Parameters.AddWithValue("@NgayKetThuc", UpdatedContract.NgayKetThuc);
                command.Parameters.AddWithValue("@MaHopDong", UpdatedContract.MaHopDong);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sửa hợp đồng thành công!");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Sửa hợp đồng không thành công!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

    }
}
