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
    public partial class PlayerWindow : Window
    {
        private DataTable dt;
        public string ConnString = "Server=(LocalDB)\\MSSQLLocalDB;Database=Football;TrustServerCertificate=True;MultipleActiveResultSets=true";

        public PlayerWindow()
        {
            InitializeComponent();
            dt = new DataTable();
            getData();
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
          

            try
            {
                insertData(playerNameTxt.Text, int.Parse(ageTxt.Text), positionTxt.Text);
              
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message} ");
            }
        }
        // Event handler for Edit button click
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long playerId = Convert.ToInt64(selectedRow["id"]);
                string playerName = selectedRow["player_name"].ToString();
                long ageNo = Convert.ToInt64(selectedRow["age"]);
                string position = selectedRow["position"].ToString();
                
                // Open the edit window and pass the selected league's details
                EditPlayerWindow editWindow = new EditPlayerWindow(playerId, playerName, ageNo, position);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
        // Event handler for Delete button click
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_player", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Player deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the Player.");
                    }
                }

                // Refresh the DataGrid after deleting
                getData();
            }
        }


        // Update getData method to bind the DataTable to the DataGrid
        private void getData()
        {


            using (SqlConnection sqlConnection = new SqlConnection(ConnString))
            {
                // Bind the DataTable to the DataGrid as before
                SqlConnection connection = new SqlConnection(ConnString);
                SqlCommand playerCmd = new SqlCommand("get_player", connection);
                SqlDataAdapter da = new SqlDataAdapter(playerCmd);
                dt.Clear(); // Clear the existing data
                 // Fill the DataTable with new data

                playerDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
            }
        }

        private void insertData (string player_name, int age, string position)
        {

            using (SqlConnection sqlConnection = new SqlConnection(ConnString))
            {
                sqlConnection.Open();
                string query = "INSERT INTO players (player_name, age, position) VALUES (@player_name, @age, @position)";
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                cmd.Parameters.AddWithValue("@player_name", player_name);
                cmd.Parameters.AddWithValue("@age", age);
                cmd.Parameters.AddWithValue("@position", position);
                

                
                // Clear the input fields
                playerNameTxt.Text = "";
                ageTxt.Text = "";
                positionTxt.Text = "";
               

                getData();


            }

        }


    }
}