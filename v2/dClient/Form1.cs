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
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace dClient
{
    public partial class Form1 : Form
    {

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
        public static int TogMove;
        public static int MValX;
        public static int MValY;

        public Region _old;
        private bool _resize = false;

        public Recorder recorder;

        public Size ClosedSz = new Size(43, 577);
        public Size OpenedSz = new Size(250, 577);

        public List<WebView2> Sessions = new List<WebView2>();
        public Form1()
        {
            InitializeComponent();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Sessions.Add(new WebView2());
            listBox1.Items.Add("Session");
            listBox1.SelectedIndex = 0;

            pictureBox1.Load("https://cdn.prod.website-files.com/6257adef93867e50d84d30e2/62fddf0fde45a8baedcc7ee5_847541504914fd33810e70a0ea73177e%20(2)-1.png");
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            IntPtr ico = bmp.GetHicon();
            this.Icon = Icon.FromHandle(ico);

            setup_browser();

            this.Region = _old;
            round_corners(new Control[] { this, button1, button2, button3, button4 });

            panel2.Size = ClosedSz;
            add_settings(this.Sessions[0]);
        }

        public WebView2 get_current_session()
        {
            return this.Sessions[listBox1.SelectedIndex];
        }

        public void add_settings(WebView2 c)
        {
            this.Controls.Add(c);
            c.Location = new Point(panel2.Width, panel1.Height);
            c.Size = new Size(this.Size.Width - panel2.Size.Width, this.Size.Height - panel1.Size.Height);
        }
        private void round_corners(Control[] Controls)
        {
            foreach (Control obj in Controls)
            {
                string type = obj.GetType().ToString().ToLower();
                int d = 15;

                GraphicsPath gay = new GraphicsPath();
                gay.AddArc(0, 0, d, d, 180, 90);
                gay.AddArc(obj.Width - d, 0, d, d, 270, 90);
                gay.AddArc(obj.Width - d, obj.Height - d, d, d, 0, 90);
                gay.AddArc(0, obj.Height - d, d, d, 90, 90);
                gay.CloseAllFigures();
                obj.Region = new Region(gay);
            }
        }

        async void setup_browser()
        {
            if (listBox1.Items.Count == 0)
            {
                return;
            }

            WebView2 c = get_current_session();
            await c.EnsureCoreWebView2Async();
            c.CoreWebView2.Navigate("https://discord.com/channels/@me");
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e) { TogMove = 1; MValX = e.X; MValY = e.Y; }
        private void panel1_MouseMove(object sender, MouseEventArgs e) { if (TogMove == 1) { base.SetDesktopLocation(Control.MousePosition.X - MValX, Control.MousePosition.Y - MValY); } }
        private void panel1_MouseUp(object sender, MouseEventArgs e) { TogMove = 0; }

        public Panel get_nav_bar()
        {
            return panel1;
        }
        public WebView2 get_webview()
        {
            return get_current_session();
        }

        public void change_size(Size s)
        {
            foreach (WebView2 c in this.Sessions)
            {
                c.Size = s;
            }
        }
        public void change_location(Point s)
        {
            foreach (WebView2 c in this.Sessions)
            {
                c.Location = s;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            WebView2 c = get_current_session();
            if (panel2.Size == ClosedSz)
            {
                panel9.Visible = true;
                panel5.Visible = true;
                panel10.Visible = true;
                panel2.Size = OpenedSz;
                c.Location = new Point(panel2.Width, panel1.Height);
                c.Size = new Size(this.Width - panel2.Width, this.Height - panel1.Height);
            }
            else
            {
                panel9.Visible = false;
                panel5.Visible = false;
                panel10.Visible = false;
                panel2.Size = ClosedSz;
                c.Location = new Point(panel2.Width, panel1.Height);
                c.Size = new Size(this.Width - panel2.Width, this.Height - panel1.Height);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            WebView2 c = get_current_session();
            if (checkBox1.Checked)
            {
                this.Region = this._old;
                change_location(new Point(panel2.Width + 10, panel1.Height));
                change_size(new Size(this.Size.Width - panel2.Width - 20, this.Size.Height - panel1.Height - 10));
            }
            else
            {
                change_location(new Point(panel2.Width, panel1.Height));
                change_size(new Size(this.Size.Width - panel2.Width, this.Size.Height - panel1.Height));
                this.ClosedSz = new Size(43, this.Size.Height);
                this.OpenedSz = new Size(250, this.Size.Height);
                panel2.Size = this.OpenedSz;
                this.round_corners(new Control[] { this });
                panel5.Location = new Point(panel5.Location.X, this.Height - panel5.Size.Height - 80);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ss_vault s = new ss_vault();
            s.Show();
        }

        public async void setup_private_browser(WebView2 c)
        {
            string userDataFolder = Path.Combine(
                Path.GetTempPath(),
                "MyApp",
                Guid.NewGuid().ToString()
            );

            var environment = await CoreWebView2Environment.CreateAsync(
                null,
                userDataFolder
            );

            await c.EnsureCoreWebView2Async(environment);
            c.CoreWebView2.Navigate("https://discord.com/login");
        }

        private void label7_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add($"Session");
            this.Sessions.Add(new WebView2());
            setup_private_browser(this.Sessions[listBox1.Items.Count - 1]);
            add_settings(this.Sessions[listBox1.Items.Count - 1]);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index == 0) return;
            listBox1.SelectedIndex = listBox1.Items.Count - 1;


            this.Sessions[index].Dispose();
            this.Sessions.RemoveAt(index);
            listBox1.Items.RemoveAt(index);
            MessageBox.Show($"Deleted {index}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tenor t = new tenor();
            t.Show();
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            WebView2 c = get_current_session();
            if (c == null) return;
            c.BringToFront();
        }
    }
}
