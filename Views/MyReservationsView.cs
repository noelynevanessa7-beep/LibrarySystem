using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class MyReservationsView : UserControl, IRefreshable
{
    private readonly DataGridView grid = UI.Grid();

    public MyReservationsView()
    {
        var bar = UI.Toolbar(
            UI.MakeButton("Cancel selected reservation", Cancel),
            UI.MakeButton("Refresh", RefreshData));
        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData() => UI.Bind(grid, ReservationController.Mine());

    private void Cancel()
    {
        int id = UI.SelectedId(grid);
        if (MessageBox.Show("Cancel this reservation?", "Library", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        ReservationController.Cancel(id);
        RefreshData();
    }
}
