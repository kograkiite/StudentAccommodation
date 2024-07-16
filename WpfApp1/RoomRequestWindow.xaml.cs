using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace StudentManagement
{
    public partial class RoomRequestWindow : Window
    {
        public event Action RoomRequestsUpdated;
        private ObservableCollection<RoomRequest> roomRequests;
        private User currentUser;
        private string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

        public RoomRequestWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            roomRequests = new ObservableCollection<RoomRequest>();
            LoadRoomRequests();

            // Hiển thị nút Cancel Request nếu user là student
            if (currentUser.Role == "student")
            {
                btnCancelRequest.Visibility = Visibility.Visible;
                btnAcceptRequest.Visibility = Visibility.Collapsed;
                btnDeclineRequest.Visibility = Visibility.Collapsed;
            }
            // Hiển thị nút Accept và Decline nếu user là admin
            else if (currentUser.Role == "admin")
            {
                btnCancelRequest.Visibility = Visibility.Collapsed;
                btnAcceptRequest.Visibility = Visibility.Visible;
                btnDeclineRequest.Visibility = Visibility.Visible;
            }
        }

        private void LoadRoomRequests()
        {
            string query = "";
            if (currentUser.Role == "student")
            {
                // Truy vấn các yêu cầu của sinh viên hiện tại
                query = "SELECT RequestID, MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayYeuCau, TrangThaiYeuCau " +
                        "FROM RoomRequests " +
                        "WHERE MaSinhVien = @MaSinhVien";
            }
            else if (currentUser.Role == "admin")
            {
                // Truy vấn tất cả yêu cầu nếu user là admin
                query = "SELECT RequestID, MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayYeuCau, TrangThaiYeuCau " +
                        "FROM RoomRequests";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", currentUser.SinhVienID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        RoomRequest request = new RoomRequest
                        {
                            RequestID = Convert.ToInt32(reader["RequestID"]),
                            MaSinhVien = reader["MaSinhVien"].ToString(),
                            TenSinhVien = reader["TenSinhVien"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            SoPhong = reader["SoPhong"].ToString(),
                            NgayYeuCau = Convert.ToDateTime(reader["NgayYeuCau"]),
                            TrangThaiYeuCau = reader["TrangThaiYeuCau"].ToString()
                        };
                        roomRequests.Add(request);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            dgRoomRequests.ItemsSource = roomRequests;
        }

        private bool CheckRoomCapacity(string room)
        {
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

        private void UpdateSinhVienRoom(string maSinhVien, string soPhong)
        {
            string updateQuery = "UPDATE SinhVien SET SoPhong = @SoPhong WHERE id = @MaSinhVien";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@SoPhong", soPhong);
                command.Parameters.AddWithValue("@MaSinhVien", maSinhVien);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã chấp nhận yêu cầu và cập nhật thông tin phòng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnAcceptRequest_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoomRequests.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn chấp nhận yêu cầu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    RoomRequest selectedRequest = dgRoomRequests.SelectedItem as RoomRequest;

                    // Kiểm tra sức chứa phòng trước khi chấp nhận yêu cầu
                    if (!CheckRoomCapacity(selectedRequest.SoPhong))
                    {
                        MessageBox.Show("Phòng đã chọn đã đầy. Vui lòng chọn phòng khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    UpdateRoomInfo(selectedRequest.SoPhong);
                    // Cập nhật thông tin phòng cho sinh viên
                    UpdateSinhVienRoom(selectedRequest.MaSinhVien, selectedRequest.SoPhong);

                    // Xóa yêu cầu phòng khỏi cơ sở dữ liệu
                    DeleteRoomRequest(selectedRequest.RequestID);
                    roomRequests.Remove(selectedRequest);

                    // Trigger event để cập nhật lại DataGrid trong MainWindow
                    RoomRequestsUpdated?.Invoke();
                }
            }
        }

        private void btnDeclineRequest_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoomRequests.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn từ chối yêu cầu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    RoomRequest selectedRequest = dgRoomRequests.SelectedItem as RoomRequest;

                    // Xóa yêu cầu phòng khỏi cơ sở dữ liệu
                    DeleteRoomRequest(selectedRequest.RequestID);
                    roomRequests.Remove(selectedRequest);

                    // Trigger event để cập nhật lại DataGrid trong MainWindow
                    RoomRequestsUpdated?.Invoke();
                }
            }
        }

        private void UpdateRoomInfo(string room)
        {
            string updateQuery = "UPDATE Phong SET SoLuongSinhVienHienTai = SoLuongSinhVienHienTai + 1, TrangThaiPhong = CASE WHEN SoLuongSinhVienHienTai + 1 = SucChua THEN 0 ELSE 1 END WHERE SoPhong = @Room";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);
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


        private void btnCancelRequest_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoomRequests.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn hủy yêu cầu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    RoomRequest selectedRequest = dgRoomRequests.SelectedItem as RoomRequest;
                    DeleteRoomRequest(selectedRequest.RequestID);
                    roomRequests.Remove(selectedRequest);

                    // Trigger event để cập nhật lại DataGrid trong MainWindow
                    RoomRequestsUpdated?.Invoke();
                }
            }
        }

        private void DeleteRoomRequest(int requestID)
        {
            string query = "DELETE FROM RoomRequests WHERE RequestID = @RequestID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RequestID", requestID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
