using System.Drawing;
using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class CatalogueView : UserControl, IRefreshable
{
    private readonly TextBox txtSearch = new TextBox { Width = 240 };
    private readonly DataGridView grid = UI.Grid();
    private readonly Label lblMessage = new Label { AutoSize = true, ForeColor = Color.Firebrick, Margin = new Padding(10, 9, 0, 0) };

    public CatalogueView(bool canReserve)
    {
        var bar = UI.Toolbar(
            UI.MakeLabel("Search by title, author or section:"), txtSearch,
            UI.MakeButton("Search", RefreshData),
            UI.MakeButton("Show all", () => { txtSearch.Clear(); RefreshData(); }));
        if (canReserve) bar.Controls.Add(UI.MakeButton("Reserve selected book", Reserve));
        bar.Controls.Add(lblMessage);

        txtSearch.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; UI.Try(RefreshData); }
        };

        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData()
    {
        var table = BookController.Catalogue(txtSearch.Text);
        UI.Bind(grid, table);
        lblMessage.Text = table.Rows.Count == 0 ? "No books found" : "";
    }

    private void Reserve()
    {
        int place = ReservationController.Reserve(UI.SelectedId(grid));
        UI.Info($"Book reserved. Your place in the waiting list is {place}.");
        RefreshData();
    }
}
