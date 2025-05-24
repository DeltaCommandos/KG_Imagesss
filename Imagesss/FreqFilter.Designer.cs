namespace Imagesss
{
    partial class FreqFilter
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
            pictureBoxFiltered = new PictureBox();
            pictureBoxFilter = new PictureBox();
            comboBox1 = new ComboBox();
            button1 = new Button();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFiltered).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFilter).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxFiltered
            // 
            pictureBoxFiltered.BackColor = SystemColors.AppWorkspace;
            pictureBoxFiltered.Location = new Point(15, 142);
            pictureBoxFiltered.Name = "pictureBoxFiltered";
            pictureBoxFiltered.Size = new Size(441, 296);
            pictureBoxFiltered.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxFiltered.TabIndex = 0;
            pictureBoxFiltered.TabStop = false;
            // 
            // pictureBoxFilter
            // 
            pictureBoxFilter.BackColor = SystemColors.ControlLight;
            pictureBoxFilter.Location = new Point(291, 15);
            pictureBoxFilter.Name = "pictureBoxFilter";
            pictureBoxFilter.Size = new Size(165, 121);
            pictureBoxFilter.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxFilter.TabIndex = 1;
            pictureBoxFilter.TabStop = false;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Идеальный", "Гаусс ФНЧ", "Гаусс ФВЧ" });
            comboBox1.Location = new Point(15, 15);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(197, 23);
            comboBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(15, 96);
            button1.Name = "button1";
            button1.Size = new Size(197, 40);
            button1.TabIndex = 3;
            button1.Text = "Рассчитать";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(15, 54);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(197, 23);
            textBox1.TabIndex = 4;
            // 
            // FreqFilter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size(472, 450);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(comboBox1);
            Controls.Add(pictureBoxFilter);
            Controls.Add(pictureBoxFiltered);
            Name = "FreqFilter";
            Text = "Частотная фильтрация";
            ((System.ComponentModel.ISupportInitialize)pictureBoxFiltered).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFilter).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxFiltered;
        private PictureBox pictureBoxFilter;
        private ComboBox comboBox1;
        private Button button1;
        private TextBox textBox1;
    }
}