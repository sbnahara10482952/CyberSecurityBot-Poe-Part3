using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CyberSecurityBotGUI
{
    public class DatabaseHelper
    {
        private string connectionString =
            "server=localhost;database=CyberSecurityBot;user=root;password=Theboysibs11@;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Initialize Database (Create Table if not exists)
        public void InitializeDatabase()
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        Title VARCHAR(255) NOT NULL,
                        Description TEXT,
                        ReminderDate DATETIME,
                        Status VARCHAR(50) DEFAULT 'Pending',
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Add Task with Description and Reminder
        public void AddTask(string title, string description, DateTime? reminderDate)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Tasks (Title, Description, ReminderDate, Status, CreatedAt) " +
                               "VALUES (@title, @description, @date, 'Pending', @createdAt)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(description) ? "" : description);
                    cmd.Parameters.AddWithValue("@date", reminderDate.HasValue ? reminderDate.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get All Tasks
        public DataTable GetAllTasks()
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Title, Description, ReminderDate, Status, CreatedAt FROM Tasks ORDER BY CreatedAt DESC";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Get Pending Tasks
        public DataTable GetPendingTasks()
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Title, Description, ReminderDate, Status, CreatedAt FROM Tasks " +
                               "WHERE Status = 'Pending' ORDER BY CreatedAt DESC";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Update Task Status (Complete/Delete)
        public void UpdateTaskStatus(int taskId, string status)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE Tasks SET Status = @status WHERE Id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete Task Permanently
        public void DeleteTask(int taskId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Tasks WHERE Id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get Tasks with Reminders Due Soon
        public DataTable GetTasksWithReminders()
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Title, Description, ReminderDate FROM Tasks " +
                               "WHERE Status = 'Pending' AND ReminderDate IS NOT NULL " +
                               "AND ReminderDate <= DATE_ADD(NOW(), INTERVAL 3 DAY)";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }
}