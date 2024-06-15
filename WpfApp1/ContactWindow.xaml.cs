using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

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
            string connectionString = "Data Source=LAPTOP-DI-DONG;Initial Catalog=StudentManagement;Integrated Security=True;Trust Server Certificate=True";
            string query = "SELECT * FROM HopDong";

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
                            SoPhong = row["SoPhong"].ToString(),
                            NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
                            NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"])
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
            EditContractWindow editWindow = new EditContractWindow(selectedContract);

            if (editWindow.ShowDialog() == true)
            {
                Contract updatedContract = editWindow.UpdatedContract;
                int index = Contracts.IndexOf(selectedContract);
                Contracts[index] = updatedContract;
            }
        }
    }   

    public class Contract
    {
        public int MaHopDong { get; set; }
        public string MaSinhVien { get; set; }
        public string SoPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }
}
