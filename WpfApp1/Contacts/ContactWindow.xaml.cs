using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement
{
    public partial class ContractWindow : Window
    {
        public ObservableCollection<Contract> Contracts { get; set; }

        public ContractWindow()
        {
            InitializeComponent();
            Contracts = new ObservableCollection<Contract>();
            dataGrid.ItemsSource = Contracts;

            LoadContractsFromDatabase();
        }

        private void LoadContractsFromDatabase()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT hd.MaHopDong, hd.MaSinhVien, sv.fullname AS TenSinhVien, sv.phoneNumber AS SoDienThoai, hd.SoPhong, hd.NgayBatDau, hd.NgayKetThuc, hd.TrangThai " +
                           "FROM HopDong hd " +
                           "INNER JOIN SinhVien sv ON hd.MaSinhVien = sv.id";

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
                        Contracts.Add(new Contract()
                        {
                            MaHopDong = Convert.ToInt32(row["MaHopDong"]),
                            MaSinhVien = row["MaSinhVien"].ToString(),
                            TenSinhVien = row["TenSinhVien"].ToString(),
                            SoDienThoai = row["SoDienThoai"].ToString(),
                            SoPhong = row["SoPhong"].ToString(),
                            NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
                            NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]),
                            TrangThai = Convert.ToBoolean(row["TrangThai"]), // Convert database BIT to boolean
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



        private void btnRegisterContract_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterContractWindow();
            if (registerWindow.ShowDialog() == true)
            {
                Contracts.Add(registerWindow.NewContract);
            }
        }

        private void btnEditContract_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng để chỉnh sửa.");
                return;
            }

            Contract selectedContract = (Contract)dataGrid.SelectedItem;
            if (selectedContract == null)
            {
                MessageBox.Show("Hợp đồng được chọn không hợp lệ.");
                return;
            }

            int selectedIndex = Contracts.IndexOf(selectedContract);
            if (selectedIndex < 0 || selectedIndex >= Contracts.Count)
            {
                MessageBox.Show("Chỉ số hợp đồng không hợp lệ.");
                return;
            }

            EditContractWindow editWindow = new EditContractWindow(selectedContract, Contracts, selectedIndex);

            if (editWindow.ShowDialog() == true)
            {
                Contract updatedContract = editWindow.UpdatedContract;
                if (updatedContract != null)
                {
                    Contracts[selectedIndex] = updatedContract;
                }
                else
                {
                    MessageBox.Show("Hợp đồng cập nhật không hợp lệ.");
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                dataGrid.ItemsSource = Contracts;
            }
            else
            {
                var filteredList = Contracts.Where(c =>
                    c.MaSinhVien.ToLower().Contains(searchText) ||
                    c.TenSinhVien.ToLower().Contains(searchText) ||
                    c.SoDienThoai.ToLower().Contains(searchText)
                ).ToList();

                dataGrid.ItemsSource = filteredList;
            }
        }

    }

    public class Contract
    {
        public int MaHopDong { get; set; }
        public string MaSinhVien { get; set; }
        public string TenSinhVien { get; set; }
        public string SoDienThoai { get; set; }
        public string SoPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        private bool trangThai; // Change trangThai to boolean type
        public bool TrangThai
        {
            get => trangThai;
            set
            {
                trangThai = value;
                TrangThaiDisplay = trangThai ? "Còn hạn" : "Hết hạn";
            }
        }
        public string TrangThaiDisplay { get; private set; }
        // Constructor to initialize TrangThaiDisplay based on TrangThai
        public Contract()
        {
            TrangThai = true; // Default to "Còn hạn" when a new contract is created
        }
    }



}