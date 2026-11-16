using Lab3;
using Lab3.Models;
using Lab3.Views;
using System.Windows.Forms;

namespace Lab3;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            Db.Init();   // creates tables, the admin account and sample books on first run
        }
        catch (Exception ex)
        {
            MessageBox.Show("Cannot connect to PostgreSQL. Check appsettings.json and that the database exists.\n\n" + ex.Message,
                            "Library", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        while (true)
        {
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) break;

            using var main = new MainForm();
            main.ShowDialog();
            if (!main.LoggedOut) break;     // window closed -> exit; "Log out" -> back to login
        }
    }
}
