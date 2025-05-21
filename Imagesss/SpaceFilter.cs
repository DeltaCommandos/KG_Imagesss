using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Imagesss
{
    public partial class SpaceFilter : Form
    {
        private Bitmap originalImage;
        private Bitmap currentImage;
        private float[,] customKernel;
        private int kernelWidth = 3;
        private int kernelHeight = 3;

        public SpaceFilter(Bitmap image)
        {
            InitializeComponent();
            originalImage = new Bitmap(image);
            currentImage = new Bitmap(image);
            pictureBox1.Image = currentImage;

            // Initialize default kernel
            customKernel = new float[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    customKernel[i, j] = 1f / 9f;

            

            textBox1.Text = "3";
            textBox2.Text = "3";
            numericUpDown1.Value = 3;
        }

        private void ApplyFilter()
        {
            if (currentImage == null) return;

            switch (comboSpace.SelectedItem.ToString())
            {
                case "Линейная фильтрация":
                    ApplyLinearFilter();
                    break;
                case "Медианная фильтрация":
                    ApplyMedianFilter();
                    break;
                case "Размытие по Гауссу":
                    ApplyGaussianBlur();
                    break;
                case "Нет":
                    currentImage = new Bitmap(originalImage);
                    break;
            }

            pictureBox1.Image = currentImage;
        }

        private void ApplyLinearFilter()
        {
            try
            {
                kernelWidth = int.Parse(textBox1.Text);
                kernelHeight = int.Parse(textBox2.Text);

                if (kernelWidth % 2 == 0 || kernelHeight % 2 == 0)
                {
                    MessageBox.Show("Размеры ядра должны быть нечетными");
                    return;
                }

                currentImage = LinearFilter(originalImage, customKernel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при линейной фильтрации: {ex.Message}");
            }
        }

        private void ApplyMedianFilter()
        {
            try
            {
                kernelWidth = int.Parse(textBox1.Text);
                kernelHeight = int.Parse(textBox2.Text);

                if (kernelWidth % 2 == 0 || kernelHeight % 2 == 0)
                {
                    MessageBox.Show("Размеры окна должны быть нечетными");
                    return;
                }

                currentImage = MedianFilter(originalImage, kernelWidth, kernelHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при медианной фильтрации: {ex.Message}");
            }
        }

        private void ApplyGaussianBlur()
        {
            try
            {
                float sigma = (float)numericUpDown1.Value;
                kernelWidth = int.Parse(textBox1.Text);
                kernelHeight = int.Parse(textBox2.Text);

                if (kernelWidth % 2 == 0 || kernelHeight % 2 == 0)
                {
                    MessageBox.Show("Размеры ядра должны быть нечетными");
                    return;
                }

                float[,] gaussianKernel = CreateGaussianKernel(kernelWidth, kernelHeight, sigma);
                currentImage = LinearFilter(originalImage, gaussianKernel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при размытии по Гауссу: {ex.Message}");
            }
        }

        private Bitmap LinearFilter(Bitmap source, float[,] kernel)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            BitmapData sourceData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytes = sourceData.Stride * sourceData.Height;
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, bytes);
            source.UnlockBits(sourceData);

            int kernelWidth = kernel.GetLength(1);
            int kernelHeight = kernel.GetLength(0);
            int kernelOffsetX = kernelWidth / 2;
            int kernelOffsetY = kernelHeight / 2;

            for (int y = kernelOffsetY; y < source.Height - kernelOffsetY; y++)
            {
                for (int x = kernelOffsetX; x < source.Width - kernelOffsetX; x++)
                {
                    int byteOffset = y * sourceData.Stride + x * 4;
                    float blue = 0, green = 0, red = 0;

                    for (int ky = -kernelOffsetY; ky <= kernelOffsetY; ky++)
                    {
                        for (int kx = -kernelOffsetX; kx <= kernelOffsetX; kx++)
                        {
                            int pixelOffset = byteOffset + ky * sourceData.Stride + kx * 4;
                            float kernelValue = kernel[ky + kernelOffsetY, kx + kernelOffsetX];

                            blue += pixelBuffer[pixelOffset] * kernelValue;
                            green += pixelBuffer[pixelOffset + 1] * kernelValue;
                            red += pixelBuffer[pixelOffset + 2] * kernelValue;
                        }
                    }

                    resultBuffer[byteOffset] = (byte)Math.Min(255, Math.Max(0, blue));
                    resultBuffer[byteOffset + 1] = (byte)Math.Min(255, Math.Max(0, green));
                    resultBuffer[byteOffset + 2] = (byte)Math.Min(255, Math.Max(0, red));
                    resultBuffer[byteOffset + 3] = 255; // Alpha channel
                }
            }

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            result.UnlockBits(resultData);

            return result;
        }

        private Bitmap MedianFilter(Bitmap source, int windowWidth, int windowHeight)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            BitmapData sourceData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytes = sourceData.Stride * sourceData.Height;
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, bytes);
            source.UnlockBits(sourceData);

            int windowOffsetX = windowWidth / 2;
            int windowOffsetY = windowHeight / 2;
            int windowSize = windowWidth * windowHeight;

            for (int y = windowOffsetY; y < source.Height - windowOffsetY; y++)
            {
                for (int x = windowOffsetX; x < source.Width - windowOffsetX; x++)
                {
                    int byteOffset = y * sourceData.Stride + x * 4;
                    byte[] windowR = new byte[windowSize];
                    byte[] windowG = new byte[windowSize];
                    byte[] windowB = new byte[windowSize];
                    int index = 0;

                    for (int wy = -windowOffsetY; wy <= windowOffsetY; wy++)
                    {
                        for (int wx = -windowOffsetX; wx <= windowOffsetX; wx++)
                        {
                            int pixelOffset = byteOffset + wy * sourceData.Stride + wx * 4;
                            windowB[index] = pixelBuffer[pixelOffset];
                            windowG[index] = pixelBuffer[pixelOffset + 1];
                            windowR[index] = pixelBuffer[pixelOffset + 2];
                            index++;
                        }
                    }

                    // Use QuickSelect to find median
                    resultBuffer[byteOffset] = QuickSelect(windowB, windowSize / 2);
                    resultBuffer[byteOffset + 1] = QuickSelect(windowG, windowSize / 2);
                    resultBuffer[byteOffset + 2] = QuickSelect(windowR, windowSize / 2);
                    resultBuffer[byteOffset + 3] = 255; // Alpha channel
                }
            }

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);
            result.UnlockBits(resultData);

            return result;
        }

        private byte QuickSelect(byte[] arr, int k)
        {
            // Quickselect algorithm implementation
            int left = 0;
            int right = arr.Length - 1;
            Random rand = new Random();

            while (left <= right)
            {
                int pivotIndex = Partition(arr, left, right, rand.Next(left, right + 1));

                if (pivotIndex == k)
                {
                    return arr[pivotIndex];
                }
                else if (pivotIndex < k)
                {
                    left = pivotIndex + 1;
                }
                else
                {
                    right = pivotIndex - 1;
                }
            }

            return arr[left];
        }

        private int Partition(byte[] arr, int left, int right, int pivotIndex)
        {
            byte pivotValue = arr[pivotIndex];
            Swap(ref arr[pivotIndex], ref arr[right]);
            int storeIndex = left;

            for (int i = left; i < right; i++)
            {
                if (arr[i] < pivotValue)
                {
                    Swap(ref arr[i], ref arr[storeIndex]);
                    storeIndex++;
                }
            }

            Swap(ref arr[storeIndex], ref arr[right]);
            return storeIndex;
        }

        private void Swap(ref byte a, ref byte b)
        {
            byte temp = a;
            a = b;
            b = temp;
        }

        private float[,] CreateGaussianKernel(int width, int height, float sigma)
        {
            float[,] kernel = new float[height, width];
            float sum = 0;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                for (int x = -halfWidth; x <= halfWidth; x++)
                {
                    float value = (float)(1.0 / (2.0 * Math.PI * sigma * sigma) *
                                 Math.Exp(-(x * x + y * y) / (2 * sigma * sigma)));
                    kernel[y + halfHeight, x + halfWidth] = value;
                    sum += value;
                }
            }

            // Normalize the kernel
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    kernel[y, x] /= sum;

            return kernel;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }
    }
}