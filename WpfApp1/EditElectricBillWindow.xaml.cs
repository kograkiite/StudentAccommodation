using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentManagement
{
    public partial class EditElectricBillWindow : Window
    {
        private ElectricBill billToEdit;

        public EditElectricBillWindow(ElectricBill bill)
        {
            InitializeComponent();
            billToEdit = bill;
            LoadData();
        }

        private void LoadData()
        {
            if (billToEdit != null)
            {
                txtMaTienDien.Text = billToEdit.MaTienDien.ToString();
                txtMaHopDong.Text = billToEdit.MaHopDong.ToString();
                txtSoDien.Text = billToEdit.SoDien.ToString();
                dpNgayTinhTien.SelectedDate = billToEdit.NgayTinhTien;
                txtThanhTien.Text = billToEdit.ThanhTien.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update properties of billToEdit with new values from UI
            billToEdit.MaHopDong = int.Parse(txtMaHopDong.Text);
            billToEdit.SoDien = int.Parse(txtSoDien.Text);
            billToEdit.NgayTinhTien = dpNgayTinhTien.SelectedDate ?? DateTime.Now; // Handle null case if necessary
            billToEdit.ThanhTien = decimal.Parse(txtThanhTien.Text);

            // Update the database
            UpdateDatabase(billToEdit);

            // For demonstration, assuming saving is done directly in the main window
            MessageBox.Show("Saved changes.");

            // Close the window
            Close();
        }

        private void UpdateDatabase(ElectricBill bill)
        {
            // Implement your SQL update logic here
            // Example:
            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = $"UPDATE [TienDien] SET MaHopDong = {bill.MaHopDong}, SoDien = {bill.SoDien}, NgayTinhTien = '{bill.NgayTinhTien.ToString("yyyy-MM-dd")}', ThanhTien = {bill.ThanhTien} WHERE MaTienDien = '{bill.MaTienDien}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating data: " + ex.Message);
                }
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel editing and close the window
            Close();
        }
    }
}
