using Microsoft.Data.SqlClient;
using StudentManagement.Payment;
using StudentManagement;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StudentManagement.Rooms;

namespace StudentManagement
{
    public partial class StudentDashboard : Window
    {
        public StudentDashboard()
        {
            InitializeComponent();
        }

        private void RegisterContract_Click(object sender, RoutedEventArgs e)
        {
            RegisterContractWindow rc = new RegisterContractWindow();
            rc.Show();
        }

        private void RoomStudent_Click(object sender, RoutedEventArgs e)
        {
            RoomWithStudent roomWithStudent = new RoomWithStudent();
            roomWithStudent.Show();
        }
        private void Payment_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow pw = new PaymentWindow();
            pw.Show();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Show the Login window
            Login loginWindow = new Login();
            loginWindow.Show();

            // Close the MainWindow
            this.Close();
        }
    }
}
