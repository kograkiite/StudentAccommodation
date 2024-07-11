using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement
{
    public partial class RoomManagement : Window
    {
        public ObservableCollection<Room> Rooms { get; set; }

        public RoomManagement()
        {
            InitializeComponent();
            Rooms = new ObservableCollection<Room>();
            BangRooms.ItemsSource = Rooms;

            LoadRoomsFromDatabase();
        }

        private void LoadRoomsFromDatabase()
        {
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT * FROM Phong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Rooms.Add(new Room()
                        {
                            SoPhong = row["SoPhong"].ToString(),
                            SucChua = Convert.ToInt32(row["SucChua"]),
                            GiaThue = Convert.ToDecimal(row["GiaThue"]),
                            SoLuongSinhVienHienTai = Convert.ToInt32(row["SoLuongSinhVienHienTai"]),
                            TrangThaiPhong = Convert.ToBoolean(row["TrangThaiPhong"]) ? "Còn chỗ" : "Hết chỗ"
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            var addRoomWindow = new AddRoomWindow();
            if (addRoomWindow.ShowDialog() == true)
            {
                Rooms.Add(addRoomWindow.NewRoom);
            }
        }

        private void btnUpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (BangRooms.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để cập nhật.");
                return;
            }

            var selectedRoom = (Room)BangRooms.SelectedItem;
            var updateRoomWindow = new UpdateRoomWindow(selectedRoom);
            if (updateRoomWindow.ShowDialog() == true)
            {
                var updatedRoom = updateRoomWindow.UpdatedRoom;
                var index = Rooms.IndexOf(selectedRoom);
                Rooms[index] = updatedRoom;
            }
        }

        private void btnDeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var selectedRooms = BangRooms.SelectedItems.Cast<Room>().ToList();
            if (selectedRooms.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phòng để xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa phòng đã chọn không?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
                string query = "DELETE FROM Phong WHERE SoPhong = @SoPhong";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var room in selectedRooms)
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@SoPhong", room.SoPhong);
                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Rooms.Remove(room);
                                MessageBox.Show($"Xóa phòng {room.SoPhong} thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Xóa phòng {room.SoPhong} không thành công.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void txtSearchRoom_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchRoom.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                BangRooms.ItemsSource = Rooms;
            }
            else
            {
                var filteredList = Rooms.Where(room =>
                    room.SoPhong.ToLower().Contains(searchText)
                ).ToList();

                BangRooms.ItemsSource = filteredList;
            }
        }
    }

    public class Room
    {
        public string SoPhong { get; set; }
        public int SucChua { get; set; }
        public decimal GiaThue { get; set; }
        public int SoLuongSinhVienHienTai { get; set; }
        public string TrangThaiPhong { get; set; }
    }
}
