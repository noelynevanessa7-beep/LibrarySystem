using System.Data;
using System.Text.Json;
using Npgsql;

namespace Lab3.Models;

/// <summary>Small helper around Npgsql so the repositories stay short.</summary>
public static class Db
{
    private static string _connectionString;

    public static string ConnectionString
    {
        get
        {
            if (_connectionString == null)
            {
                string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                _connectionString = doc.RootElement.GetProperty("ConnectionString").GetString();
            }
            return _connectionString;
        }
    }

    private static NpgsqlCommand Command(NpgsqlConnection con, string sql, (string, object)[] args)
    {
        var cmd = new NpgsqlCommand(sql, con);
        foreach (var (name, value) in args)
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        return cmd;
    }

    public static int Exec(string sql, params (string, object)[] args)
    {
        using var con = new NpgsqlConnection(ConnectionString);
        con.Open();
        using var cmd = Command(con, sql, args);
        return cmd.ExecuteNonQuery();
    }

    public static object Scalar(string sql, params (string, object)[] args)
    {
        using var con = new NpgsqlConnection(ConnectionString);
        con.Open();
        using var cmd = Command(con, sql, args);
        object value = cmd.ExecuteScalar();
        return value == DBNull.Value ? null : value;
    }

    public static DataTable Table(string sql, params (string, object)[] args)
    {
        using var con = new NpgsqlConnection(ConnectionString);
        con.Open();
        using var cmd = Command(con, sql, args);
        using var adapter = new NpgsqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static List<T> Query<T>(string sql, Func<NpgsqlDataReader, T> map, params (string, object)[] args)
    {
        using var con = new NpgsqlConnection(ConnectionString);
        con.Open();
        using var cmd = Command(con, sql, args);
        using var reader = cmd.ExecuteReader();
        var list = new List<T>();
        while (reader.Read()) list.Add(map(reader));
        return list;
    }

    /// <summary>Creates the tables, the first administrator and a few sample books.</summary>
    public static void Init()
    {
        string schema = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Database", "schema.sql"));
        Exec(schema);

        if (Convert.ToInt64(Scalar("SELECT COUNT(*) FROM users WHERE role = 'administrator'")) == 0)
        {
            Exec("INSERT INTO users (full_name, email, password_hash, role) VALUES (@n, @e, @p, 'administrator')",
                 ("n", "Administrator"), ("e", "admin@library.com"), ("p", PasswordHasher.Hash("Admin@123")));
        }

        if (Convert.ToInt64(Scalar("SELECT COUNT(*) FROM books")) == 0)
        {
            Exec(@"INSERT INTO books (title, author, section, copies) VALUES
                   ('Clean Code', 'Robert C. Martin', 'Computer Science', 2),
                   ('Introduction to Algorithms', 'Thomas H. Cormen', 'Computer Science', 1),
                   ('Things Fall Apart', 'Chinua Achebe', 'Literature', 3),
                   ('A Brief History of Time', 'Stephen Hawking', 'Science', 1),
                   ('Principles of Economics', 'N. Gregory Mankiw', 'Economics', 2)");
        }
    }
}
