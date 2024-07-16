using Microsoft.Data.SqlClient;
using System.Windows;

namespace StudentManagement
{
    public partial class PaymentForStudent : Window
    {
        public PaymentForStudent()
        {
            InitializeComponent();
            LoadThuTien();
        }

        private void LoadThuTien()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT MaThuTien, MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, GiaThue, TrangThai, MaHopDong " +
                           "FROM ThuTien WHERE MaSinhVien = @MaSinhVien";

            List<ThuTien> thuTienList = new List<ThuTien>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", CurrentUser.SinhVienID); // Sử dụng SinhVienID từ CurrentUser

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        thuTienList.Add(new ThuTien
                        {
                            MaThuTien = (int)reader["MaThuTien"],
                            MaSinhVien = reader["MaSinhVien"].ToString(),
                            TenSinhVien = reader["TenSinhVien"].ToString(),
                            SoDienThoai = reader["SoDienThoai"]?.ToString(),
                            SoPhong = reader["SoPhong"].ToString(),
                            GiaThue = (decimal)reader["GiaThue"],
                            TrangThai = (bool)reader["TrangThai"],
                            MaHopDong = reader["MaHopDong"] as int? // Nullable
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading payment information: " + ex.Message);
                }
            }

            ThuTienDataGrid.ItemsSource = thuTienList;
        }
    }

    public class ThuTien
    {
        public int MaThuTien { get; set; }
        public string MaSinhVien { get; set; }
        public string TenSinhVien { get; set; }
        public string SoDienThoai { get; set; }
        public string SoPhong { get; set; }
        public decimal GiaThue { get; set; }
        public bool TrangThai { get; set; }
        public int? MaHopDong { get; set; } // Nullable nếu không có giá trị
    }
}
