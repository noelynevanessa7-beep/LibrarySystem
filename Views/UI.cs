using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

/// <summary>Screens that reload their data when shown.</summary>
public interface IRefreshable
{
    void RefreshData();
}

/// <summary>Small helpers so every screen is built the same way.</summary>
public static class UI
{
    public static void Try(Action action)
    {
        try { action(); }
        catch (AppException ex) { MessageBox.Show(ex.Message, "Library", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (Npgsql.NpgsqlException ex) { MessageBox.Show("Database error: " + ex.Message, "Library", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        catch (Exception ex) { MessageBox.Show("Unexpected error: " + ex.Message, "Library", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    public static Button MakeButton(string text, Action onClick)
    {
        var b = new Button { Text = text, AutoSize = true, Padding = new Padding(8, 2, 8, 2), Margin = new Padding(4) };
        b.Click += (s, e) => Try(onClick);
        return b;
    }

    public static Label MakeLabel(string text) =>
        new Label { Text = text, AutoSize = true, Margin = new Padding(6, 9, 2, 0) };

    public static FlowLayoutPanel Toolbar(params Control[] controls)
    {
        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(4) };
        bar.Controls.AddRange(controls);
        return bar;
    }

    public static DataGridView Grid() => new DataGridView
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White
    };

    /// <summary>A toolbar on top and a control filling the rest.</summary>
    public static TableLayoutPanel Stack(Control top, Control fill)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        top.Dock = DockStyle.Fill;
        fill.Dock = DockStyle.Fill;
        t.Controls.Add(top, 0, 0);
        t.Controls.Add(fill, 0, 1);
        return t;
    }

    /// <summary>Label + input pairs in two columns.</summary>
    public static TableLayoutPanel FormTable(params (string Label, Control Input)[] rows)
    {
        var t = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Padding = new Padding(4) };
        foreach (var (label, input) in rows)
        {
            t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            t.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 10, 3) });
            input.Margin = new Padding(3, 5, 3, 3);
            t.Controls.Add(input);
        }
        return t;
    }

    public static void Bind(DataGridView grid, DataTable table)
    {
        grid.DataSource = table;
        if (grid.Columns.Contains("Id")) grid.Columns["Id"].Visible = false;
    }

    public static int SelectedId(DataGridView grid)
    {
        if (grid.CurrentRow == null) throw new AppException("Please select a row first.");
        return Convert.ToInt32(grid.CurrentRow.Cells["Id"].Value);
    }

    public static void Info(string message) =>
        MessageBox.Show(message, "Library", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
