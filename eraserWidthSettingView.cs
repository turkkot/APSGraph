using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test3
{
    public partial class eraserWidthSettingView : Form
    {
        public eraserWidthSettingView(double eraserWidth)
        {
            InitializeComponent();
            numericUpDown1.Value = Convert.ToInt32(eraserWidth * 10);
        }
    }
}
