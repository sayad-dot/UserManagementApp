# User Management System

A professional .NET web application for user management with authentication, email verification, and administrative controls. Built with C#, ASP.NET Core, Entity Framework, and PostgreSQL.

## 🎯 Project Overview

This application provides a complete user management system where administrators can manage users through a clean, responsive interface. All users can perform administrative actions including blocking and deleting other users (including themselves).

## ✨ Key Features

- **User Authentication**: Secure JWT-based authentication system
- **Email Verification**: Asynchronous email verification with token-based confirmation
- **User Management Table**: Professional data table with sorting by last login time
- **Multi-Select Operations**: Checkbox-based selection for batch operations
- **Toolbar Actions**: 
  - Block selected users
  - Unblock selected users  
  - Delete selected users
  - Delete all unverified users
- **Middleware Protection**: Validates user status before each request
- **Responsive Design**: Bootstrap 5 for mobile and desktop compatibility
- **Database Integrity**: Unique index on email field ensures data consistency

## 🛠️ Technology Stack

- **.NET 8.0**: Latest LTS version of .NET
- **ASP.NET Core Web API**: RESTful API backend
- **Entity Framework Core**: ORM for database operations
- **PostgreSQL**: Robust relational database
- **JWT Authentication**: Secure token-based auth
- **Bootstrap 5**: Modern, responsive UI framework
- **MailKit**: Email sending functionality
- **BCrypt.NET**: Password hashing

## 📁 Project Structure

```
UserManagementApp/
├── UserManagementApp.API/          # Web API and frontend
│   ├── Controllers/                # API endpoints
│   ├── DTOs/                       # Data transfer objects
│   ├── Middleware/                 # Custom middleware
│   └── wwwroot/                    # Static frontend files
├── UserManagementApp.Core/         # Domain layer
│   ├── Entities/                   # Domain models
│   ├── Enums/                      # Enumerations
│   └── Interfaces/                 # Repository/service interfaces
└── UserManagementApp.Infrastructure/ # Data access layer
    ├── Data/                       # Database context
    ├── Migrations/                 # EF Core migrations
    ├── Repositories/               # Repository implementations
    └── Services/                   # Business logic services
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL (local or Azure)
- Gmail account (for email verification)

### Local Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/sayad-dot/UserManagementApp.git
   cd UserManagementApp
   ```

2. **Configure Database**
   
   Update `appsettings.json` in `UserManagementApp.API/`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=UserManagementDb;Username=postgres;Password=yourpassword;"
     }
   }
   ```

3. **Configure Email**
   
   Add your Gmail app password:
   ```json
   {
     "Email": {
       "SmtpServer": "smtp.gmail.com",
       "Port": 587,
       "Username": "your-email@gmail.com",
       "Password": "your-app-password"
     }
   }
   ```

4. **Run the application**
   ```bash
   cd UserManagementApp.API
   dotnet run
   ```

5. **Access the application**
   ```
   http://localhost:5096
   ```

## 📝 Usage

1. **Register**: Create a new account with name, email, and password
2. **Verify Email**: Check your email and click the verification link
3. **Login**: Access the user management dashboard
4. **Manage Users**: 
   - View all users sorted by last login
   - Select users using checkboxes
   - Block/unblock users
   - Delete users (including yourself)
   - Remove all unverified users

## 🔒 Security Features

- ✅ JWT token-based authentication
- ✅ Password hashing with BCrypt
- ✅ Unique email constraint at database level
- ✅ User status validation middleware
- ✅ HTTPS enforcement
- ✅ CORS configuration
- ✅ SQL injection prevention via EF Core

## 📋 Requirements Met

This project fulfills all specified requirements:

1. ✅ Unique index in database (not just code validation)
2. ✅ Professional table and toolbar design
3. ✅ Data sorted by last login time
4. ✅ Multiple selection with checkboxes
5. ✅ Server-side user validation before each request
6. ✅ Bootstrap CSS framework implementation
7. ✅ Users can manage themselves and others
8. ✅ Non-empty password validation
9. ✅ Deleted users are actually removed (not marked)
10. ✅ Asynchronous email verification
11. ✅ Blocked users cannot login
12. ✅ `getUniqIdValue()` function implemented
13. ✅ Extensive code comments with keywords
14. ✅ No browser alerts (using toast notifications)
15. ✅ No buttons in table rows (toolbar only)

## 🌐 Deployment

See [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) for detailed deployment instructions to:
- Azure App Service
- Other cloud platforms
- Custom servers

## 📄 License

This project is created for educational purposes as part of the iTransition internship task.

## 👤 Author

**Sayad Ibn Azad**
- GitHub: [@sayad-dot](https://github.com/sayad-dot)
- Email: sayadibnaazad@iut-dhaka.edu

## 🙏 Acknowledgments

- Built as part of iTransition internship program
- Task requirements provided by iTransition
- Uses open-source libraries and frameworks
