using Microsoft.Data.SqlClient;
using System.Windows;

namespace StudentManagement
{
    public partial class ContractWindowForStudent : Window
    {
        public ContractWindowForStudent()
        {
            InitializeComponent();
            LoadContracts();
        }

        private void LoadContracts()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT MaHopDong, MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayBatDau, NgayKetThuc, TrangThai " +
                           "FROM HopDong WHERE MaSinhVien = @MaSinhVien";

            List<Contract> contracts = new List<Contract>();

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
                        contracts.Add(new Contract
                        {
                            MaHopDong = (int)reader["MaHopDong"],
                            MaSinhVien = reader["MaSinhVien"].ToString(), // Thêm mã sinh viên
                            TenSinhVien = reader["TenSinhVien"].ToString(),
                            SoDienThoai = reader["SoDienThoai"]?.ToString(),
                            SoPhong = reader["SoPhong"].ToString(),
                            NgayBatDau = (DateTime)reader["NgayBatDau"],
                            NgayKetThuc = (DateTime)reader["NgayKetThuc"],
                            TrangThai = (bool)reader["TrangThai"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading contracts: " + ex.Message);
                }
            }

            ContractsDataGrid.ItemsSource = contracts;
        }
    }

    public class Contract2
    {
        public int MaHopDong { get; set; }
        public string MaSinhVien { get; set; } // Thêm mã sinh viên
        public string TenSinhVien { get; set; }
        public string SoDienThoai { get; set; }
        public string SoPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }
    }
}
