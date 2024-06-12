using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace WpfApp1
{
    public partial class UpdateStudentWindow : Window
    {
        public SinhVien UpdatedStudent { get; private set; }

        public UpdateStudentWindow(SinhVien student)
        {
            InitializeComponent();

            // Điền thông tin sinh viên vào các trường
            txtId.Text = student.id;
            txtFullname.Text = student.fullname;
            txtPhoneNumber.Text = student.phoneNumber;
            rbMale.IsChecked = student.sex == "Nam";
            rbFemale.IsChecked = student.sex == "Nữ";
            dpDateOfBirth.SelectedDate = student.dateOfBirth;

            // Không cho phép chỉnh sửa ID
            txtId.IsEnabled = false;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu các trường thông tin bị bỏ trống
            if (string.IsNullOrWhiteSpace(txtFullname.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                (!rbMale.IsChecked.HasValue && !rbFemale.IsChecked.HasValue) ||
                !dpDateOfBirth.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "UPDATE SinhVien SET fullname = @fullname, phoneNumber = @phoneNumber, sex = @sex, dateOfBirth = @dateOfBirth WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", txtId.Text);
                command.Parameters.AddWithValue("@fullname", txtFullname.Text);
                command.Parameters.AddWithValue("@phoneNumber", txtPhoneNumber.Text);
                command.Parameters.AddWithValue("@sex", rbMale.IsChecked == true ? "Nam" : "Nữ");
                command.Parameters.AddWithValue("@dateOfBirth", dpDateOfBirth.SelectedDate.HasValue ? dpDateOfBirth.SelectedDate.Value : DateTime.MinValue);

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
                            dateOfBirth = dpDateOfBirth.SelectedDate.HasValue ? dpDateOfBirth.SelectedDate.Value : DateTime.MinValue
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


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
