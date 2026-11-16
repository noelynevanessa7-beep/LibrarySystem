using System.Data;

namespace Lab3.Models;

public static class NotificationRepository
{
    public static void Add(int memberId, string message) =>
        Db.Exec("INSERT INTO notifications (member_id, message) VALUES (@m, @t)", ("m", memberId), ("t", message));

    public static DataTable ForMember(int memberId) =>
        Db.Table(@"SELECT id AS ""Id"", to_char(created_at, 'YYYY-MM-DD HH24:MI') AS ""Date"",
                          message AS ""Message"", CASE WHEN is_read THEN 'Read' ELSE 'New' END AS ""Status""
                   FROM notifications WHERE member_id = @m ORDER BY id DESC", ("m", memberId));

    public static void MarkAllRead(int memberId) =>
        Db.Exec("UPDATE notifications SET is_read = TRUE WHERE member_id = @m", ("m", memberId));
}
