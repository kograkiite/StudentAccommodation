using ClassLibrary1.Models;
using System;
using System.Linq;
using System.Windows;

namespace PE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadStaffMembers();
        }

        private void LoadStaffMembers()
        {
            using (var dbcontext = new AirConditionerShop2023DbContext())
            {
                var list = dbcontext.StaffMembers.ToList();
                dgvStaff.ItemsSource = list;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var staff = new StaffMember
            {
                MemberId = DateTime.Now.Minute + DateTime.Now.Second,
                FullName = "abcd",
                EmailAddress = "abc@gmail.com",
                Password = "@@@342",
                Role = 0
            };

            using (var dbcontext = new AirConditionerShop2023DbContext())
            {
                dbcontext.StaffMembers.Add(staff);
                dbcontext.SaveChanges();
            }

            LoadStaffMembers(); // Refresh the DataGrid
        }
    }
}
