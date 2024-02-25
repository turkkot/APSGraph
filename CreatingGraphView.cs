using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test3
{
    public partial class CreatingGraphsView : Form
    {
        private int index;
        private bool isLightTheme;
        private Color[] colors;
        private GraphSettingPanel[] arrayOfGraphSettingPanels;
        private ScottPlot.FormsPlot formsPlot;
        public CreatingGraphsView(bool isLightTheme, Color[] colors)
        {
            InitializeComponent();
            this.isLightTheme = isLightTheme;
            this.colors = colors;
            index = -1;
            arrayOfGraphSettingPanels = Array.Empty<GraphSettingPanel>();
            comboBox1.Items.Add("Задать график функции");
            comboBox1.Items.Add("Задать график по точкам");
            formsPlot = new ScottPlot.FormsPlot();
            formsPlot.Size = new Size(782, 558);
            formsPlot.Location = new Point(4, 0);
            splitContainer1.Panel2.Controls.Add(formsPlot);
            comboBox1.SelectedIndex = 0;
            if (!isLightTheme)
                formsPlot.Plot.Style(Style.Blue2);
            else
            {
                addButton.BackColor = Color.FromName("ButtonFace");
                addButton.ForeColor = Color.Black;
                this.BackColor = Color.FromName("Window");
                formsPlot.Plot.Style(Style.Default);
            }
            formsPlot.Refresh();
        }

        private class GraphSettingPanel
        {
            private FlowLayoutPanel flowLayoutPanel;
            private Panel panel;
            private TextBox graphNameTextBox;
            private TextBox functionTextBox;
            private DataGridView dataTable;
            private Panel colorPanel;
            public CheckBox isOnThePlotCheckBox;
            public Button deleteButton;
            public byte typeOfGraphPanel;
            public int index;

            private Color[] arrayOfColors;

            public GraphSettingPanel(byte typeOfGraphPanel, Color[] arrayOfColors, int index, FlowLayoutPanel flowLayoutPanel)
            {
                this.typeOfGraphPanel = typeOfGraphPanel;
                this.arrayOfColors = arrayOfColors;
                this.index = index;
                this.flowLayoutPanel = flowLayoutPanel;
            }

            public void createPanel()
            {
                panel = new Panel();
                colorPanel = new Panel();
                panel.BorderStyle = BorderStyle.FixedSingle;
                graphNameTextBox = new TextBox();
                graphNameTextBox.BorderStyle = BorderStyle.None;
                graphNameTextBox.Location = new Point(27, 11);
                graphNameTextBox.Size = new Size(263, 23);
                graphNameTextBox.PlaceholderText = "Введите название";
                colorPanel.Location = new Point(296, 15);
                colorPanel.Size = new Size(16, 11);
                colorPanel.BackColor = arrayOfColors[index];
                isOnThePlotCheckBox = new CheckBox();
                isOnThePlotCheckBox.Tag = index;
                deleteButton = new Button
                {
                    BackColor = Color.Red,
                    Font = new Font("Segoe UI Black", 7F, FontStyle.Bold, GraphicsUnit.Point),
                    ForeColor = Color.White,
                    Text = "X",
                    UseVisualStyleBackColor = false,
                    Size = new Size(22, 19),
                    Location = new Point(320, 4)
                };
                deleteButton.BringToFront();
                deleteButton.Tag = index;

                if (typeOfGraphPanel == 0)
                {
                    functionTextBox = new TextBox();
                    functionTextBox.PlaceholderText = "Введите функцию";
                    functionTextBox.Size = new Size(313, 23);
                    functionTextBox.Location = new Point(24, 40);
                    panel.Size = new Size(350, 68);
                    isOnThePlotCheckBox.Location = new Point(3, 44);
                    panel.Controls.Add(functionTextBox);
                }
                else if (typeOfGraphPanel == 1)
                {
                    dataTable = new DataGridView();
                    dataTable.Size = new Size(263, 83);
                    dataTable.Location = new Point(52, 33);
                    panel.Size = new Size(350, 130);
                    panel.Controls.Add(dataTable);
                    isOnThePlotCheckBox.Location = new Point(3, 64);
                }

                panel.Controls.Add(graphNameTextBox);
                panel.Controls.Add(colorPanel);
                panel.Controls.Add(isOnThePlotCheckBox);
                panel.Controls.Add(deleteButton);
                flowLayoutPanel.Controls.Add(panel);
            }

            public void Dispose()
            {
                if (panel != null)
                    panel.Dispose();
                if (graphNameTextBox != null)
                    graphNameTextBox.Dispose();
                if (functionTextBox != null)
                    functionTextBox.Dispose();
                if (colorPanel != null)
                    colorPanel.Dispose();
                if (isOnThePlotCheckBox != null)
                    isOnThePlotCheckBox.Dispose();
                if (dataTable != null)
                    dataTable.Dispose();
            }


        }

        private void addButton_Click(object sender, EventArgs e)
        {
            byte comboIndex = (byte)comboBox1.SelectedIndex;
            index++;
            if (index < colors.Length)
            {
                GraphSettingPanel graphSettingPanel = new GraphSettingPanel(comboIndex, colors, index, flowLayoutPanel1);
                graphSettingPanel.createPanel();
                graphSettingPanel.deleteButton.Click += DeleteButton_Click;
                graphSettingPanel.isOnThePlotCheckBox.CheckedChanged += IsOnThePlotCheckBox_CheckedChanged;
                Array.Resize(ref arrayOfGraphSettingPanels, arrayOfGraphSettingPanels.Length + 1);
                arrayOfGraphSettingPanels[^1] = graphSettingPanel;
            }
            else
            {
                index--;
                MessageBox.Show("Достигнуто максимальное значение созданных графиков", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void IsOnThePlotCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            int tag = Convert.ToInt32(checkBox.Tag);
            if (checkBox.Checked)
            {

            }
            else
            {

            }
        }

        private void discoverLine(string line)
        {
            byte funcType;
            bool isLeftPartIsOK = false;
            string[] dividedLine = line.Split(new[] { '=' });
            if (dividedLine[0] == "f(x)" || dividedLine[0] == "y(x)" || dividedLine[0] == "y")
                isLeftPartIsOK = true;
            string rightPart = dividedLine[1];
            // 3*x*sin(2*x)/(2+3*x+x)




        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int tag = Convert.ToInt32(btn.Tag);

            arrayOfGraphSettingPanels[tag].Dispose();
            for (int i = tag; i < arrayOfGraphSettingPanels.Length - 1; i++)
                arrayOfGraphSettingPanels[i] = arrayOfGraphSettingPanels[i + 1];
            Array.Resize(ref arrayOfGraphSettingPanels, arrayOfGraphSettingPanels.Length - 1);
            for (int i = 0; i < arrayOfGraphSettingPanels.Length; i++)
            {
                arrayOfGraphSettingPanels[i].index = i;
                arrayOfGraphSettingPanels[i].deleteButton.Tag = i;
            }
            index--;
        }
    }
}
