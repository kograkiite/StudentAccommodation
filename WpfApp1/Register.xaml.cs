using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace StudentManagement
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }
        public event Action<User> UserRegistered;
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text.Trim();
            string password = Password.Password;
            string confirmPassword = ConfirmPassword.Password;
            string sinhVienID = SinhVienID.Text.Trim();
            string fullname = Fullname.Text.Trim();
            string phoneNumber = PhoneNumber.Text.Trim();
            string gender = rbMale.IsChecked == true ? "Mal" : "Fel"; // Match gender to DB

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.");
                return;
            }

            if (!ValidateSinhVienID(sinhVienID))
            {
                MessageBox.Show("MaSinhVien is not in the correct format. Please use format SEXXXXXX where X is a digit from 1 to 9.");
                return;
            }

            if (!ValidatePhoneNumber(phoneNumber))
            {
                MessageBox.Show("Invalid phone number. Please enter a positive number.");
                return;
            }

            if (RegisterStudentAndUser(username, password, sinhVienID, fullname, phoneNumber, gender))
            {
                MessageBox.Show("Registration successful!");

                // Notify the Login window of the new user registration
                UserRegistered?.Invoke(new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password),
                    Role = "student",
                    Fullname = fullname,
                    PhoneNumber = phoneNumber,
                    sex = gender,
                    DateOfBirth = DateOfBirth.SelectedDate.Value,
                    SinhVienID = sinhVienID
                });

                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.");
            }
        }

        private bool ValidateSinhVienID(string sinhVienID)
        {
            // Check if SinhVienID matches the format "SEXxxxxxx"
            Regex regex = new Regex(@"^SE[0-9]{6}$");
            return regex.IsMatch(sinhVienID);
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            // Check if PhoneNumber contains only positive digits
            return phoneNumber.All(char.IsDigit) && !phoneNumber.StartsWith("-");
        }

        private bool RegisterStudentAndUser(string username, string password, string sinhVienID, string fullname, string phoneNumber, string gender)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

            // Insert into SinhVien
            string insertStudentQuery = "INSERT INTO SinhVien (id, fullname, phoneNumber, sex, dateOfBirth, SoPhong) " +
                                         "VALUES (@SinhVienID, @Fullname, @PhoneNumber, @Gender, @DateOfBirth, NULL)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand studentCommand = new SqlCommand(insertStudentQuery, connection);
                studentCommand.Parameters.AddWithValue("@SinhVienID", sinhVienID);
                studentCommand.Parameters.AddWithValue("@Fullname", fullname);
                studentCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                studentCommand.Parameters.AddWithValue("@Gender", gender);
                studentCommand.Parameters.AddWithValue("@DateOfBirth", DateOfBirth.SelectedDate.Value);

                try
                {
                    connection.Open();
                    int studentResult = studentCommand.ExecuteNonQuery();

                    if (studentResult <= 0) return false;

                    // Insert into Users
                    string insertUserQuery = "INSERT INTO Users (Username, PasswordHash, Role, SinhVienID, fullname, phoneNumber, sex, dateOfBirth) " +
                                              "VALUES (@Username, @PasswordHash, 'student', @SinhVienID, @Fullname, @PhoneNumber, @Gender, @DateOfBirth)";

                    SqlCommand userCommand = new SqlCommand(insertUserQuery, connection);
                    userCommand.Parameters.AddWithValue("@Username", username);
                    userCommand.Parameters.AddWithValue("@PasswordHash", HashPassword(password)); // Hash the password
                    userCommand.Parameters.AddWithValue("@SinhVienID", sinhVienID);
                    userCommand.Parameters.AddWithValue("@Fullname", fullname);
                    userCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    userCommand.Parameters.AddWithValue("@Gender", gender);
                    userCommand.Parameters.AddWithValue("@DateOfBirth", DateOfBirth.SelectedDate.Value);

                    int userResult = userCommand.ExecuteNonQuery();
                    return userResult > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error registering user: " + ex.Message);
                    return false;
                }
            }
        }

        private string HashPassword(string password)
        {
            // Implement a secure password hashing algorithm here
            return password; // Placeholder; replace with actual hashing
        }

        private void Username_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UsernamePlaceholder.Visibility = string.IsNullOrEmpty(Username.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(Password.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SinhVienID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SinhVienIDPlaceholder.Visibility = string.IsNullOrEmpty(SinhVienID.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Fullname_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FullnamePlaceholder.Visibility = string.IsNullOrEmpty(Fullname.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PhoneNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PhoneNumberPlaceholder.Visibility = string.IsNullOrEmpty(PhoneNumber.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
