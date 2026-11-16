using System.Data;

namespace Lab3.Models;

public static class BookRepository
{
    // Free copies = copies - copies on loan - copies kept for a waiting member
    private const string FreeExpr =
        @"(b.copies
           - (SELECT COUNT(*) FROM loans l WHERE l.book_id = b.id AND l.return_date IS NULL)
           - (SELECT COUNT(*) FROM reservations r WHERE r.book_id = b.id AND r.status = 'ready'))";

    public static DataTable Search(string term) =>
        Db.Table($@"SELECT id AS ""Id"", section AS ""Section"", title AS ""Title"", author AS ""Author"",
                           copies AS ""Copies"", CASE WHEN free_copies > 0 THEN 'Yes' ELSE 'No' END AS ""Available""
                    FROM (SELECT b.*, {FreeExpr} AS free_copies FROM books b) x
                    WHERE @t = '' OR title ILIKE @p OR author ILIKE @p OR section ILIKE @p
                    ORDER BY section, title",
                 ("t", term), ("p", "%" + term + "%"));

    public static Book Find(int id) =>
        Db.Query("SELECT id, title, author, section, copies FROM books WHERE id = @id",
                 r => new Book { Id = r.GetInt32(0), Title = r.GetString(1), Author = r.GetString(2),
                                 Section = r.GetString(3), Copies = r.GetInt32(4) },
                 ("id", id)).FirstOrDefault();

    public static int FreeCopies(int id)
    {
        object v = Db.Scalar($"SELECT {FreeExpr} FROM books b WHERE b.id = @id", ("id", id));
        return v == null ? 0 : Convert.ToInt32(v);
    }

    public static int ActiveLoans(int id) =>
        Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM loans WHERE book_id = @id AND return_date IS NULL", ("id", id)));

    public static void Insert(Book b) =>
        Db.Exec("INSERT INTO books (title, author, section, copies) VALUES (@t, @a, @s, @c)",
                ("t", b.Title), ("a", b.Author), ("s", b.Section), ("c", b.Copies));

    public static void Update(Book b) =>
        Db.Exec("UPDATE books SET title = @t, author = @a, section = @s, copies = @c WHERE id = @id",
                ("t", b.Title), ("a", b.Author), ("s", b.Section), ("c", b.Copies), ("id", b.Id));

    public static void Delete(int id) =>
        Db.Exec("DELETE FROM books WHERE id = @id", ("id", id));
}
