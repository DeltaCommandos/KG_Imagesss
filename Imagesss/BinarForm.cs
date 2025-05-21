using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic;
namespace Imagesss
{
    public partial class BinarForm : Form
    {
        private Bitmap image;
        public BinarForm(Bitmap img)
        {
            InitializeComponent();
            // image = Resize(img, 1920, 1080);
            image = img;
            pictureBox1.Image = image;
            this.Show();
            
            comboBox1.SelectedIndexChanged += (s, e) =>
            {
                Binarization();
            };
        }
        public void Binarization()
        {
           // MessageBox.Show($"{3/2}");
            switch(comboBox1.Text)
            {
                case "Гаврилов":
                    GavrilovBin();
                    break;
                case "Отсу":
                    OtsuBin();
                    break;
                case "Ниблекс":
                    NiblackBin();
                    break;
                case "Саувол":
                    SauvolBin();
                    break;
                case "Вольф":
                    WolfBin();
                    break;
                case "Брэдли-Рот":
                    BradleyRothBin();
                    break;
                case "Нет":
                    pictureBox1.Image = image;
                    break;
            }
            
        }
        private static Bitmap Resize(Bitmap img, int nw, int nh)
        {
            Bitmap imgr = new Bitmap(nw, nh);
            using (Graphics g = Graphics.FromImage(imgr))
            {
                g.DrawImage(img, 0, 0, nw, nh);
            }
            return imgr;
        }
        private void NiblackBin()
        {
            byte[] data = new byte[image.Width * image.Height * 4];
            data = GetImgBytes(image);
            int a = 2;
            float k = -0.2f;
            int w = image.Width;
            int h = image.Height;
            byte[] resdata = new byte[w * h * 4];
            Bitmap res = new Bitmap(w, h);
            while (true)
            {
                string input = Interaction.InputBox("Введите нечетное число:", "Input", "15");
                if (string.IsNullOrEmpty(input))
                {
                    pictureBox1.Image = image;
                    return;
                }
                if (int.TryParse(input, out a)&& a%2==1 && a>0)
                    break;
                MessageBox.Show("'a' должно быть нечетным", "Error", MessageBoxButtons.OK);

            }
            while (true)
            {
                string input = Interaction.InputBox("Введите k:", "Input", "-0,2");
                if (string.IsNullOrEmpty(input)) return;
                if (float.TryParse(input, out k))
                    break;
            }
            long[,] intimg = new long[h, w];
            long[,] intimgkv = new long[h, w];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    byte gr = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    data[index] = gr;
                    intimg[y, x] = gr + (x > 0 ? intimg[y, x - 1] : 0) + (y > 0 ? intimg[y - 1, x] : 0) - (y > 0 && x > 0 ? intimg[y - 1, x - 1] : 0);
                    intimgkv[y, x] = gr*gr + (x > 0 ? intimgkv[y, x - 1] : 0) + (y > 0 ? intimgkv[y - 1, x] : 0) - (y > 0 && x > 0 ? intimgkv[y - 1, x - 1] : 0);
                }
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int windw = a / 2;
                    int xmin = Math.Max(0, x - windw);
                    int xmax = Math.Min(w - 1, x + windw);
                    int ymax = Math.Min(h - 1, y + windw);
                    int ymin = Math.Max(0, y - windw);
                    long sum = intimg[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimg[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimg[ymax, xmin - 1] : 0) - (ymin > 0 ? intimg[ymin - 1, xmax] : 0);
                    long sumkv = intimgkv[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimgkv[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimgkv[ymax, xmin - 1] : 0) - (ymin > 0 ? intimgkv[ymin - 1, xmax] : 0);
                    int pixcnt = (xmax - xmin + 1) * (ymax - ymin + 1);
                    float mx = sum / (float)pixcnt;
                    float mxmx = sumkv / (float)pixcnt;
                    float Dx = mxmx - mx * mx;
                    float sigma = (float)Math.Sqrt(Dx);
                    float t = mx + k * sigma;
                    int ind = (y * w + x) * 4;
                    byte binar = (data[ind] <= t ? (byte)0 : (byte)255);
                    resdata[ind + 1] = binar;
                    resdata[ind] = binar; //data[ind+1];
                    resdata[ind + 2] = binar;//data[ind + 2] ;
                    resdata[ind + 3] = data[ind + 3];
                }
            }
            SetImgBytes(res, resdata);
            pictureBox1.Image = res;

        }

        private void SauvolBin()
        {
            byte[] data = new byte[image.Width * image.Height * 4];
            data = GetImgBytes(image);
            int a = 2;
            float k = -0.2f;
            int w = image.Width;
            int h = image.Height;
            byte[] resdata = new byte[w * h * 4];
            Bitmap res = new Bitmap(w, h);
            while (a % 2 == 0)
            {
                string input = Interaction.InputBox("Введите нечетное число:", "Input", "15");
                if (string.IsNullOrEmpty(input))
                {
                    pictureBox1.Image = image;
                    return;
                }
                if (int.TryParse(input, out a) && a % 2 == 1)
                    break;
                MessageBox.Show("'a' должно быть нечетным", "Error", MessageBoxButtons.OK);

            }
            while (true)
            {
                string input = Interaction.InputBox("Введите k:", "Input", "-0,2");
                if (string.IsNullOrEmpty(input)) return;
                if (float.TryParse(input, out k))
                    break;
            }
            long[,] intimg = new long[h, w];
            long[,] intimgkv = new long[h, w];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    byte gr = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    data[index] = gr;
                    intimg[y, x] = gr + (x > 0 ? intimg[y, x - 1] : 0) + (y > 0 ? intimg[y - 1, x] : 0) - (y > 0 && x > 0 ? intimg[y - 1, x - 1] : 0);
                    intimgkv[y, x] = gr * gr + (x > 0 ? intimgkv[y, x - 1] : 0) + (y > 0 ? intimgkv[y - 1, x] : 0) - (y > 0 && x > 0 ? intimgkv[y - 1, x - 1] : 0);
                }
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int windw = a / 2;
                    int xmin = Math.Max(0, x - windw);
                    int xmax = Math.Min(w - 1, x + windw);
                    int ymax = Math.Min(h - 1, y + windw);
                    int ymin = Math.Max(0, y - windw);
                    long sum = intimg[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimg[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimg[ymax, xmin - 1] : 0) - (ymin > 0 ? intimg[ymin - 1, xmax] : 0);
                    long sumkv = intimgkv[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimgkv[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimgkv[ymax, xmin - 1] : 0) - (ymin > 0 ? intimgkv[ymin - 1, xmax] : 0);
                    int pixcnt = (xmax - xmin + 1) * (ymax - ymin + 1);
                    float mx = sum / (float)pixcnt;
                    float mxmx = sumkv / (float)pixcnt;
                    float Dx = mxmx - mx * mx;
                    float sigma = (float)Math.Sqrt(Dx);
                    float t = mx*(1+k*(sigma/128-1));
                    int ind = (y * w + x) * 4;
                    byte binar = (data[ind] <= t ? (byte)0 : (byte)255);
                    resdata[ind + 1] = binar;
                    resdata[ind] = binar; //data[ind+1];
                    resdata[ind + 2] = binar;//data[ind + 2] ;
                    resdata[ind + 3] = data[ind + 3];
                }
            }
            SetImgBytes(res, resdata);
            pictureBox1.Image = res;

        }

        private void WolfBin()
        {
            byte[] data = new byte[image.Width * image.Height * 4];
            data = GetImgBytes(image);
            int a = 2;
            float k = -0.2f;
            int w = image.Width;
            int h = image.Height;
            byte[] resdata = new byte[w * h * 4];
            float A = 0;
            Bitmap res = new Bitmap(w, h);
            while (a % 2 == 0)
            {
                string input = Interaction.InputBox("Введите нечетное число:", "Input", "15");
                if (string.IsNullOrEmpty(input))
                {
                    pictureBox1.Image = image;
                    return;
                }
                if (int.TryParse(input, out a) && a % 2 == 1)
                    break;
                MessageBox.Show("'a' должно быть нечетным", "Error", MessageBoxButtons.OK);

            }
            while (true)
            {
                string input = Interaction.InputBox("Введите a:", "Input", "0,5");
                if (string.IsNullOrEmpty(input)) return;
                if (float.TryParse(input, out A))
                    break;
            }
            long[,] intimg = new long[h, w];
            long[,] intimgkv = new long[h, w];
            byte m = 255;
            byte[] grdata = new byte[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    byte gr = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    grdata[y*w+x] = gr;
                    if (gr < m)
                        m = gr;
                    intimg[y, x] = gr + (x > 0 ? intimg[y, x - 1] : 0) + (y > 0 ? intimg[y - 1, x] : 0) - (y > 0 && x > 0 ? intimg[y - 1, x - 1] : 0);
                    intimgkv[y, x] = gr * gr + (x > 0 ? intimgkv[y, x - 1] : 0) + (y > 0 ? intimgkv[y - 1, x] : 0) - (y > 0 && x > 0 ? intimgkv[y - 1, x - 1] : 0);
                }
            }
            float R = 0f;
            float[] arrmx = new float[h*w];
            float[] arrsigma = new float[h*w];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int windw = a / 2;
                    int xmin = Math.Max(0, x - windw);
                    int xmax = Math.Min(w - 1, x + windw);
                    int ymax = Math.Min(h - 1, y + windw);
                    int ymin = Math.Max(0, y - windw);
                    long sum = intimg[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimg[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimg[ymax, xmin - 1] : 0) - (ymin > 0 ? intimg[ymin - 1, xmax] : 0);
                    long sumkv = intimgkv[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimgkv[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimgkv[ymax, xmin - 1] : 0) - (ymin > 0 ? intimgkv[ymin - 1, xmax] : 0);
                    int pixcnt = (xmax - xmin + 1) * (ymax - ymin + 1);
                    float mx = sum / (float)pixcnt;
                    float mxmx = sumkv / (float)pixcnt;
                    float Dx = mxmx - mx * mx;
                    float sigma = (float)Math.Sqrt(Dx);
                   // float t = mx + k * sigma;
                   int ind = (y * w + x);
                    
                    if (sigma > R)
                        R = sigma;
                    arrmx[ind] = mx;
                    arrsigma[ind] = sigma;
                }
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int ind = (y * w + x);
                    float t = (1 - A) * arrmx[ind] + A * m + A * (arrsigma[ind] / R) * (arrmx[ind] - m);
                    byte binar = (grdata[ind] <= t ? (byte)0 : (byte)255);
                    resdata[ind*4 + 1] = binar;
                    resdata[ind *4] = binar; //data[ind+1];
                    resdata[ind*4 + 2] = binar;//data[ind + 2] ;
                    resdata[ind *4 +3] = data[ind*4 + 3];

                }
            }
            SetImgBytes(res, resdata);
            pictureBox1.Image = res;

        }
        private void GavrilovBin()
        {
            int w = image.Width;
            int h = image.Height;
            byte[] data = new byte[w * h*4];
            data = GetImgBytes(image);
            byte[] resdata = new byte[data.Length];
            Bitmap res = new Bitmap(image);
            byte[] gr = new byte[w * h];
            float sum = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    gr[y * w + x] = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    sum += gr[y * w + x];
                }
            }
            float t = sum / (w * h);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int ind = (y * w + x);
                    byte binar = (gr[ind] <= t ? (byte)0 : (byte)255);
                    resdata[ind * 4 + 1] = binar;
                    resdata[ind * 4] = binar; //data[ind+1];
                    resdata[ind * 4 + 2] = binar;//data[ind + 2] ;
                    resdata[ind * 4 + 3] = data[ind * 4 + 3];
                }
            }
            SetImgBytes(res, resdata);
            pictureBox1.Image = res;
        }

        private void OtsuBin()
        {
            byte[] data = new byte[image.Width * image.Height * 4];
            data = GetImgBytes(image);
            int w = image.Width;
            int h = image.Height;
            byte[] resdata = new byte[w * h * 4];
            Bitmap res = new Bitmap(w, h);

            // Вычисляем гистограмму яркости
            int[] histogram = new int[256];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    byte gray = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    histogram[gray]++;
                }
            }

            // Вычисляем общее количество пикселей
            int totalPixels = w * h;

            // Находим оптимальный порог по методу Оцу
            float sum = 0;
            for (int t = 0; t < 256; t++) sum += t * histogram[t];

            float sumB = 0;
            int wB = 0;
            int wF = 0;
            float varMax = 0;
            byte threshold = 0;

            for (int t = 0; t < 256; t++)
            {
                wB += histogram[t]; // Вес фона
                if (wB == 0) continue;

                wF = totalPixels - wB; // Вес переднего плана
                if (wF == 0) break;

                sumB += (float)(t * histogram[t]);

                float mB = sumB / wB; // Средняя яркость фона
                float mF = (sum - sumB) / wF; // Средняя яркость переднего плана

                // Вычисляем межклассовую дисперсию
                float varBetween = (float)wB * (float)wF * (mB - mF) * (mB - mF);

                // Проверяем, является ли это максимальной дисперсией
                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    threshold = (byte)t;
                }
            }

            // Применяем порог к изображению
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    byte gray = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    byte binar = (gray <= threshold) ? (byte)0 : (byte)255;

                    resdata[index] = binar;
                    resdata[index + 1] = binar;
                    resdata[index + 2] = binar;
                    resdata[index + 3] = data[index + 3];
                }
            }

            SetImgBytes(res, resdata);
            pictureBox1.Image = res;
        }


        private void BradleyRothBin()
        {
            byte[] data = new byte[image.Width * image.Height * 4];
            data = GetImgBytes(image);
            int a = 2;
            float k = -0.2f;
            int w = image.Width;
            int h = image.Height;
            byte[] resdata = new byte[w * h * 4];
            Bitmap res = new Bitmap(w, h);
            while (a % 2 == 0)
            {
                string input = Interaction.InputBox("Введите нечетное число:", "Input", "15");
                if (string.IsNullOrEmpty(input))
                {
                    pictureBox1.Image = image;
                    return;
                }
                if (int.TryParse(input, out a) && a % 2 == 1)
                    break;
                MessageBox.Show("'a' должно быть нечетным", "Error", MessageBoxButtons.OK);

            }
            while (true)
            {
                string input = Interaction.InputBox("Введите k:", "Input", "0,15");
                if (string.IsNullOrEmpty(input)) return;
                if (float.TryParse(input, out k))
                    break;
            }
            long[,] intimg = new long[h, w];
            long[,] intimgkv = new long[h, w];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = (y * w + x) * 4;
                    byte gr = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] + 0.0721 * data[index + 2]);
                    data[index] = gr;
                    intimg[y, x] = gr + (x > 0 ? intimg[y, x - 1] : 0) + (y > 0 ? intimg[y - 1, x] : 0) - (y > 0 && x > 0 ? intimg[y - 1, x - 1] : 0);
                    intimgkv[y, x] = gr * gr + (x > 0 ? intimgkv[y, x - 1] : 0) + (y > 0 ? intimgkv[y - 1, x] : 0) - (y > 0 && x > 0 ? intimgkv[y - 1, x - 1] : 0);
                }
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int windw = a / 2;
                    int xmin = Math.Max(0, x - windw);
                    int xmax = Math.Min(w - 1, x + windw);
                    int ymax = Math.Min(h - 1, y + windw);
                    int ymin = Math.Max(0, y - windw);
                    long sum = intimg[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimg[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimg[ymax, xmin - 1] : 0) - (ymin > 0 ? intimg[ymin - 1, xmax] : 0);
                    long sumkv = intimgkv[ymax, xmax] + (xmin > 0 && ymin > 0 ? intimgkv[ymin - 1, xmin - 1] : 0) - (xmin > 0 ? intimgkv[ymax, xmin - 1] : 0) - (ymin > 0 ? intimgkv[ymin - 1, xmax] : 0);
                    int pixcnt = (xmax - xmin + 1) * (ymax - ymin + 1);
                    float mx = sum / (float)pixcnt;
                    float mxmx = sumkv / (float)pixcnt;
                    float Dx = mxmx - mx * mx;
                    float sigma = (float)Math.Sqrt(Dx);
                    float t = mx + k * sigma;
                    int C = (xmax-xmin+1)*(ymax-ymin+1);
                    int ind = (y * w + x) * 4;
                    byte binar = (data[ind]*C < sum*(1-k) ? (byte)0 : (byte)255);
                    resdata[ind + 1] = binar;
                    resdata[ind] = binar; //data[ind+1];
                    resdata[ind + 2] = binar;//data[ind + 2] ;
                    resdata[ind + 3] = data[ind + 3];
                }
            }
            SetImgBytes(res, resdata);
            pictureBox1.Image = res;

        }


        private static byte[] GetImgBytes(Bitmap img)
        {
            if (img == null || img.Width == 0 || img.Height == 0)
            {
                MessageBox.Show("Invalid image.");
                return new byte[] { 0, 1 };
            }
            byte[] byts = new byte[img.Width * img.Height * 4];
            var data1 = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(data1.Scan0, byts, 0, byts.Length);
            img.UnlockBits(data1);
            return byts;
        }

        private static void SetImgBytes(Bitmap img, byte[] bytes)
        {
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            img.UnlockBits(data);
        }
    }
}
