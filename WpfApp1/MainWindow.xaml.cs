﻿using Microsoft.Data.SqlClient;
using StudentManagement;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<SinhVien> SinhVien { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SinhVien = new ObservableCollection<SinhVien>();
            BangSinhVien.ItemsSource = SinhVien;

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT * FROM SinhVien";

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
                        SinhVien.Add(new SinhVien()
                        {
                            id = row["id"].ToString(),
                            fullname = row["fullname"].ToString(),
                            phoneNumber = row["phoneNumber"].ToString(),
                            sex = row["sex"].ToString(),
                            dateOfBirth = Convert.ToDateTime(row["dateOfBirth"]),
                            Room = row["Room"].ToString() 
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
                SinhVien.Add(addStudentWindow.NewStudent);
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
                var index = SinhVien.IndexOf(selectedStudent);
                SinhVien[index] = updatedStudent;
            }
        }

        private void UpdateRoomInfoAfterDelete(string room)
        {
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string updateRoomQuery = "UPDATE Phong SET SoLuongSinhVienHienTai = SoLuongSinhVienHienTai - 1, TrangThaiPhong = CASE WHEN SoLuongSinhVienHienTai - 1 < SucChua THEN 1 ELSE 0 END WHERE SoPhong = @Room";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateRoomQuery, connection);
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
                string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
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
                                    SinhVien.Remove(student);
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
