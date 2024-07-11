using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
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
            string query = "SELECT * FROM ThuTien WHERE TrangThai = 1"; // Chỉ lấy các bản ghi có TrangThai là 1

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
                            GiaThue = Convert.ToDecimal(row["GiaThue"])
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
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
    }
}
