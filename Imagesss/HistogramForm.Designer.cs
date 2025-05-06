namespace Imagesss
{
    partial class HistogramForm
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
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            btnImportSettings = new Button();
            btnExportSettings = new Button();
            btnSaveImage = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.AppWorkspace;
            pictureBox1.Location = new Point(375, 51);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(413, 250);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = SystemColors.ControlLight;
            pictureBox2.Location = new Point(12, 307);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(776, 131);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = SystemColors.ControlLight;
            pictureBox3.Location = new Point(12, 51);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(357, 250);
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // btnImportSettings
            // 
            btnImportSettings.BackColor = SystemColors.ButtonHighlight;
            btnImportSettings.Location = new Point(12, 12);
            btnImportSettings.Name = "btnImportSettings";
            btnImportSettings.Size = new Size(125, 33);
            btnImportSettings.TabIndex = 3;
            btnImportSettings.Text = "Импорт настроек";
            btnImportSettings.UseVisualStyleBackColor = false;
            // 
            // btnExportSettings
            // 
            btnExportSettings.BackColor = SystemColors.ButtonHighlight;
            btnExportSettings.Location = new Point(143, 12);
            btnExportSettings.Name = "btnExportSettings";
            btnExportSettings.Size = new Size(132, 33);
            btnExportSettings.TabIndex = 4;
            btnExportSettings.Text = "Экспорт  настроек";
            btnExportSettings.UseVisualStyleBackColor = false;
            // 
            // btnSaveImage
            // 
            btnSaveImage.BackColor = SystemColors.ButtonHighlight;
            btnSaveImage.Location = new Point(623, 12);
            btnSaveImage.Name = "btnSaveImage";
            btnSaveImage.Size = new Size(165, 33);
            btnSaveImage.TabIndex = 5;
            btnSaveImage.Text = "Сохранить изображение";
            btnSaveImage.UseVisualStyleBackColor = false;
            // 
            // HistogramForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSaveImage);
            Controls.Add(btnExportSettings);
            Controls.Add(btnImportSettings);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Name = "HistogramForm";
            Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Button btnImportSettings;
        private Button btnExportSettings;
        private Button btnSaveImage;
    }
}