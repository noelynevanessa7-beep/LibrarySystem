using Lab3.Models;

namespace Lab3.Controllers;

public static class AuthController
{
    // REQ-1.1
    public static void Register(string name, string number, string email, string password, string confirm)
    {
        name = (name ?? "").Trim();
        number = (number ?? "").Trim();
        email = (email ?? "").Trim().ToLowerInvariant();

        if (name == "" || number == "" || email == "" || string.IsNullOrEmpty(password))
            throw new AppException("Please fill in the name, student or staff number, email and password.");
        CheckEmail(email);
        CheckPassword(password);
        if (password != confirm) throw new AppException("The two passwords are not the same.");

        bool emailTaken = UserRepository.FindByEmail(email) != null;
        bool numberTaken = UserRepository.FindByMemberNumber(number) != null;
        if (emailTaken && numberTaken) throw new AppException("This email and this student or staff number are already registered.");
        if (emailTaken) throw new AppException("This email is already registered.");
        if (numberTaken) throw new AppException("This student or staff number is already registered.");

        UserRepository.Insert(new User
        {
            FullName = name, MemberNumber = number, Email = email,
            PasswordHash = PasswordHasher.Hash(password), Role = "member"
        });
    }

    // REQ-1.2: one general message for a wrong password or an unknown email
    public static void Login(string email, string password)
    {
        var user = UserRepository.FindByEmail((email ?? "").Trim().ToLowerInvariant());
        if (user == null || !PasswordHasher.Verify(password ?? "", user.PasswordHash))
            throw new AppException("Wrong email or password.");
        Session.Start(user);
    }

    public static void Logout() => Session.End();

    // REQ-1.3: only the administrator creates librarian accounts
    public static void CreateLibrarian(string name, string email, string password)
    {
        Session.Require("administrator");
        name = (name ?? "").Trim();
        email = (email ?? "").Trim().ToLowerInvariant();

        if (name == "" || email == "" || string.IsNullOrEmpty(password))
            throw new AppException("Please fill in the name, email and password.");
        CheckEmail(email);
        CheckPassword(password);
        if (UserRepository.FindByEmail(email) != null) throw new AppException("This email is already registered.");

        UserRepository.Insert(new User
        {
            FullName = name, MemberNumber = null, Email = email,
            PasswordHash = PasswordHasher.Hash(password), Role = "librarian"
        });
    }

    public static System.Data.DataTable Librarians()
    {
        Session.Require("administrator");
        return UserRepository.Librarians();
    }

    private static void CheckEmail(string email)
    {
        if (!email.Contains('@') || !email.Contains('.')) throw new AppException("Please enter a valid email address.");
    }

    private static void CheckPassword(string password)
    {
        if (password.Length < 6) throw new AppException("The password must have at least 6 characters.");
    }
}
