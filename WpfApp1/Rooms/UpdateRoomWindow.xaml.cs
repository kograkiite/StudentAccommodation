using Microsoft.Data.SqlClient;
using System.Windows;

namespace StudentManagement
{
    public partial class UpdateRoomWindow : Window
    {
        public Room UpdatedRoom { get; private set; }

        private Room originalRoom;

        public UpdateRoomWindow(Room room)
        {
            InitializeComponent();
            originalRoom = room;

            // Load room information into text boxes
            txtSoPhong.Text = room.SoPhong;
            txtSucChua.Text = room.SucChua.ToString();
            txtGiaThue.Text = room.GiaThue.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text) ||
                string.IsNullOrWhiteSpace(txtSucChua.Text) ||
                string.IsNullOrWhiteSpace(txtGiaThue.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string soPhong = txtSoPhong.Text.Trim();
            int sucChua, soLuongSinhVienHienTai;
            decimal giaThue;

            // Validate integer fields
            if (!int.TryParse(txtSucChua.Text.Trim(), out sucChua) || sucChua <= 0)
            {
                MessageBox.Show("Sức chứa phải là số nguyên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate decimal field
            if (!decimal.TryParse(txtGiaThue.Text.Trim(), out giaThue) || giaThue <= 0)
            {
                MessageBox.Show("Giá thuê phải là số thực.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Update room information
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "UPDATE Phong SET SoPhong = @SoPhong, SucChua = @SucChua, GiaThue = @GiaThue WHERE SoPhong = @OriginalSoPhong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SoPhong", soPhong);
                command.Parameters.AddWithValue("@SucChua", sucChua);
                command.Parameters.AddWithValue("@GiaThue", giaThue);
                command.Parameters.AddWithValue("@OriginalSoPhong", originalRoom.SoPhong);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin phòng thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        UpdatedRoom = new Room
                        {
                            SoPhong = soPhong,
                            SucChua = sucChua,
                            GiaThue = giaThue,
                            SoLuongSinhVienHienTai = originalRoom.SoLuongSinhVienHienTai,
                            TrangThaiPhong = originalRoom.TrangThaiPhong
                        };
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thông tin phòng không thành công.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
