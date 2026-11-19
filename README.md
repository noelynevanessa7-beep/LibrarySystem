# Library Book Reservation and Borrowing System

WinForms C# desktop app with a database, built with the MVC pattern.

Student: Kalaki Noelyn Vanessa 

ID 27540

## Structure (MVC)
- `Models/`      entities + database access (Npgsql repositories, password hashing)
- `Controllers/` business rules from the functional requirements, role checks (`Session.Require`)
- `Views/`       WinForms screens; they only call controllers, never the database
- `Database/schema.sql` tables (created automatically on first start)

## Setup
1. Install .NET 8 SDK, Visual Studio 2022 (".NET desktop development") and PostgreSQL.
2. In pgAdmin or psql create an empty database:  `CREATE DATABASE librarydb;`
3. Open `appsettings.json` and put your PostgreSQL password in the connection string.
4. Open the folder in Visual Studio (File > Open > Project, choose `LibrarySystem.csproj`) and press F5.
   Or in a terminal: `dotnet run`
5. One administrator:
   `admin@library.com` / `Admin@123`

## How it works
1. Log in as the administrator and create a librarian account.
2. Register two members (Register button on the login screen).
3. Log in as the librarian: add books, issue a book to a member (member number), record returns, see overdue loans.
4. Log in as a member: search, reserve a book with no free copy, cancel, renew, read notifications.

## Requirements 
| Requirement | Where |
|---|---|
| REQ-1.x accounts and roles | `AuthController`, `Session`, tabs in `MainForm` |
| REQ-2.x catalogue | `BookController`, `CatalogueView`, `BookManagementView` |
| REQ-3.x reservations | `ReservationController`, `ReservationRepository.ProcessHolds` |
| REQ-4.x loan desk | `LoanController`, `LoanDeskView`, `MyLoansView` |
| REQ-5.x overdue and reminders | `ReportController`, `OverdueView`, notifications |
