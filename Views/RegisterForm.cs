using System.Drawing;
using System.Windows.Forms;
using Lab3.Controllers;

namespace Lab3.Views;

public class RegisterForm : Form
{
    private readonly TextBox txtName = new TextBox { Width = 240 };
    private readonly TextBox txtNumber = new TextBox { Width = 240 };
    private readonly TextBox txtEmail = new TextBox { Width = 240 };
    private readonly TextBox txtPassword = new TextBox { Width = 240, UseSystemPasswordChar = true };
    private readonly TextBox txtConfirm = new TextBox { Width = 240, UseSystemPasswordChar = true };

    public RegisterForm()
    {
        Text = "Register as a member";
        ClientSize = new Size(420, 330);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var registerButton = new Button { Text = "Register", AutoSize = true, Padding = new Padding(10, 2, 10, 2) };
        registerButton.Click += (s, e) => UI.Try(() =>
        {
            AuthController.Register(txtName.Text, txtNumber.Text, txtEmail.Text, txtPassword.Text, txtConfirm.Text);
            UI.Info("Registration successful. You can now log in.");
            DialogResult = DialogResult.OK;
        });

        var cancelButton = new Button { Text = "Cancel", AutoSize = true, Padding = new Padding(10, 2, 10, 2), DialogResult = DialogResult.Cancel };

        var buttons = new FlowLayoutPanel { AutoSize = true };
        buttons.Controls.Add(registerButton);
        buttons.Controls.Add(cancelButton);

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(20)
        };
        layout.Controls.Add(UI.FormTable(
            ("Full name", txtName),
            ("Student / staff no.", txtNumber),
            ("Email", txtEmail),
            ("Password", txtPassword),
            ("Confirm password", txtConfirm)));
        layout.Controls.Add(buttons);

        Controls.Add(layout);
        AcceptButton = registerButton;
        CancelButton = cancelButton;
    }
}
