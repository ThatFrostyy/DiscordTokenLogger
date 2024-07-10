using System.Windows.Forms;
using CefSharp;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace TokenLogger
{
    public partial class Main : Form
    {
        private string token;
        private System.Timers.Timer timer;

        public Main()
        {
            InitializeComponent();
            webView.LoadUrl("https://discord.com/login");

            Delay();

            timer = new System.Timers.Timer(5000); 
            timer.Elapsed += new ElapsedEventHandler(ToggleComps);
            timer.Start();
        }

        private async void Delay()
        {
            await Task.Delay(5000);

            if (webView.Address == "https://discord.com/login")
            {
                button1.Enabled = true;
                textBox1.Enabled = true;
                checkBox1.Enabled = true;
            }
            else
            {
                MessageBox.Show($"Already logged in, skiping login.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ToggleComps(object source, ElapsedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.Invoke((Action)delegate
                {
                    if (webView.Address == "https://discord.com/login")
                    {
                        button1.Enabled = true;
                        textBox1.Enabled = true;
                        checkBox1.Enabled = true;
                    }
                    else
                    {
                        button1.Enabled = false;
                        textBox1.Enabled = false;
                        checkBox1.Enabled = false;
                    }
                });
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (webView.Address == "https://discord.com/login")
            {
                var script = File.ReadAllText("script.js");
                script = script.Replace("TOKEN", token);

                try
                {
                    webView.ExecuteScriptAsync(script);
                    webView.Refresh();

                    button1.Enabled = false;
                    textBox1.Enabled = false;
                    checkBox1.Enabled = false;

                    MessageBox.Show($"Successfully logged in using the token.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was an error while executing the script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }           
            }
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            token = textBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = !checkBox1.Checked;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            timer.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
