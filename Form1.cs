using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberSecurityBotGUI
{
    public partial class Form1 : Form
    {
        private ChatbotEngine bot = new ChatbotEngine();

        // GUI Controls
        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;
        private Button clearButton;
        private Label titleLabel;

        // Task Panel Controls
        private Panel taskPanel;
        private TextBox taskTitleBox;
        private TextBox taskDescBox;
        private DateTimePicker reminderPicker;
        private Button addTaskButton;
        private Button viewTasksButton;
        private DataGridView taskGridView;
        private Label taskStatusLabel;

        // Quiz Panel Controls
        private Panel quizPanel;
        private Button startQuizButton;
        private Label quizStatusLabel;

        // Activity Log Panel
        private Button showLogButton;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            PlayGreeting();
            LoadTasksFromDB();
        }

        private void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.Play();
            }
            catch
            {
                // Silent fail if file missing
            }
        }

        private void BuildInterface()
        {
            // Form settings
            this.Text = "🔐 Cybersecurity Awareness Bot - Full POE";
            this.Size = new Size(1100, 750);
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ========== TITLE ==========
            titleLabel = new Label();
            titleLabel.Text = "🔐 CYBERSECURITY AWARENESS BOT - POE PART 3";
            titleLabel.ForeColor = Color.Cyan;
            titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(250, 15);

            // ========== CHAT BOX ==========
            chatBox = new RichTextBox();
            chatBox.Location = new Point(20, 70);
            chatBox.Size = new Size(500, 400);
            chatBox.BackColor = Color.White;
            chatBox.ForeColor = Color.Black;
            chatBox.ReadOnly = true;
            chatBox.Font = new Font("Arial", 11);

            // ========== INPUT BOX ==========
            inputBox = new TextBox();
            inputBox.Location = new Point(20, 485);
            inputBox.Size = new Size(380, 30);
            inputBox.Font = new Font("Arial", 11);
            inputBox.KeyDown += InputBox_KeyDown;

            // ========== SEND BUTTON ==========
            sendButton = new Button();
            sendButton.Text = "📤 Send";
            sendButton.Location = new Point(410, 480);
            sendButton.Size = new Size(110, 40);
            sendButton.BackColor = Color.Cyan;
            sendButton.Font = new Font("Arial", 11, FontStyle.Bold);
            sendButton.Click += SendButton_Click;

            // ========== CLEAR BUTTON ==========
            clearButton = new Button();
            clearButton.Text = "🗑️ Clear Chat";
            clearButton.Location = new Point(20, 530);
            clearButton.Size = new Size(110, 35);
            clearButton.BackColor = Color.LightGray;
            clearButton.Click += ClearButton_Click;

            // ========== TASK PANEL ==========
            taskPanel = new Panel();
            taskPanel.Location = new Point(540, 70);
            taskPanel.Size = new Size(530, 510);
            taskPanel.BackColor = Color.FromArgb(20, 20, 30);
            taskPanel.BorderStyle = BorderStyle.FixedSingle;

            Label taskTitle = new Label();
            taskTitle.Text = "📋 TASK MANAGER";
            taskTitle.ForeColor = Color.Cyan;
            taskTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            taskTitle.Location = new Point(10, 10);
            taskTitle.Size = new Size(300, 30);

            Label titleLbl = new Label();
            titleLbl.Text = "Task Title:";
            titleLbl.ForeColor = Color.White;
            titleLbl.Font = new Font("Arial", 10);
            titleLbl.Location = new Point(10, 50);
            titleLbl.Size = new Size(80, 25);

            taskTitleBox = new TextBox();
            taskTitleBox.Location = new Point(95, 48);
            taskTitleBox.Size = new Size(420, 25);
            taskTitleBox.Font = new Font("Arial", 10);

            Label descLbl = new Label();
            descLbl.Text = "Description:";
            descLbl.ForeColor = Color.White;
            descLbl.Font = new Font("Arial", 10);
            descLbl.Location = new Point(10, 85);
            descLbl.Size = new Size(80, 25);

            taskDescBox = new TextBox();
            taskDescBox.Location = new Point(95, 83);
            taskDescBox.Size = new Size(420, 25);
            taskDescBox.Font = new Font("Arial", 10);

            Label reminderLbl = new Label();
            reminderLbl.Text = "Reminder:";
            reminderLbl.ForeColor = Color.White;
            reminderLbl.Font = new Font("Arial", 10);
            reminderLbl.Location = new Point(10, 120);
            reminderLbl.Size = new Size(80, 25);

            reminderPicker = new DateTimePicker();
            reminderPicker.Location = new Point(95, 118);
            reminderPicker.Size = new Size(200, 25);
            reminderPicker.Font = new Font("Arial", 10);
            reminderPicker.Format = DateTimePickerFormat.Custom;
            reminderPicker.CustomFormat = "dd/MM/yyyy HH:mm";
            reminderPicker.ShowUpDown = true;

            addTaskButton = new Button();
            addTaskButton.Text = "➕ Add Task";
            addTaskButton.Location = new Point(310, 118);
            addTaskButton.Size = new Size(100, 25);
            addTaskButton.BackColor = Color.LightGreen;
            addTaskButton.Font = new Font("Arial", 9, FontStyle.Bold);
            addTaskButton.Click += AddTaskButton_Click;

            viewTasksButton = new Button();
            viewTasksButton.Text = "🔄 Refresh Tasks";
            viewTasksButton.Location = new Point(420, 118);
            viewTasksButton.Size = new Size(95, 25);
            viewTasksButton.BackColor = Color.LightBlue;
            viewTasksButton.Font = new Font("Arial", 9, FontStyle.Bold);
            viewTasksButton.Click += ViewTasksButton_Click;

            taskStatusLabel = new Label();
            taskStatusLabel.Text = "Pending Tasks: 0";
            taskStatusLabel.ForeColor = Color.White;
            taskStatusLabel.Font = new Font("Arial", 10);
            taskStatusLabel.Location = new Point(10, 155);
            taskStatusLabel.Size = new Size(200, 25);

            // Task Grid View
            taskGridView = new DataGridView();
            taskGridView.Location = new Point(10, 185);
            taskGridView.Size = new Size(505, 310);
            taskGridView.BackgroundColor = Color.White;
            taskGridView.Font = new Font("Arial", 9);
            taskGridView.AllowUserToAddRows = false;
            taskGridView.AllowUserToDeleteRows = false;
            taskGridView.ReadOnly = true;
            taskGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            taskGridView.RowHeadersVisible = false;
            taskGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Add context menu for tasks
            ContextMenuStrip taskMenu = new ContextMenuStrip();
            ToolStripMenuItem completeItem = new ToolStripMenuItem("Mark Complete");
            completeItem.Click += CompleteTaskFromGrid_Click;
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete Task");
            deleteItem.Click += DeleteTaskFromGrid_Click;
            taskMenu.Items.Add(completeItem);
            taskMenu.Items.Add(deleteItem);
            taskGridView.ContextMenuStrip = taskMenu;

            taskPanel.Controls.AddRange(new Control[] {
                taskTitle, titleLbl, taskTitleBox, descLbl, taskDescBox,
                reminderLbl, reminderPicker, addTaskButton, viewTasksButton,
                taskStatusLabel, taskGridView
            });

            // ========== SHOW LOG BUTTON ==========
            showLogButton = new Button();
            showLogButton.Text = "📜 Show Activity Log";
            showLogButton.Location = new Point(140, 530);
            showLogButton.Size = new Size(150, 35);
            showLogButton.BackColor = Color.LightYellow;
            showLogButton.Font = new Font("Arial", 10, FontStyle.Bold);
            showLogButton.Click += ShowLogButton_Click;

            // ========== QUIZ BUTTON ==========
            startQuizButton = new Button();
            startQuizButton.Text = "🎯 Start Cybersecurity Quiz";
            startQuizButton.Location = new Point(300, 530);
            startQuizButton.Size = new Size(220, 35);
            startQuizButton.BackColor = Color.Orange;
            startQuizButton.Font = new Font("Arial", 10, FontStyle.Bold);
            startQuizButton.Click += StartQuizButton_Click;

            quizStatusLabel = new Label();
            quizStatusLabel.Text = "";
            quizStatusLabel.ForeColor = Color.LightGreen;
            quizStatusLabel.Font = new Font("Arial", 10);
            quizStatusLabel.Location = new Point(300, 570);
            quizStatusLabel.Size = new Size(220, 25);

            // ========== ADD CONTROLS ==========
            this.Controls.AddRange(new Control[] {
                titleLabel,
                chatBox, inputBox, sendButton, clearButton,
                taskPanel, showLogButton, startQuizButton, quizStatusLabel
            });

            // Welcome message
            ShowWelcomeMessage();
        }

        private void ShowWelcomeMessage()
        {
            chatBox.AppendText(
@"======================================================
|| 🔐 CYBERSECURITY AWARENESS BOT - POE PART 3 ||
======================================================

[SECURITY ACTIVE ✅]

Welcome! I'm your cybersecurity assistant with new features:

📋 TASK MANAGER - Add, view, complete, and delete tasks
🎯 CYBERSECURITY QUIZ - Test your knowledge (12 questions)
📜 ACTIVITY LOG - See what I've done for you
🧠 NLP ENHANCEMENTS - Understands different phrasings

======================================================
");

            chatBox.AppendText("Bot: Hello! Ask me about:\n");
            chatBox.AppendText("• Password safety\n");
            chatBox.AppendText("• Phishing scams\n");
            chatBox.AppendText("• Online privacy\n");
            chatBox.AppendText("• Malware protection\n\n");
            chatBox.AppendText("Quick commands:\n");
            chatBox.AppendText("• 'add task [title]' or 'remind me to [task]'\n");
            chatBox.AppendText("• 'view tasks' - See all pending tasks\n");
            chatBox.AppendText("• 'quiz' - Start the cybersecurity quiz\n");
            chatBox.AppendText("• 'activity log' - Show recent actions\n");
            chatBox.AppendText("• 'complete task [title]' - Mark complete\n");
            chatBox.AppendText("• 'delete task [title]' - Remove task\n\n");
        }

        private void LoadTasksFromDB()
        {
            try
            {
                DataTable dt = new DatabaseHelper().GetPendingTasks();
                taskGridView.DataSource = dt;

                // Format columns
                if (taskGridView.Columns.Count > 0)
                {
                    if (taskGridView.Columns["Id"] != null)
                        taskGridView.Columns["Id"].Visible = false;

                    if (taskGridView.Columns["Title"] != null)
                        taskGridView.Columns["Title"].HeaderText = "Title";

                    if (taskGridView.Columns["Description"] != null)
                        taskGridView.Columns["Description"].HeaderText = "Description";

                    if (taskGridView.Columns["ReminderDate"] != null)
                        taskGridView.Columns["ReminderDate"].HeaderText = "Reminder";

                    if (taskGridView.Columns["CreatedAt"] != null)
                        taskGridView.Columns["CreatedAt"].HeaderText = "Created";
                }

                int count = dt.Rows.Count;
                taskStatusLabel.Text = $"📋 Pending Tasks: {count}";
            }
            catch (Exception ex)
            {
                taskStatusLabel.Text = "⚠️ DB Error: " + ex.Message;
            }
        }

        // ========== EVENT HANDLERS ==========

        private async void SendButton_Click(object sender, EventArgs e)
        {
            string userInput = inputBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please enter a message.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Display user message
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] You: {userInput}\n");

            // Show typing indicator
            chatBox.AppendText("🤖 Bot is thinking...\n");
            await Task.Delay(500);

            // Remove typing indicator
            int lastIndex = chatBox.Text.LastIndexOf("🤖 Bot is thinking...\n");
            if (lastIndex >= 0)
            {
                chatBox.Text = chatBox.Text.Remove(lastIndex, "🤖 Bot is thinking...\n".Length);
            }

            // Get response
            string response = bot.GetResponse(userInput);

            // Display response
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] 🤖 Bot: {response}\n\n");

            // Clear input
            inputBox.Clear();

            // Scroll to bottom
            chatBox.SelectionStart = chatBox.Text.Length;
            chatBox.ScrollToCaret();

            // Update task list if needed
            if (userInput.Contains("add") || userInput.Contains("view") || userInput.Contains("complete") || userInput.Contains("delete"))
            {
                LoadTasksFromDB();
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendButton.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            chatBox.Clear();
            ShowWelcomeMessage();
        }

        private void AddTaskButton_Click(object sender, EventArgs e)
        {
            string title = taskTitleBox.Text.Trim();
            string desc = taskDescBox.Text.Trim();
            DateTime reminder = reminderPicker.Value;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Send to bot
            string command = $"add task {title}";
            if (!string.IsNullOrWhiteSpace(desc))
            {
                // Description is handled through the bot
            }

            // Add with reminder
            DatabaseHelper db = new DatabaseHelper();
            db.AddTask(title, desc, reminder);

            chatBox.AppendText($"[{DateTime.Now:HH:mm}] You: [Added task via Task Panel]\n");
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] 🤖 Bot: ✅ Task added: {title}\n");
            chatBox.AppendText($"   📝 Description: {(string.IsNullOrWhiteSpace(desc) ? "N/A" : desc)}\n");
            chatBox.AppendText($"   ⏰ Reminder: {reminder:dd/MM/yyyy HH:mm}\n\n");

            LoadTasksFromDB();
            taskTitleBox.Clear();
            taskDescBox.Clear();

            MessageBox.Show($"Task '{title}' added successfully!", "Task Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ViewTasksButton_Click(object sender, EventArgs e)
        {
            LoadTasksFromDB();
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] You: [Refreshed task list]\n");
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] 🤖 Bot: 📋 Task list refreshed.\n\n");
        }

        private void CompleteTaskFromGrid_Click(object sender, EventArgs e)
        {
            if (taskGridView.SelectedRows.Count > 0)
            {
                string title = taskGridView.SelectedRows[0].Cells["Title"].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    DatabaseHelper db = new DatabaseHelper();
                    DataTable dt = db.GetAllTasks();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Title"].ToString() == title)
                        {
                            int id = Convert.ToInt32(row["Id"]);
                            db.UpdateTaskStatus(id, "Completed");
                            break;
                        }
                    }
                    LoadTasksFromDB();
                    chatBox.AppendText($"[{DateTime.Now:HH:mm}] ✅ Task completed: {title}\n\n");
                }
            }
        }

        private void DeleteTaskFromGrid_Click(object sender, EventArgs e)
        {
            if (taskGridView.SelectedRows.Count > 0)
            {
                string title = taskGridView.SelectedRows[0].Cells["Title"].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    DialogResult result = MessageBox.Show($"Delete task '{title}'?", "Confirm Delete",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        DatabaseHelper db = new DatabaseHelper();
                        DataTable dt = db.GetAllTasks();
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["Title"].ToString() == title)
                            {
                                int id = Convert.ToInt32(row["Id"]);
                                db.DeleteTask(id);
                                break;
                            }
                        }
                        LoadTasksFromDB();
                        chatBox.AppendText($"[{DateTime.Now:HH:mm}] 🗑️ Task deleted: {title}\n\n");
                    }
                }
            }
        }

        private void ShowLogButton_Click(object sender, EventArgs e)
        {
            string log = bot.GetActivityLog();

            // Show in chat
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] You: [Requested activity log]\n");
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] 🤖 Bot:\n{log}\n\n");

            // Also show in message box for easier reading
            MessageBox.Show(log, "📋 Activity Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StartQuizButton_Click(object sender, EventArgs e)
        {
            string response = bot.GetResponse("quiz");
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] You: [Started quiz]\n");
            chatBox.AppendText($"[{DateTime.Now:HH:mm}] 🤖 Bot:\n{response}\n\n");
            quizStatusLabel.Text = "🎯 Quiz in progress...";
        }
    }
}