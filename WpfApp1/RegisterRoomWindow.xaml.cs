using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace StudentManagement
{
    public partial class RegisterRoomWindow : Window
    {
        private User currentUser;
        public ObservableCollection<string> Rooms { get; private set; }

        public RegisterRoomWindow(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadRooms();
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

        private bool HasExistingRoomRequest(string maSinhVien)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT COUNT(*) FROM RoomRequests WHERE MaSinhVien = @MaSinhVien";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", maSinhVien);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi kiểm tra RoomRequests: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void InsertRoomRequest(string maSinhVien, string tenSinhVien, string soDienThoai, string soPhong)
        {
            // Kiểm tra xem maSinhVien có tồn tại trong bảng SinhVien không
            if (!IsMaSinhVienValid(maSinhVien))
            {
                MessageBox.Show("Không thể thêm yêu cầu đăng ký phòng. MaSinhVien không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra xem sinh viên đã có yêu cầu nào chưa
            if (HasExistingRoomRequest(maSinhVien))
            {
                MessageBox.Show("Sinh viên đã có yêu cầu đăng ký phòng. Chỉ được phép tạo một yêu cầu duy nhất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string insertQuery = @"INSERT INTO RoomRequests (MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayYeuCau, TrangThaiYeuCau) 
                           VALUES (@MaSinhVien, @TenSinhVien, @SoDienThoai, @SoPhong, @NgayYeuCau, @TrangThaiYeuCau)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@MaSinhVien", maSinhVien);
                command.Parameters.AddWithValue("@TenSinhVien", tenSinhVien);
                command.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                command.Parameters.AddWithValue("@SoPhong", soPhong);
                command.Parameters.AddWithValue("@NgayYeuCau", DateTime.Now); // Ngày yêu cầu là ngày hiện tại
                command.Parameters.AddWithValue("@TrangThaiYeuCau", "Pending"); // Trạng thái yêu cầu mặc định là Pending

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Yêu cầu đăng ký phòng đã được gửi thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể thêm yêu cầu đăng ký phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thực hiện thao tác: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsMaSinhVienValid(string maSinhVien)
        {
            // Kiểm tra xem maSinhVien có tồn tại trong bảng SinhVien không
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT COUNT(*) FROM SinhVien WHERE id = @MaSinhVien";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", maSinhVien);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi kiểm tra MaSinhVien: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string maSinhVien = currentUser.SinhVienID; // Lấy SinhVienID từ currentUser
            string tenSinhVien = currentUser.Fullname; // Lấy Tên sinh viên từ currentUser
            string soDienThoai = currentUser.PhoneNumber; // Lấy Số điện thoại từ currentUser
            string soPhong = cbRoom.SelectedItem as string; // Lấy Số phòng từ combobox

            // Gọi phương thức để thêm yêu cầu đăng ký phòng vào CSDL
            InsertRoomRequest(maSinhVien, tenSinhVien, soDienThoai, soPhong);
        }
    }
}
