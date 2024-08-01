using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoccerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       
        public string ConnString = "Server=(LocalDB)\\MSSQLLocalDB;Database=Football;TrustServerCertificate=True;MultipleActiveResultSets=true";
        private DataTable dt;

        public MainWindow()
        {
            InitializeComponent();
            ClubWindow clubWindow = new ClubWindow();
            clubWindow.Show();
            PlayerWindow playerWindow = new PlayerWindow();
            playerWindow.Show();
            dt = new DataTable();
            getData();
        }


        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            
            try
            {
                insertData(league_name.Text, countryTxt.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Event handler for Edit button click
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)leagueDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long leagueId = Convert.ToInt64(selectedRow["id"]);
                string leagueName = selectedRow["league_name"].ToString();
                string countryName = selectedRow["country"].ToString();
                // Open the edit window and pass the selected league's details
                EditLeagueWindow editWindow = new EditLeagueWindow(leagueId, leagueName, countryName);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
                // Event handler for Delete button click
                private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)leagueDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_league", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("League deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the league.");
                    }
                }

                // Refresh the DataGrid after deleting
                getData();
            }
        }
                

        // Update getData method to bind the DataTable to the DataGrid
        private void getData()
        {
            string ConnString = "Server=(LocalDB)\\MSSQLLocalDB;Database=Football;TrustServerCertificate=True;MultipleActiveResultSets=true";

            using (SqlConnection sqlConnection = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand("get_league", sqlConnection);
                sqlConnection.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt.Clear(); // Clear the existing data
                da.Fill(dt); // Fill the DataTable with new data
            }

            leagueDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
        }

        private  void insertData(string leaguename, string country)
        {
           
            using (SqlConnection sqlConnection = new SqlConnection(ConnString))
            {
                sqlConnection.Open();
                string query = "INSERT INTO Leagues (league_name, country) VALUES (@league_name, @country)";
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                cmd.Parameters.AddWithValue("@league_name", leaguename);
                cmd.Parameters.AddWithValue("@country", country);
                
                 cmd.ExecuteNonQuery();
                // Clear the input fields
                league_name.Text = "";
                    countryTxt.Text = "";
                    getData();
              

            }

        }


    }
}