using System.Data.SqlTypes;
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
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System;

using System.Data;
//using Microsoft.Data.SqlClient;



namespace Library
{
    public partial class MainWindow : Window
    {
        private SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
        }



        // Connect to the SQL Server
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = "Server=DESKTOP-OP5TL27\\SQLEXPRESS;initial catalog = BookShop; integrated security = true;";
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                StatusTextBlock.Text = "Status : Connected";
                MessageBox.Show("Connected to SQL Server successfully!", "Connection Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SQL Server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Disconnect from the SQL Server
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                    StatusTextBlock.Text = "Status : Disconnected";
                    MessageBox.Show("Disconnected from SQL Server.", "Connection Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No active connection to disconnect.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error disconnecting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load data from the SQL Server
        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
                {
                    MessageBox.Show("Please connect to the SQL Server first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string query = "SELECT * FROM Authors"; // Replace with your table name and column names
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                LData.Items.Clear(); // Clear previous items before loading new data
                while (sqlDataReader.Read())
                {
                    LData.Items.Add($"{sqlDataReader["Name"]} {sqlDataReader["Surname"]}");
                }

                sqlDataReader.Close(); // Close the reader after iterating through the rows
                MessageBox.Show("Data loaded successfully!", "Load Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnCreateTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
                {
                    MessageBox.Show("Please connect to the SQL Server first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string createTableQuery = @"
                    CREATE TABLE Authors (
                        ID INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(MAX) NOT NULL,
                        Surname NVARCHAR(MAX) NOT NULL,
                        CountryId INT NOT NULL
                    );
                    CREATE TABLE Books(
                        Id INT NULL IDENTITY(1,1) PRIMARY KEY,
                        AuthorId INT NOT NULL references Authors(Id),
                        Title VARCHAR(100) NOT NULL,
                        Price INT NULL,
                        Page INT NULL
                    )";

                SqlCommand sqlCommand = new SqlCommand(createTableQuery, sqlConnection);
                sqlCommand.ExecuteNonQuery();

                MessageBox.Show("Table created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating table: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if connection is established
                if (sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Please connect to the SQL Server first.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ensure inputs are not empty
                if (string.IsNullOrWhiteSpace(FirstName.Text) || string.IsNullOrWhiteSpace(LastName.Text))
                {
                    MessageBox.Show("Both First Name and Last Name must be filled.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // SQL Insert Query
                string insertQuery = "INSERT INTO Authors (Name, Surname) VALUES (@FirstName, @LastName)";

                // Execute the query with parameters
                using (SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@FirstName", FirstName.Text.Trim());
                    sqlCommand.Parameters.AddWithValue("@LastName", LastName.Text.Trim());

                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data inserted successfully!", "Insert Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Optionally clear the textboxes after successful insertion
                        FirstName.Clear();
                        LastName.Clear();
                    }
                    else
                    {
                        MessageBox.Show("No rows were inserted. Please try again.", "Insert Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}