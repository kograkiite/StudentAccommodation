using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement
{
    public partial class AddRoomWindow : Window
    {
        public Room NewRoom { get; private set; }

        public AddRoomWindow()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "Số Phòng" || textBox.Text == "Sức Chứa" || textBox.Text == "Giá Thuê")
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "txtSoPhong")
                {
                    textBox.Text = "Số Phòng";
                }
                else if (textBox.Name == "txtSucChua")
                {
                    textBox.Text = "Sức Chứa";
                }
                else if (textBox.Name == "txtGiaThue")
                {
                    textBox.Text = "Giá Thuê";
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text) || txtSoPhong.Text == "Số Phòng" ||
                string.IsNullOrWhiteSpace(txtSucChua.Text) || txtSucChua.Text == "Sức Chứa" ||
                string.IsNullOrWhiteSpace(txtGiaThue.Text) || txtGiaThue.Text == "Giá Thuê")
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            // Validate capacity (Sức Chứa) and rent (Giá Thuê)
            if (!int.TryParse(txtSucChua.Text.Trim(), out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Sức chứa phòng phải là một số nguyên dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtGiaThue.Text.Trim(), out decimal rent) || rent <= 0)
            {
                MessageBox.Show("Giá thuê phòng phải là một số dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NewRoom = new Room
            {
                SoPhong = txtSoPhong.Text,
                SucChua = int.Parse(txtSucChua.Text),
                GiaThue = decimal.Parse(txtGiaThue.Text),
                SoLuongSinhVienHienTai = 0,
                TrangThaiPhong = "Còn chỗ"
            };

            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "INSERT INTO Phong (SoPhong, SucChua, GiaThue, SoLuongSinhVienHienTai, TrangThaiPhong) VALUES (@SoPhong, @SucChua, @GiaThue, @SoLuongSinhVienHienTai, @TrangThaiPhong)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SoPhong", NewRoom.SoPhong);
                command.Parameters.AddWithValue("@SucChua", NewRoom.SucChua);
                command.Parameters.AddWithValue("@GiaThue", NewRoom.GiaThue);
                command.Parameters.AddWithValue("@SoLuongSinhVienHienTai", 0);
                command.Parameters.AddWithValue("@TrangThaiPhong", 1);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Thêm phòng thành công!");
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
