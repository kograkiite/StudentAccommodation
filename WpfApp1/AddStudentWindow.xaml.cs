using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace WpfApp1
{
    public partial class AddStudentWindow : Window
    {
        public SinhVien NewStudent { get; private set; }

        public AddStudentWindow()
        {
            InitializeComponent();

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtFullname.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                (!rbMale.IsChecked.HasValue || !rbFemale.IsChecked.HasValue) ||
                !dpDateOfBirth.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "INSERT INTO SinhVien (id, fullname, phoneNumber, sex, dateOfBirth) VALUES (@id, @fullname, @phoneNumber, @sex, @dateOfBirth)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", txtId.Text);
                command.Parameters.AddWithValue("@fullname", txtFullname.Text);
                command.Parameters.AddWithValue("@phoneNumber", txtPhoneNumber.Text);
                command.Parameters.AddWithValue("@sex", rbMale.IsChecked == true ? "Nam" : "Nữ");
                command.Parameters.AddWithValue("@dateOfBirth", dpDateOfBirth.SelectedDate.Value);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm sinh viên thành công.");
                        NewStudent = new SinhVien
                        {
                            id = txtId.Text,
                            fullname = txtFullname.Text,
                            phoneNumber = txtPhoneNumber.Text,
                            sex = rbMale.IsChecked == true ? "Nam" : "Nữ",
                            dateOfBirth = dpDateOfBirth.SelectedDate.Value
                        };

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Thêm sinh viên không thành công.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
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
