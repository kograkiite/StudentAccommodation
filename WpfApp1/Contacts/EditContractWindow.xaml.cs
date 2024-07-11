using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement
{
    public partial class EditContractWindow : Window
    {
        public Contract UpdatedContract { get; private set; }
        private Contract tempContract;
        private ObservableCollection<Contract> Contracts;
        private int SelectedIndex;

        // Properties for RadioButtons
        public bool IsConHan
        {
            get { return tempContract.TrangThai; }
            set
            {
                if (value)
                    tempContract.TrangThai = true;
            }
        }

        public bool IsHetHan
        {
            get { return !tempContract.TrangThai; }
            set
            {
                if (value)
                    tempContract.TrangThai = false;
            }
        }

        public EditContractWindow(Contract contract, ObservableCollection<Contract> contracts, int selectedIndex)
        {
            InitializeComponent();
            Contracts = contracts;
            SelectedIndex = selectedIndex;

            // Create a copy of the contract to display in the UI
            tempContract = new Contract
            {
                MaHopDong = contract.MaHopDong,
                MaSinhVien = contract.MaSinhVien,
                TenSinhVien = contract.TenSinhVien,
                SoDienThoai = contract.SoDienThoai,
                SoPhong = contract.SoPhong,
                NgayBatDau = contract.NgayBatDau,
                NgayKetThuc = contract.NgayKetThuc,
                TrangThai = contract.TrangThai // Assign TrangThai from the original contract
            };

            DataContext = tempContract;

            // Set default value for RadioButtons based on TrangThai
            if (tempContract.TrangThai) // true means "Còn hạn"
            {
                IsConHan = true;
            }
            else // false means "Hết hạn"
            {
                IsHetHan = true;
            }
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

            // Query update cho HopDong
            string updateHopDongQuery = "UPDATE HopDong SET MaSinhVien = @MaSinhVien, TenSinhVien = @TenSinhVien, SoDienThoai = @SoDienThoai, SoPhong = @SoPhong, NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc, TrangThai = @TrangThai WHERE MaHopDong = @MaHopDong";

            // Query update cho ThuTien
            string updateThuTienQuery = "UPDATE ThuTien SET TrangThai = @TrangThai WHERE MaHopDong = @MaHopDong";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Update HopDong
                    SqlCommand hopDongCommand = new SqlCommand(updateHopDongQuery, connection, transaction);
                    hopDongCommand.Parameters.AddWithValue("@MaSinhVien", tempContract.MaSinhVien);
                    hopDongCommand.Parameters.AddWithValue("@TenSinhVien", tempContract.TenSinhVien);
                    hopDongCommand.Parameters.AddWithValue("@SoDienThoai", tempContract.SoDienThoai);
                    hopDongCommand.Parameters.AddWithValue("@SoPhong", tempContract.SoPhong);
                    hopDongCommand.Parameters.AddWithValue("@NgayBatDau", tempContract.NgayBatDau);
                    hopDongCommand.Parameters.AddWithValue("@NgayKetThuc", tempContract.NgayKetThuc);
                    hopDongCommand.Parameters.AddWithValue("@TrangThai", tempContract.TrangThai);
                    hopDongCommand.Parameters.AddWithValue("@MaHopDong", tempContract.MaHopDong);

                    int hopDongRowsAffected = hopDongCommand.ExecuteNonQuery();

                    // Update ThuTien
                    SqlCommand thuTienCommand = new SqlCommand(updateThuTienQuery, connection, transaction);
                    thuTienCommand.Parameters.AddWithValue("@TrangThai", tempContract.TrangThai);
                    thuTienCommand.Parameters.AddWithValue("@MaHopDong", tempContract.MaHopDong);

                    int thuTienRowsAffected = thuTienCommand.ExecuteNonQuery();

                    // Commit transaction if both updates succeed
                    if (hopDongRowsAffected > 0 && thuTienRowsAffected > 0)
                    {
                        transaction.Commit();

                        UpdatedContract = tempContract; // Update UpdatedContract value

                        // Update ObservableCollection directly
                        if (Contracts != null && SelectedIndex >= 0 && SelectedIndex < Contracts.Count)
                        {
                            Contracts[SelectedIndex] = UpdatedContract;
                        }

                        MessageBox.Show("Sửa hợp đồng và Thu Tiền thành công!");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        transaction.Rollback(); // Rollback if either update fails
                        MessageBox.Show("Sửa hợp đồng và Thu Tiền không thành công!");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Event handler for RadioButton checked events
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Ensure only one RadioButton is checked at a time
            var radioButton = sender as RadioButton;
            bool isChecked = radioButton.IsChecked ?? false;

            if (isChecked)
            {
                if (radioButton.Content.ToString() == "Còn hạn")
                    tempContract.TrangThai = true;
                else if (radioButton.Content.ToString() == "Hết hạn")
                    tempContract.TrangThai = false;
            }
        }
    }
}
