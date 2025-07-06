# 💼 Job Application Portal

A **role-based web application** built with **ASP.NET Core MVC** that simplifies job postings and application submissions. Users can register, apply for jobs, upload resumes, and manage their applications, while Admin and HR users can manage jobs and applicants.

---

## 🚀 Technologies Used

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **ASP.NET Identity**
- **Bootstrap 5**
- **LINQ**
- **Razor Views**
- **Dependency Injection**

---

## 🧠 Features

### 🔑 Role-Based Access

- **Administrator**

  - Add/Edit/Delete job postings.
  - View all applicants and their uploaded resumes.
  - Full access to portal and user management.

- **HR (Human Resources)**

  - Manage job positions.
  - View applicants and review applications.

- **Applicant**
  - Register/login to apply for jobs.
  - Upload resume files (PDF, DOCX, PNG, JPG).
  - View, edit, or delete submitted applications.

### 📁 Resume Upload Support

- File types supported: `.pdf`, `.docx`, `.jpg`, `.png`.

---

## 🛠️ Setup Instructions

### 🔧 Backend Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/Hamza-Zahoor157/Job-Application-Portal.git
   cd JobApplicationPortal
   ```

2. Open the project in **Visual Studio 2022+**.

3. Update the **connection string** in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=JobPortalDB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

4. Apply migrations and update the database:

   ```bash
   PM> Add-Migration InitialCreate
   PM> Update-Database
   ```

5. Run the application (`F5` or `Ctrl+F5`).

---

## 👥 Sample Login Credentials

| Role          | Email             | Password  |
| ------------- | ----------------- | --------- |
| **Admin**     | hamid@gmail.com   | 1234ABcd@ |
| **HR**        | hr@gmail.com      | 1234ABcd@ |
| **Applicant** | suleman@gmail.com | 1234ABcd@ |

---

## 🧭 Walkthrough & Screenshots
` In User Manual PDF `
### 🖥️ Landing / Login Page

### 📝 Registration Page

### 📊 Dashboard (Admin / HR)

### ➕ Add Job Position

### 🛠️ Edit Job Position

### ❌ Delete Job Position

### 👁️ View Job Details

### 👨‍💻 Applicant Dashboard

### 📄 Apply for Job

### ✏️ Edit Application

### 🧾 View Resume

### ❌ Delete Application

---




## 📌 Notes

- Resume files are stored in `wwwroot/Resumes`.
- Ensure write permissions for the folder if hosted on a server.
- Identity uses a custom user model with extended fields (FullName, City).

---

## 🧑‍💻 Developed by

**Hamza Zahoor**  
Job Application Portal Project
