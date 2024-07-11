using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace StudentManagement
{
    public partial class EditContractWindow : Window
    {
        public Contract UpdatedContract { get; private set; }
        private Contract tempContract;
        private ObservableCollection<Contract> Contracts;
        private int SelectedIndex;

        public EditContractWindow(Contract contract, ObservableCollection<Contract> contracts, int selectedIndex)
        {
            InitializeComponent();
            Contracts = contracts;
            SelectedIndex = selectedIndex;

            // Tạo một bản sao của hợp đồng để hiển thị trong giao diện
            tempContract = new Contract
            {
                MaHopDong = contract.MaHopDong,
                MaSinhVien = contract.MaSinhVien,
                TenSinhVien = contract.TenSinhVien,
                SoDienThoai = contract.SoDienThoai,
                SoPhong = contract.SoPhong,
                NgayBatDau = contract.NgayBatDau,
                NgayKetThuc = contract.NgayKetThuc
            };

            DataContext = tempContract;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tempContract.NgayBatDau > tempContract.NgayKetThuc)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                return;
            }

            string connectionString = "Data Source=DESKTOP-C809PVE\\SQLEXPRESS01;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";

            string query = "UPDATE HopDong SET MaSinhVien = @MaSinhVien, TenSinhVien = @TenSinhVien, SoDienThoai = @SoDienThoai, SoPhong = @SoPhong, NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc WHERE MaHopDong = @MaHopDong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaSinhVien", tempContract.MaSinhVien);
                command.Parameters.AddWithValue("@TenSinhVien", tempContract.TenSinhVien);
                command.Parameters.AddWithValue("@SoDienThoai", tempContract.SoDienThoai);
                command.Parameters.AddWithValue("@SoPhong", tempContract.SoPhong);
                command.Parameters.AddWithValue("@NgayBatDau", tempContract.NgayBatDau);
                command.Parameters.AddWithValue("@NgayKetThuc", tempContract.NgayKetThuc);
                command.Parameters.AddWithValue("@MaHopDong", tempContract.MaHopDong);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        UpdatedContract = tempContract; // Cập nhật giá trị của UpdatedContract

                        // Cập nhật ObservableCollection trực tiếp
                        if (Contracts != null && SelectedIndex >= 0 && SelectedIndex < Contracts.Count)
                        {
                            Contracts[SelectedIndex] = UpdatedContract;
                        }

                        MessageBox.Show("Sửa hợp đồng thành công!");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Sửa hợp đồng không thành công!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
