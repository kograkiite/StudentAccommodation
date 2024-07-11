﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1;

namespace StudentManagement
{
    public partial class RegisterContractWindow : Window
    {
        public Contract NewContract { get; set; }
        private ObservableCollection<SinhVien> students;
        private SinhVien selectedStudent;

        public RegisterContractWindow()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void LoadStudents()
        {
            students = new ObservableCollection<SinhVien>();
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT id AS MaSinhVien, fullname AS TenSinhVien, phoneNumber AS SoDienThoai, Room AS SoPhong FROM SinhVien";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        students.Add(new SinhVien
                        {
                            id = reader["MaSinhVien"].ToString(),
                            fullname = reader["TenSinhVien"].ToString(),
                            phoneNumber = reader["SoDienThoai"].ToString(),
                            Room = reader["SoPhong"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void txtMaSV_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear existing student info when text changes
            ClearStudentInfo();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string studentId = txtMaSV.Text.Trim();

            if (!string.IsNullOrEmpty(studentId))
            {
                SinhVien foundStudent = students.FirstOrDefault(s => s.id == studentId);
                if (foundStudent != null)
                {
                    selectedStudent = foundStudent;
                    DisplayStudentInfo(selectedStudent);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên có mã số " + studentId);
                    ClearStudentInfo();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập Mã Sinh Viên để tìm kiếm.");
            }
        }

        private void DisplayStudentInfo(SinhVien student)
        {
            txtTenSV.Text = student.fullname;
            txtSoDienThoai.Text = student.phoneNumber;
            txtSoPhong.Text = student.Room;
        }

        private void ClearStudentInfo()
        {
            selectedStudent = null;
            txtTenSV.Text = "";
            txtSoDienThoai.Text = "";
            txtSoPhong.Text = "";
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Validate that the start date is not greater than the end date
            if (dpNgayBatDau.SelectedDate > dpNgayKetThuc.SelectedDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }

            if (selectedStudent == null)
            {
                MessageBox.Show("Vui lòng nhập và tìm kiếm Mã Sinh Viên để chọn sinh viên!");
                return;
            }

            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

            // Check if the selected room already has a contract
            string checkRoomQuery = "SELECT MaHopDong FROM HopDong WHERE SoPhong = @SoPhong";
            int? existingContractId = null;

            // First SqlConnection for checking existing contract
            using (SqlConnection checkRoomConnection = new SqlConnection(connectionString))
            {
                SqlCommand checkRoomCommand = new SqlCommand(checkRoomQuery, checkRoomConnection);
                checkRoomCommand.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);

                try
                {
                    checkRoomConnection.Open();
                    object result = checkRoomCommand.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        existingContractId = Convert.ToInt32(result);
                        // Debugging statement
                        Console.WriteLine($"Existing Contract ID found: {existingContractId}");
                    }
                    else
                    {
                        existingContractId = null; // Set explicitly to null if no contract exists
                                                   // Debugging statement
                        Console.WriteLine("No existing contract found for the room.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error checking existing contract: " + ex.Message);
                    return; // Exit method on error
                }
            }

            // If an existing contract is found, display message and exit
            if (existingContractId.HasValue)
            {
                MessageBox.Show("Phòng này đã có sinh viên đăng kí hợp đồng. Không thể đăng kí thêm hợp đồng cho phòng này.");
                return;
            }

            // Query to insert contract and retrieve student info
            string insertContractQuery = "INSERT INTO HopDong (MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayBatDau, NgayKetThuc, TrangThai) " +
                                         "OUTPUT INSERTED.MaHopDong " +
                                         "VALUES (@MaSinhVien, @TenSinhVien, @SoDienThoai, @SoPhong, @NgayBatDau, @NgayKetThuc, 1); " +
                                         "SELECT fullname AS TenSinhVien, phoneNumber AS SoDienThoai FROM SinhVien WHERE id = @MaSinhVien";

            // Query to insert payment record into ThuTien
            string insertPaymentQuery = "INSERT INTO ThuTien (MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, GiaThue) " +
                                        "VALUES (@MaSinhVien, @TenSinhVien, @SoDienThoai, @SoPhong, @GiaThue)";

            // Second SqlConnection for inserting new contract and payment
            using (SqlConnection insertConnection = new SqlConnection(connectionString))
            {
                SqlCommand insertContractCommand = new SqlCommand(insertContractQuery, insertConnection);
                insertContractCommand.Parameters.AddWithValue("@MaSinhVien", selectedStudent.id);
                insertContractCommand.Parameters.AddWithValue("@TenSinhVien", selectedStudent.fullname);
                insertContractCommand.Parameters.AddWithValue("@SoDienThoai", selectedStudent.phoneNumber);
                insertContractCommand.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);
                insertContractCommand.Parameters.AddWithValue("@NgayBatDau", dpNgayBatDau.SelectedDate);
                insertContractCommand.Parameters.AddWithValue("@NgayKetThuc", dpNgayKetThuc.SelectedDate);

                try
                {
                    insertConnection.Open();
                    // Execute the query for inserting contract
                    insertContractCommand.ExecuteNonQuery();

                    // Update NewContract with retrieved student info
                    NewContract = new Contract()
                    {
                        MaHopDong = GetNewContractId(), // Get new contract ID from database
                        MaSinhVien = selectedStudent.id,
                        TenSinhVien = selectedStudent.fullname,
                        SoDienThoai = selectedStudent.phoneNumber,
                        SoPhong = txtSoPhong.Text,
                        NgayBatDau = dpNgayBatDau.SelectedDate.GetValueOrDefault(),
                        NgayKetThuc = dpNgayKetThuc.SelectedDate.GetValueOrDefault()
                    };

                    // Insert payment record into ThuTien
                    SqlCommand insertPaymentCommand = new SqlCommand(insertPaymentQuery, insertConnection);
                    insertPaymentCommand.Parameters.AddWithValue("@MaSinhVien", selectedStudent.id);
                    insertPaymentCommand.Parameters.AddWithValue("@TenSinhVien", selectedStudent.fullname);
                    insertPaymentCommand.Parameters.AddWithValue("@SoDienThoai", selectedStudent.phoneNumber);
                    insertPaymentCommand.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);
                    insertPaymentCommand.Parameters.AddWithValue("@GiaThue", GetRoomRent(txtSoPhong.Text));

                    insertPaymentCommand.ExecuteNonQuery();

                    // Display success message
                    MessageBox.Show("Đăng ký hợp đồng thành công!");

                    // Reload students after successful registration
                    LoadStudents();

                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private decimal GetRoomRent(string soPhong)
        {
            // Kết nối cơ sở dữ liệu
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT GiaThue FROM Phong WHERE SoPhong = @SoPhong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SoPhong", soPhong);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        return Convert.ToDecimal(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return 0; // Return 0 if unable to retrieve room rent
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private int GetNewContractId()
        {
            // Kết nối cơ sở dữ liệu
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT IDENT_CURRENT('HopDong')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return 0; // Trả về 0 nếu không thể lấy được mã hợp đồng mới
        }
    }
}
