using System.Data;

namespace Lab3.Models;

public static class LoanRepository
{
    /// <summary>Issues a book (due 14 days from today) and returns the due date.</summary>
    public static DateTime Issue(int bookId, int memberId) =>
        Convert.ToDateTime(Db.Scalar(
            @"INSERT INTO loans (book_id, member_id, borrow_date, due_date)
              VALUES (@b, @m, CURRENT_DATE, CURRENT_DATE + 14) RETURNING due_date",
            ("b", bookId), ("m", memberId)));

    public static Loan Find(int id) =>
        Db.Query("SELECT id, book_id, member_id, return_date IS NOT NULL, renewed FROM loans WHERE id = @id",
                 r => new Loan { Id = r.GetInt32(0), BookId = r.GetInt32(1), MemberId = r.GetInt32(2),
                                 Returned = r.GetBoolean(3), Renewed = r.GetBoolean(4) },
                 ("id", id)).FirstOrDefault();

    public static void MarkReturned(int id) =>
        Db.Exec("UPDATE loans SET return_date = CURRENT_DATE WHERE id = @id", ("id", id));

    public static void Renew(int id) =>
        Db.Exec("UPDATE loans SET due_date = due_date + 7, renewed = TRUE, reminder_sent = FALSE WHERE id = @id", ("id", id));

    public static DataTable ForMember(int memberId) =>
        Db.Table(@"SELECT l.id AS ""Id"", b.title AS ""Title"",
                          to_char(l.borrow_date, 'YYYY-MM-DD') AS ""Borrowed"",
                          to_char(l.due_date, 'YYYY-MM-DD') AS ""Due date"",
                          CASE WHEN l.renewed THEN 'Yes' ELSE 'No' END AS ""Renewed""
                   FROM loans l JOIN books b ON b.id = l.book_id
                   WHERE l.member_id = @m AND l.return_date IS NULL
                   ORDER BY l.due_date", ("m", memberId));

    public static DataTable Active() =>
        Db.Table(@"SELECT l.id AS ""Id"", b.title AS ""Title"", u.full_name AS ""Member"",
                          u.member_number AS ""Member no."", to_char(l.due_date, 'YYYY-MM-DD') AS ""Due date""
                   FROM loans l JOIN books b ON b.id = l.book_id JOIN users u ON u.id = l.member_id
                   WHERE l.return_date IS NULL
                   ORDER BY l.due_date");

    public static DataTable Overdue() =>
        Db.Table(@"SELECT l.id AS ""Id"", b.title AS ""Title"", u.full_name AS ""Member"",
                          to_char(l.due_date, 'YYYY-MM-DD') AS ""Due date"",
                          (CURRENT_DATE - l.due_date) AS ""Days late""
                   FROM loans l JOIN books b ON b.id = l.book_id JOIN users u ON u.id = l.member_id
                   WHERE l.return_date IS NULL AND l.due_date < CURRENT_DATE
                   ORDER BY ""Days late"" DESC");

    /// <summary>Loans not returned, due within 2 days, and not yet reminded (REQ-5.2).</summary>
    public static List<(int LoanId, int MemberId, string Title, DateTime Due)> DueForReminder() =>
        Db.Query(@"SELECT l.id, l.member_id, b.title, l.due_date
                   FROM loans l JOIN books b ON b.id = l.book_id
                   WHERE l.return_date IS NULL AND l.reminder_sent = FALSE
                     AND l.due_date - CURRENT_DATE BETWEEN 0 AND 2",
                 r => (r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetDateTime(3)));

    public static void MarkReminded(int id) =>
        Db.Exec("UPDATE loans SET reminder_sent = TRUE WHERE id = @id", ("id", id));
}
