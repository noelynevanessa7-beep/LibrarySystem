using System.Data;
using Lab3.Models;

namespace Lab3.Controllers;

public static class BookController
{
    private static readonly string[] AnyRole = { "member", "librarian", "administrator" };

    // REQ-2.1 and REQ-2.2: an empty term shows every book, grouped by section
    public static DataTable Catalogue(string term)
    {
        Session.Require(AnyRole);
        ReservationRepository.ProcessHolds();
        return BookRepository.Search((term ?? "").Trim());
    }

    // REQ-2.3
    public static void Add(string title, string author, string section, string copies)
    {
        Session.Require("librarian");
        BookRepository.Insert(Build(0, title, author, section, copies));
        ReservationRepository.ProcessHolds();
    }

    public static void Update(int id, string title, string author, string section, string copies)
    {
        Session.Require("librarian");
        if (BookRepository.Find(id) == null) throw new AppException("This book no longer exists.");

        var book = Build(id, title, author, section, copies);
        int borrowed = BookRepository.ActiveLoans(id);
        if (book.Copies < borrowed)
            throw new AppException($"The number of copies cannot be less than the {borrowed} copies currently borrowed.");

        BookRepository.Update(book);
        ReservationRepository.ProcessHolds();
    }

    public static void Remove(int id)
    {
        Session.Require("librarian");
        if (BookRepository.ActiveLoans(id) > 0)
            throw new AppException("This book cannot be removed because it is currently borrowed.");
        BookRepository.Delete(id);
    }

    private static Book Build(int id, string title, string author, string section, string copies)
    {
        title = (title ?? "").Trim();
        author = (author ?? "").Trim();
        section = (section ?? "").Trim();

        if (title == "" || author == "" || section == "" || string.IsNullOrWhiteSpace(copies))
            throw new AppException("The title, author, section and number of copies are all required.");
        if (!int.TryParse(copies.Trim(), out int n))
            throw new AppException("The number of copies must be a whole number.");
        if (n < 0)
            throw new AppException("The number of copies cannot be negative.");

        return new Book { Id = id, Title = title, Author = author, Section = section, Copies = n };
    }
}
