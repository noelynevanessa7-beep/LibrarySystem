using System.Drawing;
using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class MainForm : Form
{
    private readonly TabControl tabs = new TabControl { Dock = DockStyle.Fill };
    private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 5 * 60 * 1000 };

    /// <summary>True when the user clicked "Log out" (false when the window was simply closed).</summary>
    public bool LoggedOut { get; private set; }

    public MainForm()
    {
        var user = Session.CurrentUser;
        Text = "Library Book Reservation and Borrowing System";
        ClientSize = new Size(1000, 620);
        StartPosition = FormStartPosition.CenterScreen;

        var welcome = new Label
        {
            Text = $"   Welcome, {user.FullName}  ({user.Role})",
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        var logout = new Button { Text = "Log out", Dock = DockStyle.Right, Width = 100, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        logout.Click += (s, e) => { AuthController.Logout(); LoggedOut = true; Close(); };

        var header = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(33, 56, 92) };
        header.Controls.Add(welcome);   // Fill first, then the Right-docked button
        header.Controls.Add(logout);

        // A user sees only the pages of their role (REQ-1.3)
        AddTab("Catalogue", new CatalogueView(canReserve: user.Role == "member"));
        switch (user.Role)
        {
            case "member":
                AddTab("My Reservations", new MyReservationsView());
                AddTab("My Loans", new MyLoansView());
                AddTab("Notifications", new NotificationsView());
                break;
            case "librarian":
                AddTab("Manage Books", new BookManagementView());
                AddTab("Loan Desk", new LoanDeskView());
                AddTab("Overdue Loans", new OverdueView());
                break;
            case "administrator":
                AddTab("Librarian Accounts", new LibrarianAccountsView());
                break;
        }

        Controls.Add(tabs);     // Fill first, then the Top-docked header
        Controls.Add(header);

        tabs.SelectedIndexChanged += (s, e) => RefreshCurrentTab();
        Shown += (s, e) => { UI.Try(ReportController.RunMaintenance); RefreshCurrentTab(); };

        timer.Tick += (s, e) => { try { ReportController.RunMaintenance(); } catch { /* try again next time */ } };
        timer.Start();
        FormClosed += (s, e) => timer.Dispose();
    }

    private void AddTab(string title, Control content)
    {
        content.Dock = DockStyle.Fill;
        var page = new TabPage(title);
        page.Controls.Add(content);
        tabs.TabPages.Add(page);
    }

    private void RefreshCurrentTab()
    {
        var page = tabs.SelectedTab;
        if (page != null && page.Controls.Count > 0 && page.Controls[0] is IRefreshable view)
            UI.Try(view.RefreshData);
    }
}
