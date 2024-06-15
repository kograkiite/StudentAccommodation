using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement
{
    public partial class ElectricBillWindow : Window
    {
        public ObservableCollection<ElectricBill> ElectricBills { get; set; }

        public ElectricBillWindow()
        {
            InitializeComponent();
            ElectricBills = new ObservableCollection<ElectricBill>();
            dataGrid.ItemsSource = ElectricBills;
            LoadElectricBillsFromDatabase();
        }

        private void LoadElectricBillsFromDatabase()
        {
            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT TOP (1000) [MaTienDien], [MaHopDong], [SoDien], [NgayTinhTien], [ThanhTien] FROM [StudentManagement].[dbo].[TienDien]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        ElectricBills.Add(new ElectricBill()
                        {
                            MaTienDien = Convert.ToInt32(row["MaTienDien"]),
                            MaHopDong = Convert.ToInt32(row["MaHopDong"]),
                            SoDien = Convert.ToInt32(row["SoDien"]),
                            NgayTinhTien = Convert.ToDateTime(row["NgayTinhTien"]),
                            ThanhTien = Convert.ToDecimal(row["ThanhTien"])
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string maHopDongText = txtMaHopDong.Text.Trim();
            if (string.IsNullOrEmpty(maHopDongText) || !int.TryParse(maHopDongText, out int maHopDong))
            {
                // Clear previous search results
                ElectricBills.Clear();
                LoadElectricBillsFromDatabase();
            }
            else
            {
                // Perform search based on MaHopDong
                SearchByMaHopDong(maHopDong);
            }
        }
        private void SearchByMaHopDong(int maHopDong)
        {
            ElectricBills.Clear();

            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = $"SELECT [MaTienDien], [MaHopDong], [SoDien], [NgayTinhTien], [ThanhTien] FROM [StudentManagement].[dbo].[TienDien] WHERE MaHopDong = {maHopDong}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        ElectricBills.Add(new ElectricBill()
                        {
                            MaTienDien = Convert.ToInt32(row["MaTienDien"]),
                            MaHopDong = Convert.ToInt32(row["MaHopDong"]),
                            SoDien = Convert.ToInt32(row["SoDien"]),
                            NgayTinhTien = Convert.ToDateTime(row["NgayTinhTien"]),
                            ThanhTien = Convert.ToDecimal(row["ThanhTien"])
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void txtMaHopDong_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaHopDong.Text == "Nhập mã hợp đồng")
            {
                txtMaHopDong.Text = "";
            }
        }

        private void txtMaHopDong_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHopDong.Text))
            {
                txtMaHopDong.Text = "Nhập mã hợp đồng";
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddElectricBillWindow addWindow = new AddElectricBillWindow();
            addWindow.ShowDialog();
            // Refresh the electric bills list after adding a new bill
            ElectricBills.Clear();
            LoadElectricBillsFromDatabase();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Get selected item from dataGrid
            ElectricBill selectedBill = dataGrid.SelectedItem as ElectricBill;

            if (selectedBill != null)
            {
                EditElectricBillWindow editWindow = new EditElectricBillWindow(selectedBill);
                editWindow.ShowDialog();
                // Refresh the electric bills list after editing a bill
                ElectricBills.Clear();
                LoadElectricBillsFromDatabase();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để sửa.");
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Không thể xóa hóa đơn này.");
        }

    }

    public class ElectricBill
    {
        public int MaTienDien { get; set; }
        public int MaHopDong { get; set; }
        public int SoDien { get; set; }
        public DateTime NgayTinhTien { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
