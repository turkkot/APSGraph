namespace test3
{
    partial class MainView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>

        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            toolStrip1 = new ToolStrip();
            cursorButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            lineButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            verticalLineButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            eraserButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            slopeLineButton = new ToolStripButton();
            textBox1 = new TextBox();
            add_graphWindow_button = new Button();
            selectFile_button = new Button();
            progressBar1 = new ProgressBar();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            createToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            recentFilesФайлыToolStripMenuItem = new ToolStripMenuItem();
            parametersToolStripMenuItem = new ToolStripMenuItem();
            ViewToolStripMenuItem = new ToolStripMenuItem();
            ToolPanelToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            fAQToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Location = new Point(0, 27);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AllowDrop = true;
            splitContainer1.Panel2.Controls.Add(textBox1);
            splitContainer1.Panel2.Controls.Add(add_graphWindow_button);
            splitContainer1.Panel2.Controls.Add(selectFile_button);
            splitContainer1.Panel2.Controls.Add(progressBar1);
            splitContainer1.Panel2.Click += splitContainer1_Panel2_Click;
            splitContainer1.Size = new Size(1164, 647);
            splitContainer1.SplitterDistance = 892;
            splitContainer1.TabIndex = 0;
            splitContainer1.SplitterMoved += SplitContainer1_SplitterMoved;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Left;
            toolStrip1.Items.AddRange(new ToolStripItem[] { cursorButton, toolStripSeparator1, lineButton, toolStripSeparator4, verticalLineButton, toolStripSeparator2, slopeLineButton, toolStripSeparator3, eraserButton, toolStripSeparator5 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(32, 643);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // cursorButton
            // 
            cursorButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            cursorButton.Image = Properties.Resources.cursor;
            cursorButton.ImageTransparentColor = Color.Magenta;
            cursorButton.Name = "cursorButton";
            cursorButton.Size = new Size(29, 20);
            cursorButton.Text = "Курсор";
            cursorButton.Click += cursorButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(29, 6);
            // 
            // lineButton
            // 
            lineButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lineButton.Image = Properties.Resources.line;
            lineButton.ImageTransparentColor = Color.Magenta;
            lineButton.Name = "lineButton";
            lineButton.Size = new Size(29, 20);
            lineButton.Text = "Горизонтальная линия";
            lineButton.Click += lineButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(29, 6);
            // 
            // verticalLineButton
            // 
            verticalLineButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            verticalLineButton.Image = Properties.Resources.verticalLine;
            verticalLineButton.ImageTransparentColor = Color.Magenta;
            verticalLineButton.Name = "verticalLineButton";
            verticalLineButton.Size = new Size(29, 20);
            verticalLineButton.Text = "toolStripButton1";
            verticalLineButton.Click += verticalLineButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(29, 6);
            // 
            // eraserButton
            // 
            eraserButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            eraserButton.Image = Properties.Resources.eraser;
            eraserButton.ImageTransparentColor = Color.Magenta;
            eraserButton.Name = "eraserButton";
            eraserButton.Size = new Size(29, 20);
            eraserButton.Text = "Ластик";
            eraserButton.Click += eraserButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(29, 6);
            // 
            // slopeLineButton
            // 
            slopeLineButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            slopeLineButton.Image = Properties.Resources.slopeLine;
            slopeLineButton.ImageTransparentColor = Color.Magenta;
            slopeLineButton.Name = "slopeLineButton";
            slopeLineButton.Size = new Size(29, 20);
            slopeLineButton.Text = "toolStripButton1";
            slopeLineButton.Click += slopeLineButton_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(27, 33, 56);
            textBox1.ForeColor = SystemColors.ScrollBar;
            textBox1.Location = new Point(3, 415);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(258, 94);
            textBox1.TabIndex = 3;
            // 
            // add_graphWindow_button
            // 
            add_graphWindow_button.AutoSize = true;
            add_graphWindow_button.BackColor = Color.FromArgb(27, 33, 56);
            add_graphWindow_button.Font = new Font("Quicksand", 12F, FontStyle.Regular, GraphicsUnit.Point);
            add_graphWindow_button.ForeColor = SystemColors.ScrollBar;
            add_graphWindow_button.Location = new Point(3, 515);
            add_graphWindow_button.Name = "add_graphWindow_button";
            add_graphWindow_button.Size = new Size(258, 45);
            add_graphWindow_button.TabIndex = 2;
            add_graphWindow_button.Text = "Добавить окно графика";
            add_graphWindow_button.UseVisualStyleBackColor = false;
            add_graphWindow_button.Click += Add_graphWindow_button_Click;
            // 
            // selectFile_button
            // 
            selectFile_button.AutoSize = true;
            selectFile_button.BackColor = Color.FromArgb(27, 33, 56);
            selectFile_button.Font = new Font("Quicksand", 12F, FontStyle.Regular, GraphicsUnit.Point);
            selectFile_button.ForeColor = SystemColors.ScrollBar;
            selectFile_button.Location = new Point(3, 566);
            selectFile_button.Name = "selectFile_button";
            selectFile_button.Size = new Size(258, 45);
            selectFile_button.TabIndex = 1;
            selectFile_button.Text = "Выбрать файл";
            selectFile_button.UseVisualStyleBackColor = false;
            selectFile_button.Click += SelectFile_button_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(4, 617);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(256, 23);
            progressBar1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, parametersToolStripMenuItem, ViewToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1164, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createToolStripMenuItem, openToolStripMenuItem, recentFilesФайлыToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(48, 20);
            fileToolStripMenuItem.Text = "Файл";
            // 
            // createToolStripMenuItem
            // 
            createToolStripMenuItem.Name = "createToolStripMenuItem";
            createToolStripMenuItem.Size = new Size(176, 22);
            createToolStripMenuItem.Text = "Создать";
            createToolStripMenuItem.Click += createToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(176, 22);
            openToolStripMenuItem.Text = "Открыть";
            // 
            // recentFilesФайлыToolStripMenuItem
            // 
            recentFilesФайлыToolStripMenuItem.Name = "recentFilesФайлыToolStripMenuItem";
            recentFilesФайлыToolStripMenuItem.Size = new Size(176, 22);
            recentFilesФайлыToolStripMenuItem.Text = "Последние файлы";
            // 
            // parametersToolStripMenuItem
            // 
            parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            parametersToolStripMenuItem.Size = new Size(83, 20);
            parametersToolStripMenuItem.Text = "Параметры";
            // 
            // ViewToolStripMenuItem
            // 
            ViewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ToolPanelToolStripMenuItem });
            ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            ViewToolStripMenuItem.Size = new Size(39, 20);
            ViewToolStripMenuItem.Text = "Вид";
            // 
            // ToolPanelToolStripMenuItem
            // 
            ToolPanelToolStripMenuItem.CheckOnClick = true;
            ToolPanelToolStripMenuItem.Name = "ToolPanelToolStripMenuItem";
            ToolPanelToolStripMenuItem.Size = new Size(196, 22);
            ToolPanelToolStripMenuItem.Text = "Панель инструментов";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fAQToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(68, 20);
            helpToolStripMenuItem.Text = "Помощь";
            // 
            // fAQToolStripMenuItem
            // 
            fAQToolStripMenuItem.Name = "fAQToolStripMenuItem";
            fAQToolStripMenuItem.Size = new Size(96, 22);
            fAQToolStripMenuItem.Text = "FAQ";
            fAQToolStripMenuItem.Click += fAQToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(29, 6);
            // 
            // MainView
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(27, 33, 56);
            ClientSize = new Size(1164, 681);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            KeyPreview = true;
            Name = "MainView";
            Text = "APS Graphs";
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private Button add_graphWindow_button;
        private Button selectFile_button;
        private ProgressBar progressBar1;
        private TextBox textBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem recentFilesФайлыToolStripMenuItem;
        private ToolStripMenuItem parametersToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem fAQToolStripMenuItem;
        private ToolStripMenuItem ViewToolStripMenuItem;
        private ToolStripMenuItem ToolPanelToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton cursorButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton lineButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton eraserButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton verticalLineButton;
        private ToolStripButton slopeLineButton;
        private ToolStripSeparator toolStripSeparator5;
    }
}