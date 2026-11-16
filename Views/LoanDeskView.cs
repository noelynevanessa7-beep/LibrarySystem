using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class LoanDeskView : UserControl, IRefreshable
{
    private readonly TextBox txtMember = new TextBox { Width = 120 };
    private readonly TextBox txtSearch = new TextBox { Width = 160 };
    private readonly DataGridView bookGrid = UI.Grid();
    private readonly DataGridView loanGrid = UI.Grid();

    public LoanDeskView()
    {
        var issueBar = UI.Toolbar(
            UI.MakeLabel("Member student/staff no."), txtMember,
            UI.MakeLabel("Find book"), txtSearch,
            UI.MakeButton("Search", LoadBooks),
            UI.MakeButton("Issue selected book", Issue));
        var issueBox = new GroupBox { Text = "Issue a book", Dock = DockStyle.Fill };
        issueBox.Controls.Add(UI.Stack(issueBar, bookGrid));

        var returnBar = UI.Toolbar(
            UI.MakeButton("Record return of selected loan", Return),
            UI.MakeButton("Refresh", RefreshData));
        var returnBox = new GroupBox { Text = "Books currently on loan", Dock = DockStyle.Fill };
        returnBox.Controls.Add(UI.Stack(returnBar, loanGrid));

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.Controls.Add(issueBox, 0, 0);
        layout.Controls.Add(returnBox, 0, 1);
        Controls.Add(layout);

        txtSearch.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; UI.Try(LoadBooks); }
        };
    }

    public void RefreshData()
    {
        LoadBooks();
        UI.Bind(loanGrid, LoanController.Active());
    }

    private void LoadBooks() => UI.Bind(bookGrid, BookController.Catalogue(txtSearch.Text));

    private void Issue()
    {
        DateTime due = LoanController.Issue(UI.SelectedId(bookGrid), txtMember.Text);
        UI.Info($"Book issued. It is due on {due:yyyy-MM-dd}.");
        RefreshData();
    }

    private void Return()
    {
        LoanController.Return(UI.SelectedId(loanGrid));
        UI.Info("Return recorded.");
        RefreshData();
    }
}
