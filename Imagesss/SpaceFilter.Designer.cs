namespace Imagesss
{
    partial class SpaceFilter
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
            pictureBox1 = new PictureBox();
            comboSpace = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label3 = new Label();
            numericUpDown1 = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveBorder;
            pictureBox1.Location = new Point(12, 59);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(771, 499);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // comboSpace
            // 
            comboSpace.DropDownStyle = ComboBoxStyle.DropDownList;
            comboSpace.FormattingEnabled = true;
            comboSpace.Items.AddRange(new object[] { "Нет","Линейная фильтрация","Медианная фильтрация","Размытие по Гауссу" });
            comboSpace.Location = new Point(12, 12);
            comboSpace.Name = "comboSpace";
            comboSpace.Size = new Size(154, 23);
            comboSpace.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(191, 15);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 3;
            label1.Text = "Высота таблицы";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(392, 15);
            label2.Name = "label2";
            label2.Size = new Size(103, 15);
            label2.TabIndex = 4;
            label2.Text = "Ширина таблицы";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(295, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(80, 23);
            textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(501, 12);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(80, 23);
            textBox2.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(660, 15);
            label3.Name = "label3";
            label3.Size = new Size(42, 15);
            label3.TabIndex = 7;
            label3.Text = "Сигма";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(720, 12);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(63, 23);
            numericUpDown1.TabIndex = 8;
            // 
            // SpaceFilter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size(991, 586);
            Controls.Add(numericUpDown1);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(comboSpace);
            Controls.Add(pictureBox1);
            Name = "SpaceFilter";
            Text = "SpaceFilter";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private ComboBox comboSpace;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label3;
        private NumericUpDown numericUpDown1;


    }
}