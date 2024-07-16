using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace StudentManagement
{
    public partial class UserProfile : Window
    {
        public UserProfile()
        {
            InitializeComponent();
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            // Load user information from CurrentUser
            FullnameTextBlock.Text = CurrentUser.Fullname;
            PhoneNumberTextBlock.Text = CurrentUser.PhoneNumber;
            GenderTextBlock.Text = CurrentUser.sex;
            DateOfBirthTextBlock.Text = CurrentUser.DateOfBirth?.ToShortDateString() ?? "N/A";
            UsernameTextBox.Text = CurrentUser.Username;
            PasswordBox.Password = CurrentUser.PasswordHash; // Load password securely
            SinhVienIDTextBlock.Text = CurrentUser.SinhVienID; // Show SinhVienID
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text.Trim();
            string newPassword = PasswordBox.Password;

            if (UpdateUserProfile(newUsername, newPassword))
            {
                MessageBox.Show("Profile updated successfully!");
            }
            else
            {
                MessageBox.Show("Failed to update profile. Please try again.");
            }
        }

        private bool UpdateUserProfile(string username, string password)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

            string updateQuery = "UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@UserID", CurrentUser.UserID); // Use UserID to identify the user
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", HashPassword(password)); // Hash the new password

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating profile: " + ex.Message);
                    return false;
                }
            }
        }

        private string HashPassword(string password)
        {
            // Implement a secure password hashing algorithm here
            return password; // Placeholder; replace with actual hashing
        }
    }
}
