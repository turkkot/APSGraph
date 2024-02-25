using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test3
{
    public partial class ColorSettingsView : Form
    {
        private Button[] buttons;
        private Button tmpButton;
        private Label[] labels;
        private Label tmpLabel;
        private ColorDialog colorDialog;
        public Color[] colors;
        public Color[] defaultColors;
        public Color specialColor;
        public readonly Color defaultSpecialColor = Color.Gray;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem changeColorsToDefaults;
        private ToolStripMenuItem specialColorMenu;
        public bool isF3Opened;

        public ColorSettingsView(bool isLightTheme, Color[] colors, Color specialColor)
        {
            // Инициализация стандартных цветов
            defaultColors = new Color[44];
            defaultColors[0] = Color.FromArgb(255, 0, 0); // Красный
            defaultColors[1] = Color.FromArgb(255, 165, 0); // Оранжевый
            defaultColors[2] = Color.FromArgb(255, 218, 185); // Персиковый
            defaultColors[3] = Color.FromArgb(0, 128, 0); // Зеленый
            defaultColors[4] = Color.FromArgb(100, 243, 255); // Голубой
            defaultColors[5] = Color.FromArgb(0, 0, 255); // Синий
            defaultColors[6] = Color.FromArgb(128, 0, 128); // Фиолетовый
            defaultColors[7] = Color.FromArgb(255, 69, 115); // Розовый
            defaultColors[8] = Color.FromArgb(162, 42, 42); // Коричневый
            defaultColors[9] = Color.FromArgb(177, 255, 91); // Toxic
            defaultColors[10] = Color.FromArgb(75, 0, 30); // Индиго
            defaultColors[11] = Color.FromArgb(220, 20, 60); // Малиновый
            defaultColors[12] = Color.FromArgb(0, 0, 139); // Темно-синий
            defaultColors[13] = Color.FromArgb(128, 128, 0); // Оливковый
            defaultColors[14] = Color.FromArgb(245, 245, 220); // Бежевый
            defaultColors[15] = Color.FromArgb(255, 255, 51); // Ярко-желтый
            defaultColors[16] = Color.FromArgb(128, 0, 0); // Бордовый
            defaultColors[17] = Color.FromArgb(153, 50, 204); // Лиловый
            defaultColors[18] = Color.FromArgb(218, 165, 32); // Горчичный
            defaultColors[19] = Color.FromArgb(60, 179, 113); // Цвет морской воды
            defaultColors[20] = Color.FromArgb(255, 0, 63); // Ярко-красный
            defaultColors[21] = Color.FromArgb(255, 140, 0); // Ярко-оранжевый
            defaultColors[22] = Color.FromArgb(0, 255, 0); // Ярко-зеленый
            defaultColors[23] = Color.FromArgb(154, 205, 50); // Желто-зеленый
            defaultColors[24] = Color.FromArgb(255, 127, 80); // Кораловый
            defaultColors[25] = Color.FromArgb(148, 0, 211); // Темно фиолетовый
            defaultColors[26] = Color.FromArgb(240, 230, 140); // Хаки
            defaultColors[27] = Color.FromArgb(152, 255, 152); // Мятный
            defaultColors[28] = Color.FromArgb(255, 253, 208); // Кремовый
            defaultColors[29] = Color.FromArgb(64, 224, 208); // Бирюзовый
            defaultColors[30] = Color.FromArgb(221, 160, 221); // Сливовый
            defaultColors[31] = Color.FromArgb(211, 211, 211); //  Светло-сервый
            defaultColors[32] = Color.FromArgb(135, 206, 250); // Светло-Голубой
            defaultColors[33] = Color.FromArgb(144, 238, 144); // Светло-зеленый
            defaultColors[34] = Color.FromArgb(255, 255, 224); // Ярко-желтый
            defaultColors[35] = Color.FromArgb(255, 255, 0); // Желтый
            defaultColors[36] = Color.FromArgb(139, 134, 104); // Персиковый
            defaultColors[37] = Color.FromArgb(70, 130, 180); // Стальной
            defaultColors[38] = Color.FromArgb(255, 224, 189); // Телесный
            defaultColors[39] = Color.FromArgb(222, 49, 99); // Черри
            defaultColors[40] = Color.FromArgb(255, 215, 0); // Медовый
            defaultColors[41] = Color.FromArgb(220, 210, 220); // Телесно-розовый
            defaultColors[42] = Color.FromArgb(200, 234, 180); // Теплый зеленый
            defaultColors[43] = Color.FromArgb(200, 120, 130); // Теплый коралловый

            //Инициализация всплывающегося меню
            contextMenuStrip = new();
            changeColorsToDefaults = new("Вернуть стандартные цвета");
            specialColorMenu = new("Специальный цвет");
            contextMenuStrip.Items.Add(changeColorsToDefaults);
            contextMenuStrip.Items.Add(specialColorMenu);
            changeColorsToDefaults.Click += ChangeColorsToDefaults_Click;
            specialColorMenu.Click += SpecialColorMenu_Click;
            this.ContextMenuStrip = contextMenuStrip;

            // Стартовые настройки формы
            this.colors = new Color[44];
            Array.Copy(Program.f1.colors, this.colors, colors.Length);
            buttons = new Button[colors.Length];
            labels = new Label[colors.Length];
            for (byte i = 0; i < colors.Length; i++)
            {

                tmpLabel = new Label();
                tmpLabel.Text = Convert.ToString(i + 1) + " " + "Graph";
                tmpLabel.Font = new Font("Tobota", 8, FontStyle.Regular);
                tmpLabel.Size = new Size(56, 15);
                tmpButton = new Button();
                tmpButton.BackColor = colors[i];
                tmpButton.Text = " ";
                tmpButton.Size = new Size(75, 23);
                if (i < 22)
                {
                    tmpLabel.Location = new Point(12, 8 + i * 29);
                    tmpButton.Location = new Point(89, 5 + i * 29);
                }
                else
                {
                    tmpLabel.Location = new Point(185, 8 + (i - 22) * 29);
                    tmpButton.Location = new Point(262, 5 + (i - 22) * 29);
                }
                labels[i] = tmpLabel;
                buttons[i] = tmpButton;
                buttons[i].Tag = i;
                buttons[i].Click += ChangeColorButton_Click;
                this.Controls.Add(labels[i]);
                this.Controls.Add(buttons[i]);
            }
            colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            InitializeComponent();

            if (!isLightTheme)
            {
                this.BackColor = Color.FromArgb(27, 33, 56);
                for (byte i = 0; i < labels.Length; i++)
                {
                    labels[i].ForeColor = Color.FromName("ScrollBar");
                }
            }

            this.specialColor = specialColor;
        }

        private void SpecialColorMenu_Click(object? sender, EventArgs e)
        {
            colorDialog.Color = specialColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                specialColor = colorDialog.Color;
                foreach (var item in Program.f1.arrayOfGraphWindows)
                    item.changePlottablesColor(this.colors, Program.f1.labelsText);
            }

        }

        private void ChangeColorsToDefaults_Click(object? sender, EventArgs e)
        {
            colors = defaultColors;
            specialColor = defaultSpecialColor;
            for (byte i = 0; i < buttons.Length; i++)
                buttons[i].BackColor = colors[i];
            foreach (var item in Program.f1.arrayOfGraphWindows)
                item.changePlottablesColor(colors, Program.f1.labelsText);
        }

        private void ChangeColorButton_Click(object? sender, EventArgs e)
        {
            Button btn = (Button)sender;
            colorDialog.Color = btn.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                btn.BackColor = colorDialog.Color;
                this.colors[Convert.ToByte(btn.Tag)] = colorDialog.Color;
                foreach (var item in Program.f1.arrayOfGraphWindows)
                    item.changePlottablesColor(this.colors, Program.f1.labelsText);
                
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            isF3Opened = false;
            bool isChanged = false;
            for (byte i = 0; i < colors.Length; i++)
                if (colors[i] != Program.f1.colors[i])
                    isChanged = true;

            if (isChanged)
            {
                var res = MessageBox.Show("Сохранить настройки цветов?", "Настройка цветов", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (res == DialogResult.Yes)
                {
                    Program.f1.colors = this.colors;
                    Program.f1.specialColor = this.specialColor;
                    foreach (var item in Program.f1.arrayOfGraphWindows)
                        item.changePlottablesColor(colors, Program.f1.labelsText);
                }
                else if (res == DialogResult.No)
                {
                    foreach (var item in Program.f1.arrayOfGraphWindows)
                        item.changePlottablesColor(Program.f1.colors, Program.f1.labelsText);
                }
                else
                {
                    e.Cancel = true;

                }
            }

        }
    }
}
