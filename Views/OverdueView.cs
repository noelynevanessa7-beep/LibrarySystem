using System.Drawing;
using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class OverdueView : UserControl, IRefreshable
{
    private readonly DataGridView grid = UI.Grid();
    private readonly Label lblMessage = new Label { AutoSize = true, ForeColor = Color.DarkGreen, Margin = new Padding(10, 9, 0, 0) };

    public OverdueView()
    {
        var bar = UI.Toolbar(
            UI.MakeButton("Refresh", RefreshData),
            UI.MakeButton("Send due-soon reminders now", SendReminders),
            lblMessage);
        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData()
    {
        var table = ReportController.Overdue();
        UI.Bind(grid, table);
        lblMessage.Text = table.Rows.Count == 0 ? "No loans are overdue." : "";
    }

    private void SendReminders()
    {
        int n = ReportController.SendReminders();
        UI.Info(n == 0 ? "No new reminders were needed." : $"{n} reminder(s) sent.");
    }
}
