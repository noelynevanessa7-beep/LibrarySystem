namespace Lab3.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string MemberNumber { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }          // member | librarian | administrator
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Section { get; set; }
    public int Copies { get; set; }
}

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public string Status { get; set; }        // waiting | ready | collected | cancelled | expired
}

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public bool Returned { get; set; }
    public bool Renewed { get; set; }
}
