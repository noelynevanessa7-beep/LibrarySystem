using System.Data;
using Npgsql;

namespace Lab3.Models;

public static class UserRepository
{
    private const string Cols = "id, full_name, member_number, email, password_hash, role";

    private static User Map(NpgsqlDataReader r) => new User
    {
        Id = r.GetInt32(0),
        FullName = r.GetString(1),
        MemberNumber = r.IsDBNull(2) ? null : r.GetString(2),
        Email = r.GetString(3),
        PasswordHash = r.GetString(4),
        Role = r.GetString(5)
    };

    public static User FindByEmail(string email) =>
        Db.Query($"SELECT {Cols} FROM users WHERE email = @e", Map, ("e", email)).FirstOrDefault();

    public static User FindByMemberNumber(string number) =>
        Db.Query($"SELECT {Cols} FROM users WHERE member_number = @n", Map, ("n", number)).FirstOrDefault();

    public static void Insert(User u) =>
        Db.Exec("INSERT INTO users (full_name, member_number, email, password_hash, role) VALUES (@n, @m, @e, @p, @r)",
                ("n", u.FullName), ("m", u.MemberNumber), ("e", u.Email), ("p", u.PasswordHash), ("r", u.Role));

    public static DataTable Librarians() =>
        Db.Table("SELECT id AS \"Id\", full_name AS \"Name\", email AS \"Email\" FROM users WHERE role = 'librarian' ORDER BY full_name");
}
