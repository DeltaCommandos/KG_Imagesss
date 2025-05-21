namespace Imagesss
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            AddImage = new Button();
            saveButton = new Button();
            lyrBox2 = new FlowLayoutPanel();
            btnShowHistogram = new Button();
            btnBinary = new Button();
            btnSpaceFilter = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveBorder;
            pictureBox1.Location = new Point(12, 44);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(771, 499);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // AddImage
            // 
            AddImage.BackColor = SystemColors.ButtonFace;
            AddImage.Location = new Point(86, 12);
            AddImage.Name = "AddImage";
            AddImage.Size = new Size(70, 26);
            AddImage.TabIndex = 1;
            AddImage.Text = "Добавить";
            AddImage.UseVisualStyleBackColor = false;
            AddImage.Click += AddImage_Click;
            // 
            // saveButton
            // 
            saveButton.BackColor = SystemColors.ButtonFace;
            saveButton.Location = new Point(12, 12);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(76, 26);
            saveButton.TabIndex = 4;
            saveButton.Text = "Сохранить";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += saveButton_Click;
            // 
            // lyrBox2
            // 
            lyrBox2.AutoScroll = true;
            lyrBox2.BackColor = SystemColors.ControlLight;
            lyrBox2.Location = new Point(789, 12);
            lyrBox2.Name = "lyrBox2";
            lyrBox2.Size = new Size(254, 531);
            lyrBox2.TabIndex = 6;
            // 
            // btnShowHistogram
            // 
            btnShowHistogram.BackColor = SystemColors.ButtonFace;
            btnShowHistogram.ForeColor = SystemColors.ActiveCaptionText;
            btnShowHistogram.Location = new Point(154, 12);
            btnShowHistogram.Name = "btnShowHistogram";
            btnShowHistogram.Size = new Size(89, 26);
            btnShowHistogram.TabIndex = 0;
            btnShowHistogram.Text = "Гистограмма";
            btnShowHistogram.UseVisualStyleBackColor = false;
            // 
            // btnBinary
            // 
            btnBinary.BackColor = SystemColors.ButtonFace;
            btnBinary.Location = new Point(240, 12);
            btnBinary.Name = "btnBinary";
            btnBinary.Size = new Size(89, 26);
            btnBinary.TabIndex = 7;
            btnBinary.Text = "Бинаризация";
            btnBinary.UseVisualStyleBackColor = false;
            // 
            // btnSpaceFilter
            // 
            btnSpaceFilter.BackColor = SystemColors.ButtonFace;
            btnSpaceFilter.Location = new Point(326, 12);
            btnSpaceFilter.Name = "btnSpaceFilter";
            btnSpaceFilter.Size = new Size(189, 26);
            btnSpaceFilter.TabIndex = 8;
            btnSpaceFilter.Text = "Пространственная фильтрация";
            btnSpaceFilter.UseVisualStyleBackColor = false;
            btnSpaceFilter.Click += btnSpaceFilter_Click;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ButtonFace;
            button1.Location = new Point(511, 12);
            button1.Name = "button1";
            button1.Size = new Size(145, 26);
            button1.TabIndex = 9;
            button1.Text = "Частотная фильтрация";
            button1.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size(1055, 569);
            Controls.Add(button1);
            Controls.Add(btnSpaceFilter);
            Controls.Add(btnBinary);
            Controls.Add(btnShowHistogram);
            Controls.Add(lyrBox2);
            Controls.Add(saveButton);
            Controls.Add(AddImage);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Псевдонедофотошопчик";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Button AddImage;
        private Button saveButton;
        private FlowLayoutPanel lyrBox2;
        private Button btnShowHistogram;
        private Button btnBinary;
        private Button btnSpaceFilter;
        private Button button1;
    }
}
