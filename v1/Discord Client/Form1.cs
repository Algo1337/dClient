using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Discord_Client
{
    public partial class Form1 : Form
    {
        Panel tools = new Panel();
        Region _old;
        public static int TogMove;
        public static int MValX;
        public static int MValY;
        TextBox search = new TextBox();
        WebView2 w = new WebView2();
        Label results = new Label();

        Label rdr = new Label();

        public FormRecorder recorder;

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        private const int resizeBorder = 8; // thickness in pixels

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        const int RIGHT_MOUSEEVENTF_RIGHTDOWN = 0x08;
        const int RIGHT_MOUSEEVENTF_RIGHTUP = 0x10;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Load("https://cdn.prod.website-files.com/6257adef93867e50d84d30e2/62fddf0fde45a8baedcc7ee5_847541504914fd33810e70a0ea73177e%20(2)-1.png");
            this.Controls.Add(this.w);
            this.w.Location = new Point(0, panel1.Height);
            this.w.Size = new Size(this.Width, this.Height - panel1.Height);

            this.Controls.Add(this.tools);
            this.tools.Location = new Point(0, panel1.Height);
            this.tools.Size = new Size(200, this.Height - panel1.Height);
            this.tools.BackColor = Color.Black;

            Panel b = new Panel();
            Label s = new Label();
            TextBox t = this.search;
            Label title = new Label();
            b.Cursor = Cursors.Hand;
            this.tools.Controls.Add(title);
            title.Text = "Spam Report A User Visible\r\nOn Screen";

            this.results.Location = new Point(5, 120);
            this.results.Text = "Report Count: N/A";
            this.results.Font = new Font("Ariel", 10);
            this.results.ForeColor = Color.White;
            this.results.Size = new Size(this.tools.Size.Width - 10, 20);
            this.tools.Controls.Add(this.results);

            b.Click += get_position;
            s.Click += get_position;

            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Font = new Font("Ariel", 10);
            title.Location = new Point(5, 10);
            title.Size = new Size(this.tools.Size.Width, 40);
            title.ForeColor = Color.White; 

            t.Location = new Point(5, 55);
            t.Size = new Size(this.tools.Size.Width - 10, 50);
            this.tools.Controls.Add(t);
            t.BorderStyle = BorderStyle.None;
            t.Font = new Font("Ariel", 12);

            b.Location = new Point(5, 80);
            b.Size = new Size(this.tools.Size.Width - 10, 30);
            s.Text = "Report Spam";
            b.Controls.Add(s);
            s.TextAlign = ContentAlignment.MiddleCenter;
            s.Dock = DockStyle.Fill;
            this.tools.Controls.Add(b);
            b.BackColor = Color.Gray;



            setup_browser();
            _old = this.Region;
            this.round_corners(new Control[] { this , b, t});

            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            panel1.MouseUp += panel1_MouseUp;

            panel1.BringToFront();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;

            if (m.Msg == WM_NCHITTEST)
            {
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));

                int grip = 8;

                bool left = pos.X <= grip;
                bool right = pos.X >= ClientSize.Width - grip;
                bool top = pos.Y <= grip;
                bool bottom = pos.Y >= ClientSize.Height - grip;

                if (left && top)
                    m.Result = (IntPtr)HTTOPLEFT;
                else if (right && top)
                    m.Result = (IntPtr)HTTOPRIGHT;
                else if (left && bottom)
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (right && bottom)
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (left)
                    m.Result = (IntPtr)HTLEFT;
                else if (right)
                    m.Result = (IntPtr)HTRIGHT;
                else if (top)
                    m.Result = (IntPtr)HTTOP;
                else if (bottom)
                    m.Result = (IntPtr)HTBOTTOM;
                else
                    m.Result = (IntPtr)1; // HTCLIENT

                return;
            }

            base.WndProc(ref m);
        }


        public async void setup_browser()
        {
            await this.w.EnsureCoreWebView2Async();
            this.w.CoreWebView2.Navigate("https://discord.com/channels/@me");
        }

        public void round_corners(Control[] Controls)
        {
            foreach (Control obj in Controls)
            {
                string type = obj.GetType().ToString().ToLower();
                int d = 10;
                if (obj.GetType().ToString() == "SWX.Form1")
                {
                    d = 30;
                }

                GraphicsPath gay = new GraphicsPath();
                gay.AddArc(0, 0, d, d, 180, 90);
                gay.AddArc(obj.Width - d, 0, d, d, 270, 90);
                gay.AddArc(obj.Width - d, obj.Height - d, d, d, 0, 90);
                gay.AddArc(0, obj.Height - d, d, d, 90, 90);
                gay.CloseAllFigures();
                obj.Region = new Region(gay);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) { TogMove = 1; MValX = e.X; MValY = e.Y; }
        private void panel1_MouseMove(object sender, MouseEventArgs e) { if (TogMove == 1) { base.SetDesktopLocation(Control.MousePosition.X - MValX, Control.MousePosition.Y - MValY); } }
        private void panel1_MouseUp(object sender, MouseEventArgs e) { TogMove = 0; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Region = this._old;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (label3.Text.Contains("[ - ]"))
            {
                label3.Text = label3.Text.Replace("[ - ]", "[ + ]");
                this.Region = _old;
                this.w.Location = new Point(5, panel1.Height);
                this.w.Size = new Size(this.Width - 10, this.Height - panel1.Height - 5);
            } else
            {
                label3.Text = label3.Text.Replace("[ + ]", "[ - ]");
                this.round_corners(new Control[] { this });
                this.w.Location = new Point(0, panel1.Height);
                this.w.Size = new Size(this.Width, this.Height - panel1.Height);
            }
        }

        private async void label4_Click(object sender, EventArgs e)
        {
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MyApp",
                    Guid.NewGuid().ToString()
                )
            );
            var p = new WebView2();
            await p.EnsureCoreWebView2Async(env);

            p.CoreWebView2.Navigate("https://discord.com/channels/@me");
            this.Controls.Add(p);
        }
        private async void label4_Click_1(object sender, EventArgs e)
        {
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MyApp",
                    Guid.NewGuid().ToString()
                )
            );

            this.w?.Dispose();
            this.w = new WebView2();
            this.w.Location = new Point(0, panel1.Height);
            this.w.Size = new Size(this.Width, this.Height - panel1.Height);

            this.Controls.Add(this.w);

            await this.w.EnsureCoreWebView2Async(env);

            this.w.CoreWebView2.Navigate("https://discord.com/channels/@me");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if(label5.Text.Contains("[ - ]"))
            {
                label5.Text = label5.Text.Replace("[ - ]", "[ + ]");
                this.w.Location = new Point(this.tools.Width, panel1.Height);
                this.w.Size = new Size(this.Width - this.tools.Width, this.Height - this.panel1.Height);
                this.tools.Size = new Size(200, this.Height - panel1.Height);
                this.tools.Visible = true;
            } else
            {
                label5.Text = label5.Text.Replace("[ + ]", "[ - ]");
                this.w.Location = new Point(0, panel1.Height);
                this.w.Size = new Size(this.Width, this.Height - this.panel1.Height);
                this.tools.Visible = false;
            }
        }

        public Point? find_text(string query)
        {
            var root = AutomationElement.RootElement;
            var condition = new PropertyCondition(AutomationElement.NameProperty, query);

            var element = root.FindFirst(TreeScope.Descendants, condition);
            if (element != null)
            {
                var rect = element.Current.BoundingRectangle;
                int centerX = (int)(rect.X + rect.Width / 2);
                int centerY = (int)(rect.Y + rect.Height / 2);

                return new Point(centerX, centerY);

            }

            return null;
        }

        private async void get_position(object sender, EventArgs e)
        {
            string[] queries = new string[]
            {
                this.search.Text,
                "Report Message",
                "Harmful misinformation or glorifying violence",
                "Spreading misinformation or conspiracy theories",
                "They are saying bad things about me or someone I know",
                "Submit Report",
                "Done"
            };

            for (int c = 0; c < 5; c++)
            {
                int i = 0;
                foreach (string s in queries)
                {
                    Point? p = find_text(s);
                    if (p == null)
                    {
                        break;
                    }

                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point(((Point)p).X, ((Point)p).Y);
                    this.w.Focus();
                    await this.w.CoreWebView2.ExecuteScriptAsync("window.focus();");
                    await Task.Delay(500);

                    if (i == 0)
                    {
                        mouse_event(RIGHT_MOUSEEVENTF_RIGHTDOWN, ((Point)p).X + 1, 0, 0, 0);
                        mouse_event(RIGHT_MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    }
                    else
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, ((Point)p).X + 1, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    }

                    await Task.Delay(500);

                    i++;
                }

                this.results.Text = $"Report Count: {c}";
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
