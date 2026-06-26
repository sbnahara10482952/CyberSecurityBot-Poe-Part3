namespace CyberSecurityBotGUI;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1100, 750);
        this.Text = "🔐 Cybersecurity Awareness Bot - POE Part 3";
        this.BackColor = System.Drawing.Color.Black;
    }
}