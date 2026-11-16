using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class NotificationsView : UserControl, IRefreshable
{
    private readonly DataGridView grid = UI.Grid();

    public NotificationsView()
    {
        var bar = UI.Toolbar(
            UI.MakeButton("Mark all as read", () => { NotificationController.MarkAllRead(); RefreshData(); }),
            UI.MakeButton("Refresh", RefreshData));
        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData()
    {
        UI.Bind(grid, NotificationController.Mine());
        if (grid.Columns.Contains("Message")) grid.Columns["Message"].FillWeight = 400;
    }
}
