using Microsoft.Data.SqlClient;
using StudentManagement.Payment;
using StudentManagement;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<SinhVien> SinhVienList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SinhVienList = new ObservableCollection<SinhVien>();
            BangSinhVien.ItemsSource = SinhVienList;

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT id, fullname, phoneNumber, sex, dateOfBirth, SoPhong FROM SinhVien";

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
                        SinhVienList.Add(new SinhVien()
                        {
                            id = row["id"].ToString(),
                            fullname = row["fullname"].ToString(),
                            phoneNumber = row["phoneNumber"].ToString(),
                            sex = row["sex"].ToString(),
                            dateOfBirth = Convert.ToDateTime(row["dateOfBirth"]),
                            Room = row["SoPhong"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addStudentWindow = new AddStudentWindow();
            if (addStudentWindow.ShowDialog() == true)
            {
                SinhVienList.Add(addStudentWindow.NewStudent);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (BangSinhVien.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên để cập nhật.");
                return;
            }

            var selectedStudent = (SinhVien)BangSinhVien.SelectedItem;
            var updateStudentWindow = new UpdateStudentWindow(selectedStudent);
            if (updateStudentWindow.ShowDialog() == true)
            {
                var updatedStudent = updateStudentWindow.UpdatedStudent;
                var index = SinhVienList.IndexOf(selectedStudent);
                SinhVienList[index] = updatedStudent;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudents = BangSinhVien.SelectedItems;
            if (selectedStudents.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên đã chọn không?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
                string deleteQuery = "DELETE FROM SinhVien WHERE id = @id";
                string updateRoomQuery = "UPDATE Phong SET SoLuongSinhVienHienTai = SoLuongSinhVienHienTai - 1, TrangThaiPhong = CASE WHEN SoLuongSinhVienHienTai - 1 < SucChua THEN 1 ELSE 0 END WHERE SoPhong = @Room";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    for (int i = selectedStudents.Count - 1; i >= 0; i--)
                    {
                        var student = (SinhVien)selectedStudents[i];

                        // Xóa sinh viên
                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@id", student.id);
                            try
                            {
                                int rowsAffected = deleteCommand.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    SinhVienList.Remove(student);
                                    MessageBox.Show($"Xóa sinh viên {student.fullname} thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Cập nhật thông tin phòng sau khi xóa sinh viên
                                    using (SqlCommand updateRoomCommand = new SqlCommand(updateRoomQuery, connection))
                                    {
                                        updateRoomCommand.Parameters.AddWithValue("@Room", student.Room);
                                        try
                                        {
                                            updateRoomCommand.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Xóa sinh viên {student.fullname} không thành công.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                return; // Dừng lại nếu gặp lỗi
                            }
                        }
                    }
                }
            }
        }

        private void btnManageRoom_Click(object sender, RoutedEventArgs e)
        {
            var roomManagementWindow = new RoomManagement();
            roomManagementWindow.ShowDialog();
        }

        private void OpenContractWindow_Click(object sender, RoutedEventArgs e)
        {
            ContractWindow contractWindow = new ContractWindow();
            contractWindow.Show();
        }

        private void btnManagePayments_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow paymentWindow = new PaymentWindow();
            paymentWindow.ShowDialog();
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Show the Login window
            Login loginWindow = new Login();
            loginWindow.Show();

            // Close the MainWindow
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                BangSinhVien.ItemsSource = SinhVienList;
            }
            else
            {
                var filteredList = SinhVienList.Where(sv =>
                    sv.id.ToLower().Contains(searchText) ||
                    sv.fullname.ToLower().Contains(searchText) ||
                    sv.phoneNumber.ToLower().Contains(searchText)
                ).ToList();

                BangSinhVien.ItemsSource = filteredList;
            }
        }
    }

    public class SinhVien
    {
        public string id { get; set; }
        public string fullname { get; set; }
        public string phoneNumber { get; set; }
        public string sex { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string Room { get; set; } // Thêm thuộc tính Room
    }
}
