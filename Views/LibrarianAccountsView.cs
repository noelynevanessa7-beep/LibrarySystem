using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class LibrarianAccountsView : UserControl, IRefreshable
{
    private readonly TextBox txtName = new TextBox { Width = 160 };
    private readonly TextBox txtEmail = new TextBox { Width = 200 };
    private readonly TextBox txtPassword = new TextBox { Width = 140, UseSystemPasswordChar = true };
    private readonly DataGridView grid = UI.Grid();

    public LibrarianAccountsView()
    {
        var bar = UI.Toolbar(
            UI.MakeLabel("Name"), txtName,
            UI.MakeLabel("Email"), txtEmail,
            UI.MakeLabel("Password"), txtPassword,
            UI.MakeButton("Create librarian account", Create));
        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData() => UI.Bind(grid, AuthController.Librarians());

    private void Create()
    {
        AuthController.CreateLibrarian(txtName.Text, txtEmail.Text, txtPassword.Text);
        txtName.Clear(); txtEmail.Clear(); txtPassword.Clear();
        UI.Info("Librarian account created.");
        RefreshData();
    }
}
