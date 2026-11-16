using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class MyLoansView : UserControl, IRefreshable
{
    private readonly DataGridView grid = UI.Grid();

    public MyLoansView()
    {
        var bar = UI.Toolbar(
            UI.MakeButton("Renew selected loan (+7 days)", Renew),
            UI.MakeButton("Refresh", RefreshData));
        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData() => UI.Bind(grid, LoanController.Mine());

    private void Renew()
    {
        LoanController.Renew(UI.SelectedId(grid));
        UI.Info("Loan renewed for 7 more days.");
        RefreshData();
    }
}
