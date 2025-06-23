# 🗳️ eVote Web Application

**eVote** is a monolithic ASP.NET web application that enables users to register and vote for candidates, respecting specific rules and restrictions around voting behavior.

---

## 🚀 Basic Functionalities

### ✅ User Registration
- Users can register with an email and password.
- Registered users can log in.

### 👤 Account Management
- Users can become candidates or drop out.
- Candidates cannot vote.

### 🗂️ Voting List
- Publicly accessible.
- Shows how many **valid votes** remain for each user.
- Displays vote count per user.
- Sorted in descending order by number of votes.
- Shows who each user voted for.

### 🧾 Voting Rules
- Candidates cannot vote.
- Users must be logged in to vote.
- Max **2 votes per user** (1 per candidate).
- Users can change votes at any time.
- Becoming a candidate keeps previous votes (now invalid).
- Leaving candidacy does not restore past votes’ validity.

---

## ⚙️ About This Project

This is a prototype built in just **4 days**, so it is still in an early stage with limited features, test coverage, and structure. Some `//TODO` comments, unused classes, and failing tests have been intentionally left for future development.

While this doesn’t reflect production-level standards, it demonstrates how I structure logic and think during fast-paced development.

---

## 🔒 Server/Client Security Model

Even though the app is monolithic, it mimics a client-server architecture:

- Authentication token is stored in a **cookie**.
- The token is sent with every API request.
- The server validates the token and returns user info.

This setup helps separate client logic from server validation, improving modularity and security.

---

## 🛢️ Database

- The app uses **SQLite** (`eVote/eVote.db`) — auto-created and pre-populated.
- All users are **single-letter emails** with repeating-letter passwords:

| Email | Password |
|-------|----------|
| `a`   | `aaaa`   |
| `b`   | `bbbb`   |
| `c`   | `cccc`   |
| ...   | ...      |

This makes testing easier and faster.

---

## 📡 API

- The backend API controller is located at:  
  `eVote/Controllers/ApiController.cs`
- Used by the client-side (Razor Pages) to perform authenticated operations.
- **Note:** No Swagger/OpenAPI documentation yet.

---

## ⚠️ Known Issues & Limitations

- **Port Binding:**  
  App runs by default on `https://localhost:7152/`.  
  Ports are set in both `launchSettings.json` and `appsettings.json`.  
  You may need to update them manually if port conflicts arise.

- **Cookies:**  
  Make sure cookies are enabled in your browser. The app relies on them for authentication.

- **Testing:**  
  Some tests are incomplete or failing. These were left for future iterations and learning purposes. Priority was given to the development of the application than to the tests

- **Security:**  
  Password validation exists, but since the app is monolithic, **security is weak**. Client and server logic are not truly isolated, so assume limited protection.

---

## 🧩 Requirements

To run this project, you’ll need:

- **Visual Studio 2022** (or later)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

## 📦 Dependencies

This project uses the following NuGet packages:

| Package                                           | Version  | Description                                                                 |
|--------------------------------------------------|----------|-----------------------------------------------------------------------------|
| `Microsoft.AspNetCore.Authentication.JwtBearer`  | 8.0.17   | Enables JWT authentication for ASP.NET Core.                               |
| `Microsoft.EntityFrameworkCore`                  | 9.0.6    | Core ORM framework for .NET.                                                |
| `Microsoft.EntityFrameworkCore.Design`           | 9.0.6    | Design-time EF Core tools.                                                  |
| `Microsoft.EntityFrameworkCore.Sqlite`           | 9.0.6    | SQLite database provider.                                                   |
| `Microsoft.EntityFrameworkCore.SqlServer`        | 9.0.6    | SQL Server database provider (optional).                                    |
| `System.IdentityModel.Tokens.Jwt`                | 8.12.1   | Utilities for generating and validating JWT tokens.                         |

---

## 🏁 Quick Start

To run the application launch it like you normally would in a C# .NET application :

From Visual Studio:
1. Open the solution in Visual Studio.
2. Click **Start** or press `F5` to run.


Alternatively, you can run the application from the command line:
1. Go to the eVote project directory (where this README file is located).
2. Run the following command:

```bash
dotnet run --launch-profile "https"
```