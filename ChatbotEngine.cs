using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CyberSecurityBotGUI
{
    public class ChatbotEngine
    {
        private Random random = new Random();
        private DatabaseHelper db = new DatabaseHelper();
        private string userName = "";
        private string lastTopic = "";

        // Task Manager (in-memory cache)
        private List<string> tasks = new List<string>();
        private List<string> completedTasks = new List<string>();

        // Memory system
        private List<string> memory = new List<string>();

        // Activity Log
        private List<string> activityLog = new List<string>();
        private const int MAX_LOG_ENTRIES = 10;

        // Quiz System
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private int currentQuestionIndex = 0;
        private int quizScore = 0;
        private bool quizActive = false;
        private List<int> answeredQuestions = new List<int>();

        // Response lists
        public List<string> passwordResponses = new List<string>()
        {
            "Use passwords with 12+ characters.",
            "Avoid using personal information in passwords.",
            "Use different passwords for different accounts.",
            "Enable two-factor authentication whenever possible."
        };

        public List<string> phishingResponses = new List<string>()
        {
            "Never click suspicious links.",
            "Verify email senders before responding.",
            "Scammers often pretend to be trusted organisations.",
            "Avoid emails asking for urgent payments."
        };

        public List<string> privacyResponses = new List<string>()
        {
            "Review your privacy settings regularly.",
            "Avoid oversharing personal information online.",
            "Use secure websites with HTTPS.",
            "Be careful when downloading unknown files."
        };

        public List<string> malwareResponses = new List<string>()
        {
            "Install trusted antivirus software.",
            "Avoid downloading files from unknown websites.",
            "Keep your operating system updated.",
            "Malware can steal personal information from your device."
        };

        private Dictionary<string, List<string>> keywordResponses;

        // Constructor
        public ChatbotEngine()
        {
            keywordResponses = new Dictionary<string, List<string>>()
            {
                { "password", passwordResponses },
                { "phishing", phishingResponses },
                { "privacy", privacyResponses },
                { "malware", malwareResponses },
                { "virus", malwareResponses },
                { "hacker", malwareResponses }
            };

            // Initialize quiz questions
            InitializeQuizQuestions();

            // Initialize database
            db.InitializeDatabase();

            // Load tasks from DB
            LoadTasksFromDB();

            // Log initialization
            AddActivityLog("Chatbot initialized successfully.");
        }

        // Quiz Question Class
        public class QuizQuestion
        {
            public string Question { get; set; } = string.Empty;
            public List<string> Options { get; set; } = new List<string>();
            public int CorrectAnswer { get; set; }
            public string Explanation { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
        }

        // Initialize Quiz Questions (12 questions)
        private void InitializeQuizQuestions()
        {
            quizQuestions = new List<QuizQuestion>
            {
                // Password Safety
                new QuizQuestion
                {
                    Question = "What is the best practice for creating a strong password?",
                    Options = new List<string> { "Use your birthday", "Use 12+ characters with symbols and numbers", "Use the same password everywhere", "Use 'password123'" },
                    CorrectAnswer = 1,
                    Explanation = "Strong passwords should be at least 12 characters and include uppercase, lowercase, numbers, and special characters.",
                    Category = "Password Safety"
                },
                new QuizQuestion
                {
                    Question = "True or False: You should use the same password for multiple accounts.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Using the same password for multiple accounts is risky. If one account is compromised, all are at risk.",
                    Category = "Password Safety"
                },
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string> { "A second password", "A security question", "An additional verification step", "A fingerprint scanner" },
                    CorrectAnswer = 2,
                    Explanation = "2FA adds an extra layer of security by requiring a second form of verification, like a code sent to your phone.",
                    Category = "Password Safety"
                },

                // Phishing
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report it as phishing", "Ignore it" },
                    CorrectAnswer = 2,
                    Explanation = "Report phishing emails to help prevent scams and protect others from falling victim.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    Question = "True or False: Phishing emails always come from unknown senders.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Phishing emails can appear to come from trusted sources through email spoofing techniques.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Options = new List<string> { "Personalized greeting", "Urgent action required", "Correct spelling", "Known sender" },
                    CorrectAnswer = 1,
                    Explanation = "Phishing emails often create urgency to pressure you into acting without thinking.",
                    Category = "Phishing"
                },

                // Privacy
                new QuizQuestion
                {
                    Question = "What does HTTPS indicate on a website?",
                    Options = new List<string> { "The site is fast", "The site is secure", "The site is popular", "The site is old" },
                    CorrectAnswer = 1,
                    Explanation = "HTTPS indicates that data between your browser and the website is encrypted and secure.",
                    Category = "Privacy"
                },
                new QuizQuestion
                {
                    Question = "True or False: You should share your location on social media in real-time.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Sharing real-time location can compromise your physical safety and privacy.",
                    Category = "Privacy"
                },
                new QuizQuestion
                {
                    Question = "What should you check before downloading a file from the internet?",
                    Options = new List<string> { "File size", "File source", "Download speed", "File name" },
                    CorrectAnswer = 1,
                    Explanation = "Always verify the source of files to avoid malware and malicious downloads.",
                    Category = "Privacy"
                },

                // Malware
                new QuizQuestion
                {
                    Question = "What is malware?",
                    Options = new List<string> { "A type of hardware", "Malicious software", "A security feature", "A browser extension" },
                    CorrectAnswer = 1,
                    Explanation = "Malware is malicious software designed to damage or gain unauthorized access to systems.",
                    Category = "Malware"
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus software can protect against all types of cyber threats.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "While antivirus software is important, it's not 100% effective. Multiple layers of security are recommended.",
                    Category = "Malware"
                },
                new QuizQuestion
                {
                    Question = "What should you do if your computer shows signs of malware infection?",
                    Options = new List<string> { "Ignore it", "Run a full antivirus scan", "Delete files randomly", "Restart the computer" },
                    CorrectAnswer = 1,
                    Explanation = "Running a full antivirus scan can help detect and remove malware from your system.",
                    Category = "Malware"
                }
            };

            // Shuffle questions for variety
            quizQuestions = quizQuestions.OrderBy(x => random.Next()).ToList();
        }

        // Load tasks from database
        private void LoadTasksFromDB()
        {
            tasks.Clear();
            DataTable dt = db.GetPendingTasks();
            foreach (DataRow row in dt.Rows)
            {
                tasks.Add(row["Title"].ToString());
            }
        }

        // Add activity log
        private void AddActivityLog(string action)
        {
            string timestampedAction = $"[{DateTime.Now:HH:mm:ss}] {action}";
            activityLog.Insert(0, timestampedAction);

            if (activityLog.Count > MAX_LOG_ENTRIES)
            {
                activityLog.RemoveAt(activityLog.Count - 1);
            }
        }

        // Get Activity Log
        public string GetActivityLog()
        {
            if (activityLog.Count == 0)
            {
                return "No activity recorded yet.";
            }

            string log = "📋 RECENT ACTIVITY LOG:\n";
            log += "=================================\n";
            for (int i = 0; i < Math.Min(activityLog.Count, 10); i++)
            {
                log += $"{i + 1}. {activityLog[i]}\n";
            }
            return log;
        }

        // Main chatbot response method
        public string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            // ========== TASK 4: Activity Log ==========
            if (input.Contains("activity log") || input.Contains("show log") || input.Contains("what have you done"))
            {
                return GetActivityLog();
            }

            // ========== TASK 3: NLP Enhancements ==========
            // Handle task variations
            if (input.Contains("add") && (input.Contains("task") || input.Contains("reminder")))
            {
                return HandleAddTaskNLP(input);
            }

            if (input.Contains("remind me to") || input.Contains("remind me about"))
            {
                return HandleReminderNLP(input);
            }

            if ((input.Contains("view") || input.Contains("show") || input.Contains("list")) && input.Contains("task"))
            {
                return ViewTasks();
            }

            if (input.Contains("complete") && input.Contains("task"))
            {
                return HandleCompleteTaskNLP(input);
            }

            if (input.Contains("delete") && input.Contains("task"))
            {
                return HandleDeleteTaskNLP(input);
            }

            // ========== TASK 2: Quiz System ==========
            if (input.Contains("quiz") || input.Contains("game") || input.Contains("play"))
            {
                return StartQuiz();
            }

            if (quizActive)
            {
                return HandleQuizAnswer(input);
            }

            // ========== TASK 1: Task Management ==========
            if (input.StartsWith("add task "))
            {
                return AddTask(input.Replace("add task ", "").Trim(), "", null);
            }

            else if (input == "view tasks")
            {
                return ViewTasks();
            }

            else if (input.StartsWith("complete task "))
            {
                return CompleteTask(input.Replace("complete task ", "").Trim());
            }

            else if (input.StartsWith("delete task "))
            {
                return DeleteTask(input.Replace("delete task ", "").Trim());
            }

            // ========== Original Chatbot Features ==========
            else if (input.StartsWith("my name is"))
            {
                userName = input.Replace("my name is", "").Trim();
                AddActivityLog($"User introduced themselves as: {userName}");
                return "Nice to meet you, " + userName + "! I will remember your name.";
            }

            else if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey"))
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    return "Hello again, " + userName + "! How can I help you today?";
                }
                return "Hello! How can I help you stay cyber safe today?";
            }

            else if (input.Contains("password"))
            {
                if (!memory.Contains("password safety"))
                {
                    memory.Add("password safety");
                }
                lastTopic = "password";
                List<string> responses = keywordResponses["password"];
                AddActivityLog("Discussed password safety tips");
                return responses[random.Next(responses.Count)] + "\n\nWould you like to take a quiz on password safety? Type 'quiz' to start!";
            }

            else if (input.Contains("phishing") || input.Contains("scam") || input.Contains("fraud"))
            {
                if (!memory.Contains("phishing awareness"))
                {
                    memory.Add("phishing awareness");
                }
                lastTopic = "phishing";
                List<string> responses = keywordResponses["phishing"];
                AddActivityLog("Discussed phishing awareness");
                return responses[random.Next(responses.Count)] + "\n\nWould you like to test your knowledge with a quiz? Type 'quiz' to start!";
            }

            else if (input.Contains("privacy") || input.Contains("safe browsing"))
            {
                if (!memory.Contains("online privacy"))
                {
                    memory.Add("online privacy");
                }
                lastTopic = "privacy";
                List<string> responses = keywordResponses["privacy"];
                AddActivityLog("Discussed online privacy");
                return responses[random.Next(responses.Count)];
            }

            else if (input.Contains("malware") || input.Contains("virus") || input.Contains("hacker") || input.Contains("trojan"))
            {
                if (!memory.Contains("malware protection"))
                {
                    memory.Add("malware protection");
                }
                lastTopic = "malware";
                List<string> responses = keywordResponses["malware"];
                AddActivityLog("Discussed malware protection");
                return responses[random.Next(responses.Count)];
            }

            else if (input.Contains("tell me more") || input.Contains("another tip") || input.Contains("more tips"))
            {
                List<string> extraTips = new List<string>()
                {
                    "Always log out from shared computers.",
                    "Do not reuse passwords across websites.",
                    "Avoid opening suspicious email attachments.",
                    "Use multi-factor authentication whenever possible.",
                    "Regularly back up your important data.",
                    "Keep all software and apps updated.",
                    "Be careful when using public Wi-Fi networks.",
                    "Review your bank statements regularly for unauthorized transactions."
                };
                AddActivityLog("Provided additional cybersecurity tips");
                return extraTips[random.Next(extraTips.Count)];
            }

            else if (input.Contains("what did we talk about") || input.Contains("previous topics"))
            {
                if (memory.Count > 0)
                {
                    return "Earlier we discussed: " + string.Join(", ", memory) + "\n\nType 'quiz' to test your knowledge on these topics!";
                }
                return "We have not discussed any cybersecurity topics yet.";
            }

            else if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious"))
            {
                return "It is okay to feel concerned. Staying informed and cautious online helps keep you safe. Would you like some tips to feel more secure?";
            }

            else if (input.Contains("confused") || input.Contains("don't understand"))
            {
                return "Cybersecurity can seem difficult at first, but I am here to help you understand it. What specific topic are you struggling with?";
            }

            else if (input.Contains("curious") || input.Contains("interesting") || input.Contains("want to learn"))
            {
                return "That is great! Learning more about cybersecurity is an excellent habit. Let me know what topic interests you!";
            }

            else if (input.Contains("purpose") || input.Contains("what do you do"))
            {
                return "My purpose is to educate users about cybersecurity awareness and online safety. I can help with passwords, phishing, privacy, and more!";
            }

            else if (input.Contains("tips") || input.Contains("advice") || input.Contains("help"))
            {
                List<string> tips = new List<string>()
                {
                    "Never share your passwords with anyone.",
                    "Always update your software regularly.",
                    "Avoid public Wi-Fi for sensitive transactions.",
                    "Enable two-factor authentication on important accounts.",
                    "Be careful when downloading attachments from emails.",
                    "Use a password manager to generate and store strong passwords."
                };
                AddActivityLog("Provided general cybersecurity tips");
                return tips[random.Next(tips.Count)];
            }

            else if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit") || input.Contains("quit"))
            {
                AddActivityLog("User ended conversation");
                return "Goodbye! Stay safe online and remember to practice good cybersecurity habits! 👋";
            }

            else
            {
                return "I didn't quite understand that. Could you rephrase?\n\nYou can ask me about:\n- Password safety\n- Phishing scams\n- Online privacy\n- Malware protection\n- Cybersecurity quiz\n- Task management\n- Activity log";
            }
        }

        // ========== TASK 1: Task Management Methods ==========

        private string AddTask(string title, string description, DateTime? reminderDate)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return "Please provide a task title.";
            }

            db.AddTask(title, description, reminderDate);
            tasks.Add(title);

            string reminderText = reminderDate.HasValue ? $" with a reminder on {reminderDate.Value:dd/MM/yyyy}" : " (no reminder set)";
            AddActivityLog($"Added task: {title}{reminderText}");

            LoadTasksFromDB(); // Refresh task list
            return $"✅ Task added successfully: {title}{reminderText}";
        }

        private string ViewTasks()
        {
            LoadTasksFromDB(); // Always refresh from DB

            if (tasks.Count == 0)
            {
                return "📋 No pending tasks. Add a task with 'add task [title]' or 'remind me to [task]'";
            }

            string result = "📋 YOUR PENDING TASKS:\n";
            result += "=================================\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                result += $"{i + 1}. {tasks[i]}\n";
            }
            result += $"\nTotal: {tasks.Count} pending tasks";
            result += "\nType 'complete task [title]' or 'delete task [title]' to manage them.";

            return result;
        }

        private string CompleteTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return "Please specify which task to complete.";
            }

            // Find task in database
            DataTable dt = db.GetAllTasks();
            int taskId = -1;
            foreach (DataRow row in dt.Rows)
            {
                if (row["Title"].ToString().ToLower() == title.ToLower())
                {
                    taskId = Convert.ToInt32(row["Id"]);
                    break;
                }
            }

            if (taskId != -1)
            {
                db.UpdateTaskStatus(taskId, "Completed");
                tasks.RemoveAll(t => t.ToLower() == title.ToLower());
                AddActivityLog($"Completed task: {title}");
                LoadTasksFromDB();
                return $"✅ Task marked as completed: {title}";
            }

            return $"❌ Task '{title}' not found.";
        }

        private string DeleteTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return "Please specify which task to delete.";
            }

            // Find task in database
            DataTable dt = db.GetAllTasks();
            int taskId = -1;
            foreach (DataRow row in dt.Rows)
            {
                if (row["Title"].ToString().ToLower() == title.ToLower())
                {
                    taskId = Convert.ToInt32(row["Id"]);
                    break;
                }
            }

            if (taskId != -1)
            {
                db.DeleteTask(taskId);
                tasks.RemoveAll(t => t.ToLower() == title.ToLower());
                AddActivityLog($"Deleted task: {title}");
                LoadTasksFromDB();
                return $"🗑️ Task deleted: {title}";
            }

            return $"❌ Task '{title}' not found.";
        }

        // ========== TASK 3: NLP Methods ==========

        private string HandleAddTaskNLP(string input)
        {
            // Extract task title
            string taskTitle = "";
            string[] patterns = { "add task ", "add a task ", "add reminder " };

            foreach (string pattern in patterns)
            {
                if (input.Contains(pattern))
                {
                    taskTitle = input.Substring(input.IndexOf(pattern) + pattern.Length).Trim();
                    break;
                }
            }

            // Clean up extra words
            string[] stopWords = { "to", "for", "about", "please" };
            foreach (string word in stopWords)
            {
                taskTitle = taskTitle.Replace(" " + word + " ", " ");
            }

            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                return "What task would you like to add? Please specify the task title.";
            }

            // Check for reminder in same message
            if (input.Contains("remind me in") || input.Contains("reminder"))
            {
                return HandleReminderNLP(input);
            }

            return AddTask(taskTitle, "", null) + "\nWould you like to set a reminder? Type 'remind me to [task] in X days'";
        }

        private string HandleReminderNLP(string input)
        {
            string taskTitle = "";
            int days = 7; // Default

            // Extract task
            if (input.Contains("remind me to"))
            {
                taskTitle = input.Substring(input.IndexOf("remind me to") + "remind me to".Length).Trim();
            }
            else if (input.Contains("remind me about"))
            {
                taskTitle = input.Substring(input.IndexOf("remind me about") + "remind me about".Length).Trim();
            }
            else if (input.Contains("reminder"))
            {
                taskTitle = input.Substring(input.IndexOf("reminder") + "reminder".Length).Trim();
                if (taskTitle.StartsWith("to")) taskTitle = taskTitle.Substring(2).Trim();
                if (taskTitle.StartsWith("for")) taskTitle = taskTitle.Substring(3).Trim();
            }

            // Extract days from input
            if (input.Contains(" in ") && input.Contains(" day"))
            {
                string dayPart = input.Substring(input.IndexOf(" in ") + 4);
                if (dayPart.Contains(" day"))
                {
                    string dayNum = dayPart.Substring(0, dayPart.IndexOf(" day")).Trim();
                    if (int.TryParse(dayNum, out int parsedDays))
                    {
                        days = parsedDays;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                return "What would you like me to remind you about? Example: 'remind me to update my password in 3 days'";
            }

            // Clean up - remove reminder phrases
            string[] cleanWords = { "tomorrow", "today", "next week", "in", "days" };
            foreach (string word in cleanWords)
            {
                taskTitle = taskTitle.Replace(" " + word + " ", " ");
            }

            DateTime reminderDate = DateTime.Now.AddDays(days);
            string result = AddTask($"Reminder: {taskTitle}", $"Reminder set for {reminderDate:dd/MM/yyyy HH:mm}", reminderDate);

            AddActivityLog($"Set reminder for: {taskTitle} on {reminderDate:dd/MM/yyyy}");
            return result + $"\n⏰ I'll remind you on {reminderDate:dd/MM/yyyy} (in {days} days)";
        }

        private string HandleCompleteTaskNLP(string input)
        {
            string taskTitle = "";

            // Extract task from various phrasings
            if (input.Contains("complete task"))
            {
                taskTitle = input.Substring(input.IndexOf("complete task") + "complete task".Length).Trim();
            }
            else if (input.Contains("complete"))
            {
                taskTitle = input.Replace("complete", "").Replace("task", "").Trim();
            }
            else if (input.Contains("finish"))
            {
                taskTitle = input.Replace("finish", "").Replace("task", "").Trim();
            }

            // Remove stop words
            string[] stopWords = { "the", "my", "this" };
            foreach (string word in stopWords)
            {
                taskTitle = taskTitle.Replace(word, "").Trim();
            }

            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                return "Which task would you like to complete? Please specify the task title.";
            }

            return CompleteTask(taskTitle);
        }

        private string HandleDeleteTaskNLP(string input)
        {
            string taskTitle = "";

            if (input.Contains("delete task"))
            {
                taskTitle = input.Substring(input.IndexOf("delete task") + "delete task".Length).Trim();
            }
            else if (input.Contains("remove task"))
            {
                taskTitle = input.Substring(input.IndexOf("remove task") + "remove task".Length).Trim();
            }
            else if (input.Contains("delete"))
            {
                taskTitle = input.Replace("delete", "").Replace("task", "").Trim();
            }

            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                return "Which task would you like to delete? Please specify the task title.";
            }

            return DeleteTask(taskTitle);
        }

        // ========== TASK 2: Quiz Methods ==========

        private string StartQuiz()
        {
            quizQuestions = quizQuestions.OrderBy(x => random.Next()).ToList(); // Shuffle
            currentQuestionIndex = 0;
            quizScore = 0;
            quizActive = true;
            answeredQuestions.Clear();

            AddActivityLog("Started cybersecurity quiz");

            string intro = "🎯 CYBERSECURITY QUIZ STARTED!\n";
            intro += "=================================\n";
            intro += $"You'll answer {quizQuestions.Count} questions.\n";
            intro += "I'll provide feedback after each answer.\n";
            intro += "Good luck!\n\n";

            return intro + GetCurrentQuestion();
        }

        private string GetCurrentQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                return EndQuiz();
            }

            var q = quizQuestions[currentQuestionIndex];
            string questionText = $"Question {currentQuestionIndex + 1}/{quizQuestions.Count} [{q.Category}]\n";
            questionText += $"{q.Question}\n\n";

            for (int i = 0; i < q.Options.Count; i++)
            {
                string letter = (char)('A' + i) + "";
                questionText += $"{letter}) {q.Options[i]}\n";
            }

            questionText += "\nType the letter (A, B, C, D) or the full answer:";

            return questionText;
        }

        private string HandleQuizAnswer(string input)
        {
            input = input.ToUpper().Trim();

            if (input == "QUIT" || input == "EXIT" || input == "STOP")
            {
                quizActive = false;
                AddActivityLog($"Quit quiz with score: {quizScore}/{currentQuestionIndex}");
                return $"⏹️ Quiz ended. Your score: {quizScore}/{currentQuestionIndex}";
            }

            var q = quizQuestions[currentQuestionIndex];
            int selectedAnswer = -1;

            // Check if answer is a letter (A, B, C, D)
            if (input.Length == 1 && input[0] >= 'A' && input[0] <= 'D')
            {
                selectedAnswer = input[0] - 'A';
            }
            else
            {
                // Check if answer matches option text
                for (int i = 0; i < q.Options.Count; i++)
                {
                    if (q.Options[i].ToUpper() == input || q.Options[i].ToUpper().Contains(input))
                    {
                        selectedAnswer = i;
                        break;
                    }
                }
            }

            if (selectedAnswer == -1)
            {
                return $"Please enter a valid answer (A, B, C, or D) for question {currentQuestionIndex + 1}.\n\n{GetCurrentQuestion()}";
            }

            // Check if correct
            bool isCorrect = selectedAnswer == q.CorrectAnswer;
            if (isCorrect) quizScore++;

            string feedback = isCorrect ? "✅ CORRECT!" : $"❌ INCORRECT. The correct answer was: {q.Options[q.CorrectAnswer]}";
            feedback += $"\n📚 {q.Explanation}\n";

            currentQuestionIndex++;
            answeredQuestions.Add(currentQuestionIndex - 1);

            AddActivityLog($"Quiz Q{answeredQuestions.Count}: {(isCorrect ? "Correct" : "Incorrect")} - {q.Category}");

            if (currentQuestionIndex >= quizQuestions.Count)
            {
                return feedback + "\n\n" + EndQuiz();
            }

            return feedback + "\n\n" + GetCurrentQuestion();
        }

        private string EndQuiz()
        {
            quizActive = false;
            int totalQuestions = answeredQuestions.Count;
            double percentage = (double)quizScore / totalQuestions * 100;

            string result = "🎯 QUIZ COMPLETE!\n";
            result += "=================================\n";
            result += $"Score: {quizScore}/{totalQuestions}\n";
            result += $"Percentage: {percentage:F1}%\n\n";

            if (percentage >= 90)
            {
                result += "🌟 EXCELLENT! You're a cybersecurity pro! 🌟\n";
                result += "Your knowledge is impressive. Keep up the great work!";
            }
            else if (percentage >= 70)
            {
                result += "👏 GOOD JOB! You have solid cybersecurity knowledge.\n";
                result += "Review the topics you missed to become even better!";
            }
            else if (percentage >= 50)
            {
                result += "📚 NOT BAD! You're on the right track.\n";
                result += "Consider reviewing password safety, phishing, and privacy topics.";
            }
            else
            {
                result += "💡 KEEP LEARNING! Cybersecurity is important for everyone.\n";
                result += "Ask me about specific topics like passwords, phishing, or privacy.";
            }

            AddActivityLog($"Completed quiz. Score: {quizScore}/{totalQuestions} ({percentage:F1}%)");
            return result;
        }

        // Public properties for GUI
        public bool IsQuizActive => quizActive;

        // Get pending tasks count
        public int GetPendingTasksCount()
        {
            LoadTasksFromDB();
            return tasks.Count;
        }
    }
}