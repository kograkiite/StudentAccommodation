using Microsoft.Data.SqlClient;
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

            // No longer needed as cmbMaSV is replaced by txtMaSV
            // cmbMaSV.ItemsSource = students;
            // cmbMaSV.DisplayMemberPath = "id"; // Display MaSinhVien by default
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand checkRoomCommand = new SqlCommand(checkRoomQuery, connection);
                checkRoomCommand.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);

                try
                {
                    connection.Open();
                    object result = checkRoomCommand.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        existingContractId = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error checking existing contract: " + ex.Message);
                }
            }

            if (existingContractId.HasValue)
            {
                MessageBox.Show("Phòng này đã có sinh viên đăng kí hợp đồng. Không thể đăng kí thêm hợp đồng cho phòng này.");
                return;
            }

            // Query to insert contract and retrieve student info
            string insertContractQuery = "INSERT INTO HopDong (MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayBatDau, NgayKetThuc) " +
                                         "VALUES (@MaSinhVien, @TenSinhVien, @SoDienThoai, @SoPhong, @NgayBatDau, @NgayKetThuc); " +
                                         "SELECT fullname AS TenSinhVien, phoneNumber AS SoDienThoai FROM SinhVien WHERE id = @MaSinhVien";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand insertContractCommand = new SqlCommand(insertContractQuery, connection);
                insertContractCommand.Parameters.AddWithValue("@MaSinhVien", selectedStudent.id);
                insertContractCommand.Parameters.AddWithValue("@TenSinhVien", selectedStudent.fullname);
                insertContractCommand.Parameters.AddWithValue("@SoDienThoai", selectedStudent.phoneNumber);
                insertContractCommand.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);
                insertContractCommand.Parameters.AddWithValue("@NgayBatDau", dpNgayBatDau.SelectedDate);
                insertContractCommand.Parameters.AddWithValue("@NgayKetThuc", dpNgayKetThuc.SelectedDate);

                try
                {
                    connection.Open();
                    // Execute the query
                    SqlDataReader reader = insertContractCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        // Update NewContract with retrieved student info
                        NewContract = new Contract()
                        {
                            MaHopDong = GetNewContractId(), // Get new contract ID from database
                            MaSinhVien = selectedStudent.id,
                            TenSinhVien = reader["TenSinhVien"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            SoPhong = txtSoPhong.Text,
                            NgayBatDau = dpNgayBatDau.SelectedDate.GetValueOrDefault(),
                            NgayKetThuc = dpNgayKetThuc.SelectedDate.GetValueOrDefault()
                        };

                        // Update MaHopDong of the room
                        int newContractId = NewContract.MaHopDong;
                        string updateRoomQuery = "UPDATE Phong SET MaHopDong = @MaHopDong WHERE SoPhong = @SoPhong";
                        SqlCommand updateRoomCommand = new SqlCommand(updateRoomQuery, connection);
                        updateRoomCommand.Parameters.AddWithValue("@MaHopDong", newContractId);
                        updateRoomCommand.Parameters.AddWithValue("@SoPhong", txtSoPhong.Text);
                        updateRoomCommand.ExecuteNonQuery();

                        // Display success message
                        MessageBox.Show("Đăng ký hợp đồng thành công!");
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy thông tin sinh viên từ cơ sở dữ liệu sau khi thêm hợp đồng.");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
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
