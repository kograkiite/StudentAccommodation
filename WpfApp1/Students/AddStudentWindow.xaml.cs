using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfApp1
{
    public partial class AddStudentWindow : Window
    {
        public SinhVien NewStudent { get; private set; }
        public ObservableCollection<string> Rooms { get; private set; }

        public AddStudentWindow()
        {
            InitializeComponent();
            LoadRooms();
        }

        private void LoadRooms()
        {
            Rooms = new ObservableCollection<string>();
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT SoPhong FROM Phong WHERE TrangThaiPhong = 1"; // Chỉ lấy những phòng có TrangThaiPhong = 1

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Rooms.Add(reader["SoPhong"].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            cbRoom.ItemsSource = Rooms;
        }

        private bool CheckRoomCapacity(string room)
        {
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT SucChua, SoLuongSinhVienHienTai FROM Phong WHERE SoPhong = @Room";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Room", room);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int capacity = (int)reader["SucChua"];
                        int currentNumberOfStudents = (int)reader["SoLuongSinhVienHienTai"];
                        reader.Close();

                        // So sánh với sức chứa
                        return currentNumberOfStudents < capacity;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }

        private void UpdateRoomInfo(string room)
        {
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "UPDATE Phong SET SoLuongSinhVienHienTai = SoLuongSinhVienHienTai + 1, TrangThaiPhong = CASE WHEN SoLuongSinhVienHienTai + 1 = SucChua THEN 0 ELSE 1 END WHERE SoPhong = @Room";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Room", room);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtFullname.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                (!rbMale.IsChecked.HasValue || !rbFemale.IsChecked.HasValue) ||
                !dpDateOfBirth.SelectedDate.HasValue || cbRoom.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            string roomSelected = cbRoom.SelectedItem.ToString();

            // Kiểm tra số lượng sinh viên đã đăng ký vào phòng đó
            if (!CheckRoomCapacity(roomSelected))
            {
                MessageBox.Show("Phòng đã chọn đã đầy. Vui lòng chọn phòng khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "INSERT INTO SinhVien (id, fullname, phoneNumber, sex, dateOfBirth, Room) VALUES (@id, @fullname, @phoneNumber, @sex, @dateOfBirth, @Room)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", txtId.Text);
                command.Parameters.AddWithValue("@fullname", txtFullname.Text);
                command.Parameters.AddWithValue("@phoneNumber", txtPhoneNumber.Text);
                command.Parameters.AddWithValue("@sex", rbMale.IsChecked == true ? "Nam" : "Nữ");
                command.Parameters.AddWithValue("@dateOfBirth", dpDateOfBirth.SelectedDate.Value);
                command.Parameters.AddWithValue("@Room", roomSelected);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Cập nhật thông tin phòng sau khi thêm sinh viên
                        UpdateRoomInfo(roomSelected);

                        MessageBox.Show("Thêm sinh viên thành công.");
                        NewStudent = new SinhVien
                        {
                            id = txtId.Text,
                            fullname = txtFullname.Text,
                            phoneNumber = txtPhoneNumber.Text,
                            sex = rbMale.IsChecked == true ? "Nam" : "Nữ",
                            dateOfBirth = dpDateOfBirth.SelectedDate.Value,
                            Room = roomSelected
                        };

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Thêm sinh viên không thành công.");
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
            DialogResult = false;
            Close();
        }
    }
}
