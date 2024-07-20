using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfApp1
{
    public partial class UpdateStudentWindow : Window
    {
        public SinhVien UpdatedStudent { get; private set; }
        public ObservableCollection<string> Rooms { get; private set; }

        public UpdateStudentWindow(SinhVien student)
        {
            InitializeComponent();
            LoadRooms();

            // Điền thông tin sinh viên vào các trường
            txtId.Text = student.id;
            txtFullname.Text = student.fullname;
            txtPhoneNumber.Text = student.phoneNumber;
            rbMale.IsChecked = student.sex == "Nam";
            rbFemale.IsChecked = student.sex == "Nữ";
            dpDateOfBirth.SelectedDate = student.dateOfBirth;

            // Load thông tin phòng của sinh viên vào ComboBox
            cbRoom.SelectedItem = student.Room;

            // Không cho phép chỉnh sửa ID
            txtId.IsEnabled = false;
        }

        private void LoadRooms()
        {
            Rooms = new ObservableCollection<string>();
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(txtFullname.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                (!rbMale.IsChecked.HasValue && !rbFemale.IsChecked.HasValue) ||
                !dpDateOfBirth.SelectedDate.HasValue ||
                cbRoom.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string studentId = txtId.Text.Trim();
            string studentName = txtFullname.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();

            // Kiểm tra định dạng Mã Sinh Viên
            if (!System.Text.RegularExpressions.Regex.IsMatch(studentId, @"^SE[0-9]{6}$"))
            {
                MessageBox.Show("Mã sinh viên phải theo định dạng SEXXXXXX với X là số nguyên dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra tên không chứa số
            if (System.Text.RegularExpressions.Regex.IsMatch(studentName, @"\d"))
            {
                MessageBox.Show("Tên sinh viên không được chứa số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra số điện thoại chỉ chứa các số nguyên dương
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[0-9]+$"))
            {
                MessageBox.Show("Số điện thoại chỉ được chứa các số nguyên dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra hợp đồng của sinh viên
            if (HasValidContract(studentId))
            {
                MessageBox.Show("Sinh viên đang có hợp đồng còn hạn. Không thể cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Retrieve current room of the student
            string currentRoom = GetCurrentRoom(txtId.Text);

            // Check if room update is necessary
            if (cbRoom.SelectedItem.ToString() != currentRoom)
            {
                // Update current room's information (subtract the student)
                UpdateRoomInfo(currentRoom, -1);

                // Update new room's information (add the student)
                UpdateRoomInfo(cbRoom.SelectedItem.ToString(), 1);
            }

            // Proceed with updating student information
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "UPDATE SinhVien SET fullname = @fullname, phoneNumber = @phoneNumber, sex = @sex, dateOfBirth = @dateOfBirth, SoPhong = @room WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", txtId.Text);
                command.Parameters.AddWithValue("@fullname", txtFullname.Text);
                command.Parameters.AddWithValue("@phoneNumber", txtPhoneNumber.Text);
                command.Parameters.AddWithValue("@sex", rbMale.IsChecked == true ? "Nam" : "Nữ");
                command.Parameters.AddWithValue("@dateOfBirth", dpDateOfBirth.SelectedDate.HasValue ? dpDateOfBirth.SelectedDate.Value : DateTime.MinValue);
                command.Parameters.AddWithValue("@room", cbRoom.SelectedItem.ToString());

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật sinh viên thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        UpdatedStudent = new SinhVien
                        {
                            id = txtId.Text,
                            fullname = txtFullname.Text,
                            phoneNumber = txtPhoneNumber.Text,
                            sex = rbMale.IsChecked == true ? "Nam" : "Nữ",
                            dateOfBirth = dpDateOfBirth.SelectedDate.HasValue ? dpDateOfBirth.SelectedDate.Value : DateTime.MinValue,
                            Room = cbRoom.SelectedItem.ToString()
                        };
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật sinh viên không thành công.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool HasValidContract(string studentId)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT COUNT(*) FROM HopDong WHERE MaSinhVien = @MaSinhVien AND TrangThai = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", studentId);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi kiểm tra hợp đồng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }


        // Method to retrieve current room of the student
        private string GetCurrentRoom(string studentId)
        {
            string room = string.Empty;
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT SoPhong FROM SinhVien WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", studentId);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        room = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return room;
        }

        // Method to update room information (SoLuongSinhVienHienTai and TrangThaiPhong)
        private void UpdateRoomInfo(string room, int studentChange)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "UPDATE Phong SET SoLuongSinhVienHienTai = SoLuongSinhVienHienTai + @studentChange, TrangThaiPhong = CASE WHEN SoLuongSinhVienHienTai + @studentChange = SucChua THEN 0 ELSE 1 END WHERE SoPhong = @Room";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentChange", studentChange);
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
