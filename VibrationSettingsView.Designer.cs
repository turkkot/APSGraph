﻿namespace test3
{
    partial class VibrationSettingsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            numericUpDown1 = new NumericUpDown();
            numericUpDown2 = new NumericUpDown();
            numericUpDown3 = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            SuspendLayout();
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(12, 12);
            numericUpDown1.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(73, 23);
            numericUpDown1.TabIndex = 0;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(12, 41);
            numericUpDown2.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(73, 23);
            numericUpDown2.TabIndex = 1;
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(12, 70);
            numericUpDown3.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(73, 23);
            numericUpDown3.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(103, 14);
            label1.Name = "label1";
            label1.Size = new Size(181, 15);
            label1.TabIndex = 3;
            label1.Text = "Уровень допустимых вибраций";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(103, 43);
            label2.Name = "label2";
            label2.Size = new Size(160, 15);
            label2.TabIndex = 4;
            label2.Text = "Уровень выоских вибраций";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(103, 72);
            label3.Name = "label3";
            label3.Size = new Size(183, 15);
            label3.TabIndex = 5;
            label3.Text = "Уровень критических вибраций";
            // 
            // VibrationSettingsView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(321, 116);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(numericUpDown3);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Name = "VibrationSettingsView";
            Text = "Настройка уровней вибраций";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public NumericUpDown numericUpDown1;
        public NumericUpDown numericUpDown2;
        public NumericUpDown numericUpDown3;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}