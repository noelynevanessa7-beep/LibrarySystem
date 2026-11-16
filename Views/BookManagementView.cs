using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class BookManagementView : UserControl, IRefreshable
{
    private readonly TextBox txtTitle = new TextBox { Width = 180 };
    private readonly TextBox txtAuthor = new TextBox { Width = 150 };
    private readonly TextBox txtSection = new TextBox { Width = 120 };
    private readonly TextBox txtCopies = new TextBox { Width = 50 };
    private readonly DataGridView grid = UI.Grid();

    public BookManagementView()
    {
        var bar = UI.Toolbar(
            UI.MakeLabel("Title"), txtTitle,
            UI.MakeLabel("Author"), txtAuthor,
            UI.MakeLabel("Section"), txtSection,
            UI.MakeLabel("Copies"), txtCopies,
            UI.MakeButton("Add", () => { BookController.Add(txtTitle.Text, txtAuthor.Text, txtSection.Text, txtCopies.Text); Reload(true); }),
            UI.MakeButton("Update selected", () => { BookController.Update(UI.SelectedId(grid), txtTitle.Text, txtAuthor.Text, txtSection.Text, txtCopies.Text); Reload(false); }),
            UI.MakeButton("Remove selected", Remove),
            UI.MakeButton("Clear", ClearInputs));

        grid.SelectionChanged += (s, e) => FillInputsFromSelection();
        Controls.Add(UI.Stack(bar, grid));
    }

    public void RefreshData() => Reload(false);

    private void Reload(bool clear)
    {
        UI.Bind(grid, BookController.Catalogue(""));
        if (clear) ClearInputs();
    }

    private void Remove()
    {
        int id = UI.SelectedId(grid);
        if (MessageBox.Show("Remove this book from the catalogue?", "Library", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        BookController.Remove(id);
        Reload(true);
    }

    private void FillInputsFromSelection()
    {
        if (grid.CurrentRow == null || !grid.Columns.Contains("Title")) return;
        txtTitle.Text = Convert.ToString(grid.CurrentRow.Cells["Title"].Value);
        txtAuthor.Text = Convert.ToString(grid.CurrentRow.Cells["Author"].Value);
        txtSection.Text = Convert.ToString(grid.CurrentRow.Cells["Section"].Value);
        txtCopies.Text = Convert.ToString(grid.CurrentRow.Cells["Copies"].Value);
    }

    private void ClearInputs()
    {
        txtTitle.Clear(); txtAuthor.Clear(); txtSection.Clear(); txtCopies.Clear();
    }
}
