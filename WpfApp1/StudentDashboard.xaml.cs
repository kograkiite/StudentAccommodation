using StudentManagement.Rooms;
using System.Windows;

namespace StudentManagement
{
    public partial class StudentDashboard : Window
    {
        private User currentUser;
        private string Fullname;

        public StudentDashboard(User user)
        {
            InitializeComponent();
            currentUser = user;
            Fullname = user.Fullname;
            Welcome.Text = "Xin chào, " + Fullname;
        }

        private void RegisterContract_Click(object sender, RoutedEventArgs e)
        {
            ContractWindowForStudent rc = new ContractWindowForStudent();
            rc.Show();
        }

        private void RoomStudent_Click(object sender, RoutedEventArgs e)
        {
            RoomWithStudent roomWithStudent = new RoomWithStudent();
            roomWithStudent.Show();
        }

        private void Payment_Click(object sender, RoutedEventArgs e)
        {
            PaymentForStudent pw = new PaymentForStudent();
            pw.Show();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Show the Login window
            Login loginWindow = new Login();
            loginWindow.Show();

            // Close the current window
            this.Close();
        }

        private void btnShowProfile_Click(object sender, RoutedEventArgs e)
        {
            UserProfile pw = new UserProfile();
            pw.Show();
        }

        private void btnRegisterRoom_Click(object sender, RoutedEventArgs e)
        {
            RegisterRoomWindow registerRoomWindow = new RegisterRoomWindow(currentUser);
            registerRoomWindow.Show();
        }

        private void btnViewRoomRequest_Click(object sender, RoutedEventArgs e)
        {
            RoomRequestWindow roomRequestWindow = new RoomRequestWindow(currentUser);
            roomRequestWindow.Show();
        }
    }
}
