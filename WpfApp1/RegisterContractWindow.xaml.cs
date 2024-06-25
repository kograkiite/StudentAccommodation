using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace StudentManagement
{
    public partial class RegisterContractWindow : Window
    {
        public Contract NewContract { get; set; }

        public RegisterContractWindow()
        {
            InitializeComponent();
        }

        private int GetNewContractId()
        {
            // Kết nối cơ sở dữ liệu
            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT IDENT_CURRENT('HopDong')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return 0; // Trả về 0 nếu không thể lấy được mã hợp đồng mới
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Validate that the start date is not greater than the end date
            if (dpNgayBatDau.SelectedDate > dpNgayKetThuc.SelectedDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }

            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

            string query = "INSERT INTO HopDong (MaSinhVien, SoPhong, NgayBatDau, NgayKetThuc) VALUES (@MaSinhVien, @SoPhong, @NgayBatDau, @NgayKetThuc)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", txtMaSV.Text);
                command.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);
                command.Parameters.AddWithValue("@NgayBatDau", dpNgayBatDau.SelectedDate);
                command.Parameters.AddWithValue("@NgayKetThuc", dpNgayKetThuc.SelectedDate);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đăng ký hợp đồng thành công!");

                        // Tạo một đối tượng Contract mới từ dữ liệu đã nhập và mã hợp đồng mới từ cơ sở dữ liệu
                        NewContract = new Contract()
                        {
                            MaHopDong = GetNewContractId(), // Lấy mã hợp đồng mới từ cơ sở dữ liệu
                            MaSinhVien = txtMaSV.Text,
                            SoPhong = txtSoPhong.Text,
                            NgayBatDau = dpNgayBatDau.SelectedDate.GetValueOrDefault(),
                            NgayKetThuc = dpNgayKetThuc.SelectedDate.GetValueOrDefault()
                        };
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Đăng ký hợp đồng không thành công!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
