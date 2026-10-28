using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dClient
{
    public partial class tenor : Form
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
        public tenor()
        {
            InitializeComponent();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) { TogMove = 1; MValX = e.X; MValY = e.Y; }
        private void panel1_MouseMove(object sender, MouseEventArgs e) { if (TogMove == 1) { base.SetDesktopLocation(Control.MousePosition.X - MValX, Control.MousePosition.Y - MValY); } }
        private void panel1_MouseUp(object sender, MouseEventArgs e) { TogMove = 0; }

        private void tenor_Load(object sender, EventArgs e)
        {
            setup_browser();
        }
        async void setup_browser()
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Navigate("https://tenor.com");
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
