using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Imagesss
{
    public partial class HistogramForm : Form
    {
        private Bitmap originalImage;
        private Bitmap processedImage;
        private List<PointF> controlPoints = new List<PointF>();
        private float[] cubicCurve = new float[256];
        private bool isDragging = false;
        private int draggedPointIndex = -1;

        public HistogramForm(Bitmap img)
        {
            InitializeComponent();
            originalImage = img;
            processedImage = new Bitmap(originalImage);
            pictureBox1.Image = processedImage;

            // Initialize with default control points
            controlPoints.Add(new PointF(0, 0));
            controlPoints.Add(new PointF(255, 255));

            UpdateAll();

            // Set up mouse events for interactive curve editing
            pictureBox3.MouseDown += PictureBox3_MouseDown;
            pictureBox3.MouseMove += PictureBox3_MouseMove;
            pictureBox3.MouseUp += PictureBox3_MouseUp;
            pictureBox3.Paint += PictureBox3_Paint;

            // Configure buttons
            btnSaveImage.Click += BtnSaveImage_Click;
            btnExportSettings.Click += BtnExportSettings_Click;
            btnImportSettings.Click += BtnImportSettings_Click;


            this.Show();
        }

        private void UpdateAll()
        {
            cubicCurve = CalculateCubicInterpolation();
            ApplyGradationTransformation();
            DrawHistogram();
            pictureBox3.Invalidate();
        }

        private void DrawHistogram()
        {
            int[] intensityCount = new int[256];
            int width = processedImage.Width;
            int height = processedImage.Height;
            int histHeight = pictureBox2.Height;
            int histWidth = pictureBox2.Width;
            byte[] imageData = GetImageBytes(processedImage);

            Bitmap histogram = new Bitmap(histWidth, histHeight);

            // Count pixel intensities
            for (int i = 0; i < imageData.Length; i += 4)
            {
                int intensity = (imageData[i] + imageData[i + 1] + imageData[i + 2]) / 3;
                intensityCount[intensity]++;
            }

            int maxCount = intensityCount.Max();
            float verticalScale = (float)histHeight / maxCount;
            float horizontalScale = (float)histWidth / 256;

            using (Graphics g = Graphics.FromImage(histogram))
            {
                g.Clear(Color.White);
                for (int i = 0; i < intensityCount.Length; i++)
                {
                    int x1 = (int)(i * horizontalScale);
                    int x2 = (int)((i + 1) * horizontalScale);
                    int y1 = histHeight - 1;
                    int y2 = histHeight - 1 - (int)(intensityCount[i] * verticalScale);
                    g.FillRectangle(Brushes.Black, x1, y2, x2 - x1, histHeight - y2);
                }
            }

            pictureBox2.Image = histogram;
        }

        private void PictureBox3_Paint(object sender, PaintEventArgs e)
        {
            DrawCurve(e.Graphics, pictureBox3.Width, pictureBox3.Height);
        }

        private void DrawCurve(Graphics g, int width, int height)
        {
            g.Clear(Color.White);

            // Draw grid
            Pen gridPen = new Pen(Color.LightGray, 1);
            for (int i = 0; i <= 255; i += 16)
            {
                int x = (int)(i * (width / 256f));
                g.DrawLine(gridPen, x, 0, x, height);
                g.DrawLine(gridPen, 0, height - x, width, height - x);
            }

            // Draw curve
            Pen curvePen = new Pen(Color.Blue, 2);
            PointF[] curvePoints = new PointF[256];

            for (int i = 0; i < 256; i++)
            {
                curvePoints[i] = new PointF(
                    i * (width / 256f),
                    height - cubicCurve[i] * (height / 255f));
            }

            g.DrawCurve(curvePen, curvePoints);

            // Draw control points
            foreach (var point in controlPoints)
            {
                float x = point.X * (width / 255f);
                float y = height - point.Y * (height / 255f);
                g.FillEllipse(Brushes.Red, x - 4, y - 4, 8, 8);
                g.DrawEllipse(Pens.Black, x - 4, y - 4, 8, 8);
            }
        }

        private void ApplyGradationTransformation()
        {
            Bitmap result = new Bitmap(originalImage.Width, originalImage.Height);
            byte[] originalData = GetImageBytes(originalImage);
            byte[] resultData = new byte[originalData.Length];

            for (int i = 0; i < originalData.Length; i += 4)
            {
                resultData[i] = (byte)Math.Max(0, Math.Min(255, cubicCurve[originalData[i]]));
                resultData[i + 1] = (byte)Math.Max(0, Math.Min(255, cubicCurve[originalData[i + 1]]));
                resultData[i + 2] = (byte)Math.Max(0, Math.Min(255, cubicCurve[originalData[i + 2]]));
                resultData[i + 3] = 255;
            }

            SetImageBytes(result, resultData);
            processedImage = result;
            pictureBox1.Image = processedImage;
        }

        private float[] CalculateCubicInterpolation()
        {
            if (controlPoints.Count < 2)
                return Enumerable.Range(0, 256).Select(i => (float)i).ToArray();

            // Sort points by X coordinate
            controlPoints = controlPoints.OrderBy(p => p.X).ToList();

            int n = controlPoints.Count;
            float[] x = controlPoints.Select(p => p.X).ToArray();
            float[] y = controlPoints.Select(p => p.Y).ToArray();

            float[] h = new float[n - 1];
            float[] a = new float[n];
            float[] b = new float[n];
            float[] c = new float[n];
            float[] d = new float[n];

            for (int i = 0; i < n - 1; i++)
                h[i] = x[i + 1] - x[i];

            float[] alpha = new float[n - 1];
            float[] beta = new float[n - 1];
            float[] A3 = new float[n];

            for (int i = 1; i < n - 1; i++)
            {
                float A1 = h[i - 1];
                float A2 = 2 * (h[i - 1] + h[i]);
                A3[i] = h[i];
                float F = 6 * ((y[i + 1] - y[i]) / h[i] - (y[i] - y[i - 1]) / h[i - 1]);

                if (i == 1)
                {
                    alpha[i] = A2;
                    b[i] = F;
                }
                else
                {
                    alpha[i] = A2 - (A1 * A3[i - 1]) / alpha[i - 1];
                    beta[i] = F - beta[i - 1] * A1 / alpha[i - 1];
                }
            }

            c[n - 1] = 0;
            for (int i = n - 2; i > 0; i--)
                c[i] = (beta[i] - h[i] * c[i + 1]) / alpha[i];

            c[0] = 0;

            for (int i = 0; i < n - 1; i++)
            {
                a[i] = y[i];
                b[i] = (y[i + 1] - y[i]) / h[i] - (h[i] * (c[i + 1] + 2 * c[i])) / 6;
                d[i] = (c[i + 1] - c[i]) / h[i];
            }

            float[] curve = new float[256];
            for (int i = 0; i < 256; i++)
            {
                int segment = FindSegment(x, i);
                float xi = i - x[segment];
                curve[i] = a[segment] + b[segment] * xi + c[segment] / 2 * xi * xi + d[segment] / 6 * xi * xi * xi;
                curve[i] = Math.Max(0, Math.Min(255, curve[i]));
            }

            return curve;
        }

        private int FindSegment(float[] x, float value)
        {
            for (int i = 0; i < x.Length - 1; i++)
            {
                if (value >= x[i] && value <= x[i + 1])
                    return i;
            }
            return x.Length - 2;
        }

        private static byte[] GetImageBytes(Bitmap img)
        {
            byte[] bytes = new byte[img.Width * img.Height * 4];
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            img.UnlockBits(data);
            return bytes;
        }

        private static void SetImageBytes(Bitmap img, byte[] bytes)
        {
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.WriteOnly,
                img.PixelFormat);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            img.UnlockBits(data);
        }

        private void PictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if clicking near an existing point
            for (int i = 0; i < controlPoints.Count; i++)
            {
                float pointX = controlPoints[i].X * (pictureBox3.Width / 255f);
                float pointY = pictureBox3.Height - controlPoints[i].Y * (pictureBox3.Height / 255f);

                if (Math.Abs(e.X - pointX) < 5 && Math.Abs(e.Y - pointY) < 5)
                {
                    isDragging = true;
                    draggedPointIndex = i;
                    return;
                }
            }

            // If not near existing point, add a new one (unless it's an endpoint)
            if (e.X > 10 && e.X < pictureBox3.Width - 10 &&
                e.Y > 10 && e.Y < pictureBox3.Height - 10)
            {
                float newX = e.X * (255f / pictureBox3.Width);
                float newY = (pictureBox3.Height - e.Y) * (255f / pictureBox3.Height);

                // Don't allow points with same X as existing points
                if (!controlPoints.Any(p => Math.Abs(p.X - newX) < 5))
                {
                    controlPoints.Add(new PointF(newX, newY));
                    draggedPointIndex = controlPoints.Count - 1;
                    isDragging = true;
                    UpdateAll();
                }
            }
        }

        private void PictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            // Constrain to pictureBox bounds
            int x = Math.Max(0, Math.Min(pictureBox3.Width, e.X));
            int y = Math.Max(0, Math.Min(pictureBox3.Height, e.Y));

            // Convert to 0-255 range
            float newX = x * (255f / pictureBox3.Width);
            float newY = (pictureBox3.Height - y) * (255f / pictureBox3.Height);

            // For endpoints, only allow moving vertically
            if (draggedPointIndex == 0)
            {
                newX = 0;
            }
            else if (draggedPointIndex == controlPoints.Count - 1 && controlPoints[draggedPointIndex].X == 255)
            {
                newX = 255;
            }

            // Update the point
            controlPoints[draggedPointIndex] = new PointF(newX, Math.Max(0, Math.Min(255, newY)));

            // Sort points to maintain order
            controlPoints = controlPoints.OrderBy(p => p.X).ToList();

            // Find the point again after sorting
            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (Math.Abs(controlPoints[i].X - newX) < 0.1f &&
                    Math.Abs(controlPoints[i].Y - newY) < 0.1f)
                {
                    draggedPointIndex = i;
                    break;
                }
            }

            UpdateAll();
        }

        private void PictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            // Reset to default linear curve
            controlPoints.Clear();
            controlPoints.Add(new PointF(0, 0));
            controlPoints.Add(new PointF(255, 255));
            UpdateAll();
        }

        private void BtnRemovePoint_Click(object sender, EventArgs e)
        {
            // Remove the last added point (but keep at least 2 points)
            if (controlPoints.Count > 2)
            {
                controlPoints.RemoveAt(controlPoints.Count - 1);
                UpdateAll();
            }
        }

        private void BtnImportSettings_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Histogram Settings (*.hst)|*.hst|All files (*.*)|*.*";
                ofd.Title = "Import Histogram Settings";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(ofd.FileName);
                        controlPoints.Clear();

                        foreach (string line in lines)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length == 2 &&
                                float.TryParse(parts[0], out float x) &&
                                float.TryParse(parts[1], out float y))
                            {
                                controlPoints.Add(new PointF(x, y));
                            }
                        }

                        if (controlPoints.Count < 2)
                        {
                            MessageBox.Show("File must contain at least 2 control points", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            controlPoints.Clear();
                            controlPoints.Add(new PointF(0, 0));
                            controlPoints.Add(new PointF(255, 255));
                        }

                        UpdateAll();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading settings: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnExportSettings_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Histogram Settings (*.hst)|*.hst|All files (*.*)|*.*";
                sfd.Title = "Export Histogram Settings";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        List<string> lines = new List<string>();
                        foreach (PointF point in controlPoints)
                        {
                            lines.Add($"{point.X},{point.Y}");
                        }

                        File.WriteAllLines(sfd.FileName, lines);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|BMP Image (*.bmp)|*.bmp";
                sfd.Title = "Save Processed Image";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png;
                        string ext = Path.GetExtension(sfd.FileName).ToLower();

                        if (ext == ".jpg" || ext == ".jpeg")
                            format = ImageFormat.Jpeg;
                        else if (ext == ".bmp")
                            format = ImageFormat.Bmp;

                        processedImage.Save(sfd.FileName, format);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving image: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}