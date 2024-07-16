using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using WpfApp1;

namespace StudentManagement
{
    public partial class Login : Window
    {
        private List<User> userList;

        public Login()
        {
            InitializeComponent();
            LoadUsersFromDatabase();
        }

        private void LoadUsersFromDatabase()
        {
            userList = new List<User>();
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT UserID, Username, PasswordHash, Role, Fullname, PhoneNumber, sex, DateOfBirth, SinhVienID FROM Users";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        userList.Add(new User
                        {
                            UserID = reader["UserID"].ToString(),
                            Username = reader["Username"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Role = reader["Role"].ToString(),
                            Fullname = reader["Fullname"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            sex = reader["sex"].ToString(),
                            DateOfBirth = reader["DateOfBirth"] as DateTime?,
                            SinhVienID = reader["SinhVienID"].ToString() // Lấy SinhVienID
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading users: " + ex.Message);
                }
            }
        }

        private bool IsValidCredentials(string username, string password, out User user)
        {
            user = userList.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user != null && user.PasswordHash == ComputeHash(password))
            {
                return true;
            }

            return false;
        }

        // Update when user is logged in
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text.Trim();
            string password = Password.Password;

            if (IsValidCredentials(username, password, out User user))
            {
                // Store current user information
                CurrentUser.UserID = user.UserID;
                CurrentUser.Username = user.Username;
                CurrentUser.PasswordHash = user.PasswordHash;
                CurrentUser.Role = user.Role;
                CurrentUser.Fullname = user.Fullname;
                CurrentUser.PhoneNumber = user.PhoneNumber;
                CurrentUser.sex = user.sex;
                CurrentUser.DateOfBirth = user.DateOfBirth;
                CurrentUser.SinhVienID = user.SinhVienID; // Load SinhVienID

                OpenAppropriateWindow(user.Role);
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        private string ComputeHash(string password)
        {
            // Implement hashing here if needed
            return password; // Placeholder; replace with actual hashing
        }

        private void OpenAppropriateWindow(string role)
        {
            if (role == "student")
            {
                StudentDashboard sd = new StudentDashboard();
                sd.Show();
            }
            else if (role == "admin")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        private void Username_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UsernamePlaceholder.Visibility = string.IsNullOrEmpty(Username.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(Password.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
        }
    }

    public class User
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SinhVienID { get; set; } // Added SinhVienID
    }

}
