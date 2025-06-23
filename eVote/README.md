# eVote Web Application

This project is a web application called **eVote** that allows users to register and vote for candidates, following the rules and requirements below.

## Basic Functionalities

- [ ] **User Registration**
    - [ ] Users can register by providing an email address and a password.
    - [ ] Once registered, users can log in to the application.

- [ ] **Account**
    - [ ] Users can become candidates or drop out of the candidate list.
    - [ ] If a user is a candidate, they cannot vote for anyone.

- [ ] **Voting List**
    - [ ] Anyone can view the voting list.
    - [ ] The list shows how many valid votes are still possible for all users
    - [ ] The voting list contains shows the number of votes each user has received.
    - [ ] The user list is sorted in descending order by the number of votes.
    - [ ] Users can see who they voted for

- [ ] **Voting Rules**
    - [ ] Candidates cannot vote for anyone.
    - [ ] Users must be logged in to vote.
    - [ ] Users can only vote once per candidate.
    - [ ] Users can vote for a maximum of two candidates.
    - [ ] Users can change their votes at any time.
    - [ ] When a user becomes a candidate, they keep their previous votes but are no longer counted as valid votes.
    - [ ] When a user drops out of the candidate list, their candidate votes will still appear for reference but are not counted as valid votes.

## About the Application

This is a web application done in only 4 days in ASP.NET, due to this fact, it is still in its early stages of development with a very limited set of features, test coverage, and documentation. You might still find some bugs, notes (//TODO), and unused classes left in the code by the developer for future reference, they were left intentionally for the sake of completeness. It is not as structured or organized as a project with proper setup, linting, and testing, and doesn't fully represent my developing style and how I would work in an actual team, but it is a starting reference to understand how I think when coding.

### Server/Client Security idea

Although the application is monolithic, an effort was made to try separate what would be client and server for security. A browser cookie is used to store the user's authentication token, and the token is sent with every request to the server, the server validates the token and returns the user's information if the token is valid, this is done to prevent unauthorized access to the server.

### Database

The application uses SQLite as the database, and the database is created and managed by the application, it is located at "eVote/eVote.db" and already contains some data.

Currently all users are letters, and the password is the given letter repeated four times, for example Email: "a" Password: "aaaa", this is to make it easier to test the application, you can access any user by using the email and password.

### API 

The applications uses an API, the controller is located at "eVote/Controllers/ApiController.cs" and is used to handle the requests from the client (front-end), saddly no swagger documentation is available for the API.

## Posible issues

Due to the limited time and lack of experience in certain areas, especially in the Front-End, and server-side development, there might be some problems with the application, such as:

- The application currently runs in "https://localhost:7152/" , the port number is set in appsettings.json and in launchSettings.json you can change it to any other port you want in case there are any conflicts (I wasn't able to find a simple way to dynamically find a port and wire it to the application).

- The application uses Cookies for authentication, make sure your browser allows them

- The test coverage is not complete, and there are some tests that fail, I decided to leave them in the code for future reference, but they are not maintained, priority was given to the development of the application than to the tests.

- Since the application is monolithic, security is weak, although password checking was implemented, this would assume that the client doesn t have access to the server, which is not the case. Since security is an additional feature, it is not very important.

## Requirements

To run the application, you will need to run Visual Studio 2022 with the following dependencies:

### 📦 Dependencies

This project relies on the following NuGet packages:

| Package                                           | Version  | Description                                                                 |
|--------------------------------------------------|----------|-----------------------------------------------------------------------------|
| `Microsoft.AspNetCore.Authentication.JwtBearer`  | 8.0.17   | Adds support for authenticating JWT tokens in ASP.NET Core applications.   |
| `Microsoft.EntityFrameworkCore`                  | 9.0.6    | Core package for using EF Core for data access.                            |
| `Microsoft.EntityFrameworkCore.Design`           | 9.0.6    | Design-time tools for EF Core (used for migrations, scaffolding, etc.).    |
| `Microsoft.EntityFrameworkCore.Sqlite`           | 9.0.6    | EF Core provider for SQLite.                                               |
| `Microsoft.EntityFrameworkCore.SqlServer`        | 9.0.6    | EF Core provider for SQL Server.                                           |
| `System.IdentityModel.Tokens.Jwt`                | 8.12.1   | Library for creating and validating JWT tokens.                            |

> Target framework: **.NET 8.0**

## Quick Start

To run the application launch it like you normally would in a C# .NET application, open the project in Visual Studio and use the top buttons to Run in Debug or Normal mode. You can use the following command: `dotnet run --launch-profile "https"`

