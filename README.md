# 🔐 Cybersecurity Awareness Chatbot - POE Part 3

## 📋 Project Overview
A fully-featured Cybersecurity Awareness Chatbot built with C# Windows Forms, MySQL database integration, and advanced features including task management, cybersecurity quiz, NLP simulation, and activity logging.

## 🚀 Features

### ✅ Task 1: Task Assistant with Reminders
- Add, view, complete, and delete cybersecurity tasks
- MySQL database integration for persistent storage
- Set reminders with date/time picker
- View tasks in an interactive DataGridView
- Right-click context menu for task actions

### ✅ Task 2: Cybersecurity Mini-Game (Quiz)
- 12 cybersecurity questions covering:
  - Password safety
  - Phishing awareness
  - Online privacy
  - Malware protection
- Multiple-choice and True/False questions
- Immediate feedback with explanations
- Score tracking with personalized results

### ✅ Task 3: Natural Language Processing (NLP) Simulation
- Keyword detection using string manipulation
- Understands variations like:
  - "add task" / "add a task"
  - "remind me to" / "remind me about"
  - "complete task" / "finish task"
- Extracts tasks and reminders from natural language

### ✅ Task 4: Activity Log Feature
- Logs all user actions:
  - Tasks added/completed/deleted
  - Reminders set
  - Quiz attempts
  - User interactions
- View last 10 actions via command or button
- Timestamped entries for accountability

## 🛠️ Technologies Used

- **C#** - Windows Forms Application
- **MySQL** - Database for task storage
- **MySQL Connector/NET** - Database connectivity
- **.NET Framework** - Windows Forms
- **Git** - Version control

## 📦 Database Setup

```sql
CREATE DATABASE CyberSecurityBot;
USE CyberSecurityBot;

CREATE TABLE Tasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    ReminderDate DATETIME,
    Status VARCHAR(50) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
