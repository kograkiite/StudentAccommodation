using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement
{
    public partial class UpdateRoomWindow : Window
    {
        public Room UpdatedRoom { get; private set; }

        public UpdateRoomWindow(Room room)
        {
            InitializeComponent();
            UpdatedRoom = room;
            DataContext = UpdatedRoom;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text) ||
                string.IsNullOrWhiteSpace(txtSucChua.Text) ||
                string.IsNullOrWhiteSpace(txtGiaThue.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "UPDATE Phong SET SoPhong = @SoPhong, SucChua = @SucChua, GiaThue = @GiaThue WHERE SoPhong = @SoPhong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SoPhong", UpdatedRoom.SoPhong);
                command.Parameters.AddWithValue("@SucChua", UpdatedRoom.SucChua);
                command.Parameters.AddWithValue("@GiaThue", UpdatedRoom.GiaThue);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật phòng thành công!");
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
