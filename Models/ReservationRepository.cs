using System.Data;

namespace Lab3.Models;

public static class ReservationRepository
{
    private const string Active = "status IN ('waiting', 'ready')";

    public static bool HasActive(int bookId, int memberId) =>
        Convert.ToInt64(Db.Scalar($"SELECT COUNT(*) FROM reservations WHERE book_id = @b AND member_id = @m AND {Active}",
                                  ("b", bookId), ("m", memberId))) > 0;

    public static bool OthersWaiting(int bookId, int memberId) =>
        Convert.ToInt64(Db.Scalar($"SELECT COUNT(*) FROM reservations WHERE book_id = @b AND member_id <> @m AND {Active}",
                                  ("b", bookId), ("m", memberId))) > 0;

    /// <summary>Creates the reservation and returns the member's place in the waiting list.</summary>
    public static int Create(int bookId, int memberId)
    {
        Db.Exec("INSERT INTO reservations (book_id, member_id) VALUES (@b, @m)", ("b", bookId), ("m", memberId));
        return Convert.ToInt32(Db.Scalar(
            $@"SELECT COUNT(*) FROM reservations
               WHERE book_id = @b AND {Active}
                 AND id <= (SELECT MAX(id) FROM reservations WHERE book_id = @b AND member_id = @m AND {Active})",
            ("b", bookId), ("m", memberId)));
    }

    public static Reservation Find(int id) =>
        Db.Query("SELECT id, book_id, member_id, status FROM reservations WHERE id = @id",
                 r => new Reservation { Id = r.GetInt32(0), BookId = r.GetInt32(1), MemberId = r.GetInt32(2), Status = r.GetString(3) },
                 ("id", id)).FirstOrDefault();

    public static Reservation ReadyHold(int bookId, int memberId) =>
        Db.Query("SELECT id, book_id, member_id, status FROM reservations WHERE book_id = @b AND member_id = @m AND status = 'ready'",
                 r => new Reservation { Id = r.GetInt32(0), BookId = r.GetInt32(1), MemberId = r.GetInt32(2), Status = r.GetString(3) },
                 ("b", bookId), ("m", memberId)).FirstOrDefault();

    public static void SetStatus(int id, string status) =>
        Db.Exec("UPDATE reservations SET status = @s WHERE id = @id", ("s", status), ("id", id));

    // The place is counted live, so when someone cancels everybody behind moves up (REQ-3.2)
    public static DataTable ForMember(int memberId) =>
        Db.Table($@"SELECT r.id AS ""Id"", b.title AS ""Title"",
                           CASE r.status WHEN 'ready' THEN 'Ready to collect' ELSE 'Waiting' END AS ""Status"",
                           (SELECT COUNT(*) FROM reservations x
                            WHERE x.book_id = r.book_id AND x.status IN ('waiting', 'ready') AND x.id <= r.id) AS ""Place in line"",
                           to_char(r.hold_expires, 'YYYY-MM-DD') AS ""Kept until""
                    FROM reservations r JOIN books b ON b.id = r.book_id
                    WHERE r.member_id = @m AND r.{Active}
                    ORDER BY r.id", ("m", memberId));

    /// <summary>
    /// REQ-3.3: holds not collected within 2 days expire, then every free copy is kept
    /// for the first waiting member (who is notified).
    /// </summary>
    public static void ProcessHolds()
    {
        Db.Exec("UPDATE reservations SET status = 'expired' WHERE status = 'ready' AND hold_expires < CURRENT_DATE");

        var bookIds = Db.Query("SELECT DISTINCT book_id FROM reservations WHERE status = 'waiting'", r => r.GetInt32(0));
        foreach (int bookId in bookIds)
        {
            while (BookRepository.FreeCopies(bookId) > 0)
            {
                var next = Db.Query(
                    @"SELECT r.id, r.member_id, b.title FROM reservations r JOIN books b ON b.id = r.book_id
                      WHERE r.book_id = @b AND r.status = 'waiting' ORDER BY r.id LIMIT 1",
                    r => (Id: r.GetInt32(0), MemberId: r.GetInt32(1), Title: r.GetString(2)),
                    ("b", bookId)).FirstOrDefault();

                if (next.Id == 0) break;

                Db.Exec("UPDATE reservations SET status = 'ready', hold_expires = CURRENT_DATE + 2 WHERE id = @id", ("id", next.Id));
                NotificationRepository.Add(next.MemberId,
                    $"The book \"{next.Title}\" is now kept for you for 2 days. Please collect it at the loan desk.");
            }
        }
    }
}
