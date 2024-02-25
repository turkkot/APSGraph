using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test3
{
    public partial class VibrationSettingsView : Form
    {
        private int standartValue1, standartValue2, standartValue3;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem returnStandartValuesMenuItem;
        public VibrationSettingsView(bool isLightTheme, int permissibleVibrationLevel, int highVibrationLevel, int criticalVibrationLevel)
        {
            InitializeComponent();
            standartValue1 = 50;
            standartValue2 = 75;
            standartValue3 = 105;
            numericUpDown1.Value = permissibleVibrationLevel;
            numericUpDown2.Value = highVibrationLevel;
            numericUpDown3.Value = criticalVibrationLevel;
            contextMenuStrip1 = new ContextMenuStrip();
            returnStandartValuesMenuItem = new ToolStripMenuItem();
            returnStandartValuesMenuItem.Text = "Вернуть значения по умолчанию";
            contextMenuStrip1.Items.Add(returnStandartValuesMenuItem);
            returnStandartValuesMenuItem.Click += ReturnStandartValuesMenuItem_Click;
            this.ContextMenuStrip = contextMenuStrip1;
            if (!isLightTheme)
            {
                this.BackColor = Color.FromArgb(27, 33, 56);
                label1.ForeColor = Color.FromName("ScrollBar");
                label2.ForeColor = Color.FromName("ScrollBar");
                label3.ForeColor = Color.FromName("ScrollBar");
            }
        }

        private void ReturnStandartValuesMenuItem_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = standartValue1;
            numericUpDown2.Value = standartValue2;
            numericUpDown3.Value = standartValue3;
        }
    }
}
