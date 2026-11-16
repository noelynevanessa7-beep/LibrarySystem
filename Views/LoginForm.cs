using System.Drawing;
using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class LoginForm : Form
{
    private readonly TextBox txtEmail = new TextBox { Width = 240 };
    private readonly TextBox txtPassword = new TextBox { Width = 240, UseSystemPasswordChar = true };

    public LoginForm()
    {
        Text = "Library System - Log in";
        ClientSize = new Size(400, 290);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var title = new Label
        {
            Text = "Library Book Reservation\r\nand Borrowing System",
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(10, 10, 10, 10)
        };

        var loginButton = new Button { Text = "Log in", AutoSize = true, Padding = new Padding(10, 2, 10, 2) };
        loginButton.Click += (s, e) => UI.Try(() =>
        {
            AuthController.Login(txtEmail.Text, txtPassword.Text);
            DialogResult = DialogResult.OK;
        });

        var registerButton = new Button { Text = "Register as member", AutoSize = true, Padding = new Padding(10, 2, 10, 2) };
        registerButton.Click += (s, e) =>
        {
            using var form = new RegisterForm();
            form.ShowDialog(this);
        };

        var buttons = new FlowLayoutPanel { AutoSize = true };
        buttons.Controls.Add(loginButton);
        buttons.Controls.Add(registerButton);

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(20)
        };
        layout.Controls.Add(title);
        layout.Controls.Add(UI.FormTable(("Email", txtEmail), ("Password", txtPassword)));
        layout.Controls.Add(buttons);

        Controls.Add(layout);
        AcceptButton = loginButton;
    }
}
