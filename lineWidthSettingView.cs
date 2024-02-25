using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace test3
{
    public partial class lineWidthSettingView : Form
    {
        private bool isLightTheme;
        public float lineWidth;
        Graphics g;
        Bitmap bitmap;
        Pen pen;

        public lineWidthSettingView(float lineWidth, bool isLightTheme)
        {
            this.lineWidth = lineWidth;
            this.isLightTheme = isLightTheme;
            bitmap = new Bitmap(125, 48, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(bitmap);
            InitializeComponent();
            numericUpDown1.Value = Convert.ToDecimal(lineWidth);

            if (isLightTheme)
            {
                this.BackColor = SystemColors.Window;
                g.Clear(Color.FromName("Window"));
                pen = new Pen(Color.Black, lineWidth);
            }
            else
            {
                this.BackColor = Color.FromArgb(27, 33, 56);
                g.Clear(Color.FromArgb(27, 33, 56));
                pen = new Pen(Color.White, lineWidth);
            }
            g.DrawLine(pen, new Point(2, 20), new Point(150, 20));
            pictureBox1.Image = bitmap;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            lineWidth = (int)numericUpDown1.Value;

            if (this.isLightTheme)
            {
                g.Clear(Color.FromName("Window"));
                pen = new Pen(Color.Black, lineWidth);

            }
            else
            {
                g.Clear(Color.FromArgb(27, 33, 56));
                pen = new Pen(Color.White, lineWidth);
            }
            g.DrawLine(pen, new Point(2, 20), new Point(150, 20));
            pictureBox1.Image = bitmap;
        }
    }


}
