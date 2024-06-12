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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text) || txtSoPhong.Text == "Số Phòng" ||
                string.IsNullOrWhiteSpace(txtSucChua.Text) || txtSucChua.Text == "Sức Chứa" ||
                string.IsNullOrWhiteSpace(txtGiaThue.Text) || txtGiaThue.Text == "Giá Thuê")
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            NewRoom = new Room
            {
                SoPhong = txtSoPhong.Text,
                SucChua = int.Parse(txtSucChua.Text),
                GiaThue = decimal.Parse(txtGiaThue.Text)
            };

            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "INSERT INTO Phong (SoPhong, SucChua, GiaThue) VALUES (@SoPhong, @SucChua, @GiaThue)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SoPhong", NewRoom.SoPhong);
                command.Parameters.AddWithValue("@SucChua", NewRoom.SucChua);
                command.Parameters.AddWithValue("@GiaThue", NewRoom.GiaThue);

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
    }
}
