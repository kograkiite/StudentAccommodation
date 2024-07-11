using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace StudentManagement.Payment
{
    public partial class PaymentWindow : Window
    {
        public ObservableCollection<PaymentRecord> Payments { get; set; }

        public PaymentWindow()
        {
            InitializeComponent();
            Payments = new ObservableCollection<PaymentRecord>();
            dgPayments.ItemsSource = Payments;

            LoadPaymentsFromDatabase();
        }

        private void LoadPaymentsFromDatabase()
        {
            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT t.MaThuTien, t.MaSinhVien, t.TenSinhVien, t.SoDienThoai, t.SoPhong, t.GiaThue, t.MaHopDong " +
                           "FROM ThuTien t " +
                           "WHERE t.TrangThai = 1"; // Only select records with TrangThai = 1

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
                        Payments.Add(new PaymentRecord()
                        {
                            MaThuTien = Convert.ToInt32(row["MaThuTien"]),
                            MaSinhVien = row["MaSinhVien"].ToString(),
                            TenSinhVien = row["TenSinhVien"].ToString(),
                            SoDienThoai = row["SoDienThoai"].ToString(),
                            SoPhong = row["SoPhong"].ToString(),
                            GiaThue = Convert.ToDecimal(row["GiaThue"]),
                            MaHopDong = Convert.ToInt32(row["MaHopDong"]) // Assign MaHopDong property
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                dgPayments.ItemsSource = Payments;
            }
            else
            {
                var filteredList = Payments.Where(p =>
                    p.MaSinhVien.ToLower().Contains(searchText) ||
                    p.TenSinhVien.ToLower().Contains(searchText) ||
                    p.SoDienThoai.ToLower().Contains(searchText)
                ).ToList();

                dgPayments.ItemsSource = filteredList;
            }
        }

    }

    public class PaymentRecord
    {
        public int MaThuTien { get; set; }
        public string MaSinhVien { get; set; }
        public string TenSinhVien { get; set; }
        public string SoDienThoai { get; set; }
        public string SoPhong { get; set; }
        public decimal GiaThue { get; set; }
        public int MaHopDong { get; set; }
    }

}
