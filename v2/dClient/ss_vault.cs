using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dClient
{
    public partial class ss_vault : Form
    {
        private Region _old;
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
        public static int qTogMove;
        public static int qMValX;
        public static int qMValY;
        private bool _resize = false;
        public ss_vault()
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

        private void ss_vault_Load(object sender, EventArgs e)
        {
            _old = this.Region;
            round_corners(new Control[] { this, button1, button2, button3, button4, button5, button6 });
        }
        private void round_corners(Control[] Controls)
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

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            string[] d = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
            label5.Text = $"Directory: {folderBrowserDialog1.SelectedPath}";
            foreach (string s in d)
            {
                listBox1.Items.Add(s.Replace(folderBrowserDialog1.SelectedPath, ""));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int pos = listBox1.SelectedIndex;

                pictureBox9.Image = new Bitmap(
                    new MemoryStream(File.ReadAllBytes(folderBrowserDialog1.SelectedPath + listBox1.Items[pos].ToString()))
                );

                numericUpDown1.Value = pictureBox9.Image.Width;
                numericUpDown2.Value = pictureBox9.Image.Height;
            } catch{}
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) { qTogMove = 1; qMValX = e.X; qMValY = e.Y; }
        private void panel1_MouseMove(object sender, MouseEventArgs e) { if (qTogMove == 1) { base.SetDesktopLocation(Control.MousePosition.X - qMValX, Control.MousePosition.Y - qMValY); } }
        private void panel1_MouseUp(object sender, MouseEventArgs e) { qTogMove = 0; }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Size size = pictureBox9.Size;
            Point pos = pictureBox9.Location;

            pictureBox9.Dispose();
            pictureBox9 = new PictureBox();
            this.Controls.Add(pictureBox9);
            pictureBox9.Size = size;
            pictureBox9.Location = pos;
            pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox9.DoubleClick += pictureBox9_DoubleClick;
        }

        private void pictureBox9_DoubleClick(object sender, EventArgs e)
        {
            string path = listBox1.SelectedItem.ToString();

            DataObject data = new DataObject();
            data.SetData(DataFormats.FileDrop, new string[] { label5.Text.Replace("Directory: ", "") + path });

            Clipboard.SetDataObject(data);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this._resize) // On
                {
                    this._resize = false;
                    this.Region = _old;
                }
                else
                {
                    this._resize = true;
                    this.round_corners(new Control[] { this });
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                pictureBox9.Load(textBox1.Text);
                numericUpDown1.Value = pictureBox9.Image.Width;
                numericUpDown2.Value = pictureBox9.Image.Height;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            pictureBox9.Image = new Bitmap(pictureBox9.Image, Convert.ToInt32(numericUpDown1.Value.ToString()), Convert.ToInt32(numericUpDown2.Value.ToString()));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pictureBox9.Image = new Bitmap(pictureBox9.Image, Convert.ToInt32(numericUpDown1.Value.ToString()), Convert.ToInt32(numericUpDown2.Value.ToString()));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox9.Image);

            Color background = bmp.GetPixel(0, 0);

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y) == background)
                        bmp.SetPixel(x, y, Color.Transparent);
                }
            }

            pictureBox9.Image = bmp;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox9.Image);

            bmp.MakeTransparent(bmp.GetPixel(0, 0));

            pictureBox9.Image = bmp;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string path = listBox1.SelectedItem.ToString();
            File.Delete(label5.Text.Replace("Directory: ", "") + path);
            pictureBox9.Image.Save("output.png", ImageFormat.Png);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            string path = listBox1.SelectedItem.ToString();

            DataObject data = new DataObject();
            data.SetData(DataFormats.FileDrop, new string[] { label5.Text.Replace("Directory: ", "") + path });

            Clipboard.SetDataObject(data);
        }
    }
}
