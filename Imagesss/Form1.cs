using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImgLyr;

namespace Imagesss
{
    public partial class Form1 : Form
    {
        private List<ImgLayer> _layers = new List<ImgLayer>();
        private Bitmap _resultImage;
        private readonly object _syncLock = new object();
        //private Button btnLayerHistogram;

        private void InitializeHistogramButtons()
        {
            btnShowHistogram.Click += (sender, e) =>
            {
                if (_resultImage != null)
                {
                    new HistogramForm(_resultImage).Show();
                }
                else
                {
                    MessageBox.Show("Нет изображения для анализа");
                }
            };



            // Добавляем возможность выбора слоя щелчком
            foreach (Control control in lyrBox2.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Click += (sender, e) =>
                    {
                        // Сбрасываем выделение всех панелей
                        foreach (Panel p in lyrBox2.Controls.OfType<Panel>())
                        {
                            p.BackColor = SystemColors.ControlLight;
                        }

                        // Выделяем выбранную панель
                        panel.BackColor = Color.LightBlue;
                    };
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Кнопка гистограммы для всего изображения
            btnShowHistogram = new Button
            {
                Text = "Гистограмма",
                Location = new Point(597, 12),
                Size = new Size(144, 56)
            };
            this.Controls.Add(btnShowHistogram);

            btnShowHistogram.Click += (sender, e) =>
            {
                if (_resultImage != null)
                {
                    new HistogramForm(new Bitmap(_resultImage)).Show();
                }
                else
                {
                    MessageBox.Show("Нет изображения для анализа");
                }
            };
            btnBinary.Click += (sender, e) =>
            {
                if (_resultImage != null)
                {
                    new BinarForm(new Bitmap(_resultImage)).Show();
                }
                else
                {
                    MessageBox.Show("Нет изображения для анализа");
                }
            };
        }
        private async void AddImage_Click(object sender, EventArgs e)
        {
            await AddImagesAsync();
            await ProcessLayersAsync();
        }

        private async Task AddImagesAsync()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Images|*.png;*.jpg;*.bmp|All files|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await Task.Run(() =>
                    {
                        foreach (var file in openFileDialog.FileNames)
                        {
                            var img = LoadAndResizeImage(file, 1920, 1080);
                            var imgName = Path.GetFileNameWithoutExtension(file);
                            imgName = imgName.Length > 15 ? imgName.Substring(0, 12) + "..." : imgName;

                            var layer = new ImgLayer(img, imgName);

                            lock (_syncLock)
                            {
                                // Добавляем в начало списка (верхний слой)
                                _layers.Insert(0, layer);
                                this.Invoke(new Action(() => AddLayerToUI(layer)));
                            }
                        }
                    });
                }
            }
        }

        private Bitmap LoadAndResizeImage(string path, int width, int height)
        {
            using (var original = new Bitmap(path))
            {
                var resized = new Bitmap(width, height);
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(original, 0, 0, width, height);
                }
                return resized;
            }
        }

        private void AddLayerToUI(ImgLayer layer)
        {
            var panel = new Panel
            {
                Width = 230,
                Height = 380, // Увеличили высоту для новой кнопки
                Dock = DockStyle.Top,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = layer // Связываем слой с панелью
            };


            var pictureBox = new PictureBox
            {
                Image = new Bitmap(layer.Image, new Size(220, 180)),
                Size = new Size(220, 180),
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label { Text = layer.Name, AutoSize = true, Location = new Point(5, 185) };

            layer.ChannelList.Location = new Point(5, 210);
            layer.ChannelList.Width = 60;

            layer.OperList.Location = new Point(80, 210);
            layer.OperList.Width = 100;

            var opacityTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 5,
                Location = new Point(5, 250),
                Width = 160
            };

            var opacityLabel = new Label
            {
                Text = "100%",
                AutoSize = true,
                Location = new Point(170, 250)
            };

            var btnLayerHist = new Button
            {
                Text = "Гистограмма слоя",
                Location = new Point(5, 310),
                Size = new Size(220, 30),
                Tag = layer // Связываем кнопку с этим слоем
            };

            btnLayerHist.Click += (sender, e) =>
            {
                if (btnLayerHist.Tag is ImgLayer layer)
                {
                    new HistogramForm(new Bitmap(layer.Image)).Show();
                }
            };

            // Добавляем кнопку в панель
            panel.Controls.Add(btnLayerHist);

            btnLayerHist.Click += (sender, e) =>
            {
                var btn = sender as Button;
                if (btn?.Tag is ImgLayer layer)
                {
                    new HistogramForm(new Bitmap(layer.Image)).Show();
                }
            };

            opacityTrackBar.Scroll += (sender, e) =>
            {
                layer.Opacity = opacityTrackBar.Value / 100f;
                opacityLabel.Text = $"{opacityTrackBar.Value}%";
                ProcessLayersAsync();
            };

            // Кнопки управления слоем
            var btnUp = new Button { Text = "↑", Width = 25, Height = 25, Location = new Point(5, 345) };
            var btnDelete = new Button { Text = "Х", Width = 25, Height = 25, Location = new Point(35, 345) };
            var btnDown = new Button { Text = "↓", Width = 25, Height = 25, Location = new Point(65, 345) };

            btnUp.Click += (sender, e) => MoveLayerUp(panel);
            btnDown.Click += (sender, e) => MoveLayerDown(panel);
            btnDelete.Click += (sender, e) => DeleteLayer(panel);

            panel.Controls.AddRange(new Control[] {
              pictureBox, label, layer.ChannelList, layer.OperList,
              opacityTrackBar, opacityLabel, btnLayerHist,
              btnUp, btnDown, btnDelete
            });

            lyrBox2.Controls.Add(panel);
            lyrBox2.Controls.SetChildIndex(panel, 0);
        }
        private void MoveLayerUp(Panel panel)
        {
            var layer = panel.Tag as ImgLayer;
            if (layer == null) return;

            int index = _layers.IndexOf(layer);
            if (index < _layers.Count - 1) // Можно поднять выше
            {
                // Меняем местами в списке
                _layers[index] = _layers[index + 1];
                _layers[index + 1] = layer;

                // Меняем порядок в интерфейсе
                int panelIndex = lyrBox2.Controls.IndexOf(panel);
                if (panelIndex > 0)
                {
                    lyrBox2.Controls.SetChildIndex(panel, panelIndex - 1);
                }

                ProcessLayersAsync();
            }
        }

        private void MoveLayerDown(Panel panel)
        {
            var layer = panel.Tag as ImgLayer;
            if (layer == null) return;

            int index = _layers.IndexOf(layer);
            if (index > 0) // Можно опустить ниже
            {
                // Меняем местами в списке
                _layers[index] = _layers[index - 1];
                _layers[index - 1] = layer;

                // Меняем порядок в интерфейсе
                int panelIndex = lyrBox2.Controls.IndexOf(panel);
                if (panelIndex < lyrBox2.Controls.Count - 1)
                {
                    lyrBox2.Controls.SetChildIndex(panel, panelIndex + 1);
                }

                ProcessLayersAsync();
            }
        }

        private void DeleteLayer(Panel panel)
        {
            var layer = panel.Tag as ImgLayer;
            if (layer == null) return;

            // Удаляем из списка
            _layers.Remove(layer);

            // Удаляем из интерфейса
            lyrBox2.Controls.Remove(panel);

            // Освобождаем ресурсы
            layer.Image.Dispose();
            panel.Dispose();

            ProcessLayersAsync();
        }

        private async void proButton_Click(object sender, EventArgs e)
        {
            await ProcessLayersAsync();
        }

        private async Task ProcessLayersAsync()
        {
            if (_layers.Count == 0) return;

            try
            {
                pictureBox1.Image = null;
                _resultImage?.Dispose();

                _resultImage = await Task.Run(() => CombineLayers());

                pictureBox1.Image = _resultImage;
                pictureBox1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing layers: {ex.Message}");
            }
        }

        private Bitmap CombineLayers()
        {
            if (_layers.Count == 0) return new Bitmap(1920, 1080);

            var result = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Black);
            }

            // Обрабатываем слои в обратном порядке (от нижнего к верхнему)
            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                var layer = _layers[i];
                var layerImage = ApplyChannelFilter(layer.Image, layer.Channel);

                result = BlendImages(result, layerImage, layer.Op, layer.Opacity);
            }

            return result;
        }

        private Bitmap ApplyChannelFilter(Bitmap image, string channel)
        {
            // Создаем копию, гарантируя что оригинал не заблокирован
            Bitmap CreateUnlockedCopy(Bitmap source)
            {
                Bitmap copy;
                lock (source) // Блокируем на время создания копии
                {
                    copy = new Bitmap(source.Width, source.Height);
                    using (var g = Graphics.FromImage(copy))
                    using (var temp = new Bitmap(source)) // Создаем временную копию
                    {
                        g.DrawImage(temp, 0, 0);
                    }
                }
                return copy;
            }

            if (channel == "RGB")
                return CreateUnlockedCopy(image);

            // Остальная обработка...
            var result = CreateUnlockedCopy(image);
            var data = GetImageData(result);
            var resultData = (byte[])data.Clone();

            Parallel.For(0, data.Length / 4, i =>
            {
                int index = i * 4;

                // Порядок в памяти: BGRA
                switch (channel)
                {
                    case "R":
                        resultData[index] = 0;     // B
                        resultData[index + 1] = 0;  // G
                        break;
                    case "G":
                        resultData[index] = 0;     // B
                        resultData[index + 2] = 0; // R
                        break;
                    case "B":
                        resultData[index + 1] = 0; // G
                        resultData[index + 2] = 0; // R
                        break;
                    case "RG":
                        resultData[index] = 0;     // B
                        break;
                    case "RB":
                        resultData[index + 1] = 0; // G
                        break;
                    case "GB":
                        resultData[index + 2] = 0; // R
                        break;
                }
            });

            SetImageData(result, resultData);
            return result;
        }

        private Bitmap BlendImages(Bitmap baseImage, Bitmap topImage, string blendMode, float opacity)
        {
            var result = new Bitmap(baseImage.Width, baseImage.Height);

            // Создаем копии изображений, чтобы избежать блокировок
            using (var unlockedBase = CreateUnlockedCopy(baseImage))
            using (var unlockedTop = CreateUnlockedCopy(topImage))
            {
                var baseData = GetImageData(unlockedBase);
                var topData = GetImageData(unlockedTop);
                var resultData = new byte[baseData.Length];

                int width = unlockedBase.Width;
                int height = unlockedBase.Height;
                int centerX = width / 2;
                int centerY = height / 2;
                int radius = Math.Min(centerX, centerY);

                Parallel.For(0, height, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * width + x) * 4;

                        // Применяем opacity к верхнему изображению
                        float alpha = opacity;
                        float maskAlpha = 1.0f;

                        // Расчет альфа-канала для масок
                        if (blendMode.StartsWith("Маска:"))
                        {
                            if (blendMode == "Маска: круг")
                            {
                                float dist = (float)Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                                maskAlpha = dist <= radius ? 1.0f - (dist / radius) : 0.0f;
                            }
                            else if (blendMode == "Маска: квадрат")
                            {
                                int squareSize = radius * 2;
                                int left = centerX - radius;
                                int top = centerY - radius;
                                maskAlpha = (x >= left && x < left + squareSize && y >= top && y < top + squareSize) ? 1.0f : 0.0f;
                            }
                            else if (blendMode == "Маска: прямоугольник")
                            {
                                int rectWidth = radius * 3;
                                int rectHeight = radius;
                                int left = centerX - rectWidth / 2;
                                int top = centerY - rectHeight / 2;
                                maskAlpha = (x >= left && x < left + rectWidth && y >= top && y < top + rectHeight) ? 1.0f : 0.0f;
                            }
                            alpha *= maskAlpha;
                        }

                        // Порядок в памяти: BGRA
                        int b1 = baseData[index];
                        int g1 = baseData[index + 1];
                        int r1 = baseData[index + 2];

                        int b2 = topData[index];
                        int g2 = topData[index + 1];
                        int r2 = topData[index + 2];

                        // Применяем операцию смешивания
                        switch (blendMode)
                        {
                            case "Сложение":
                                resultData[index] = (byte)Math.Min(255, (int)(b1 + b2 * alpha));
                                resultData[index + 1] = (byte)Math.Min(255, (int)(g1 + g2 * alpha));
                                resultData[index + 2] = (byte)Math.Min(255, (int)(r1 + r2 * alpha));
                                break;
                            case "Умножение":
                                resultData[index] = (byte)(b1 * b2 * alpha / 255);
                                resultData[index + 1] = (byte)(g1 * g2 * alpha / 255);
                                resultData[index + 2] = (byte)(r1 * r2 * alpha / 255);
                                break;
                            case "Максимум":
                                resultData[index] = (byte)Math.Max(b1, (int)(b2 * alpha));
                                resultData[index + 1] = (byte)Math.Max(g1, (int)(g2 * alpha));
                                resultData[index + 2] = (byte)Math.Max(r1, (int)(r2 * alpha));
                                break;
                            case "Минимум":
                                resultData[index] = (byte)Math.Min(b1, (int)(b2 * alpha));
                                resultData[index + 1] = (byte)Math.Min(g1, (int)(g2 * alpha));
                                resultData[index + 2] = (byte)Math.Min(r1, (int)(r2 * alpha));
                                break;
                            case "Среднее":
                                resultData[index] = (byte)((b1 + b2 * alpha) / 2);
                                resultData[index + 1] = (byte)((g1 + g2 * alpha) / 2);
                                resultData[index + 2] = (byte)((r1 + r2 * alpha) / 2);
                                break;
                            case "Маска: круг":
                            case "Маска: квадрат":
                            case "Маска: прямоугольник":
                                // Для масок используем нормальное наложение с рассчитанным alpha
                                resultData[index] = (byte)(b1 * (1 - alpha) + b2 * alpha);
                                resultData[index + 1] = (byte)(g1 * (1 - alpha) + g2 * alpha);
                                resultData[index + 2] = (byte)(r1 * (1 - alpha) + r2 * alpha);
                                break;
                            default: // "Без эффектов" или неизвестный режим
                                resultData[index] = (byte)(b1 * (1 - alpha) + b2 * alpha);
                                resultData[index + 1] = (byte)(g1 * (1 - alpha) + g2 * alpha);
                                resultData[index + 2] = (byte)(r1 * (1 - alpha) + r2 * alpha);
                                break;
                        }

                        // Альфа-канал
                        resultData[index + 3] = 255;
                    }
                });

                SetImageData(result, resultData);
            }
            return result;
        }

        // Добавьте этот метод в ваш класс, если его еще нет
        private Bitmap CreateUnlockedCopy(Bitmap source)
        {
            var copy = new Bitmap(source.Width, source.Height);
            using (var g = Graphics.FromImage(copy))
            {
                g.DrawImage(source, 0, 0);
            }
            return copy;
        }
        private byte[] GetImageData(Bitmap image)
        {
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var bitmapData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                int bytes = Math.Abs(bitmapData.Stride) * image.Height;
                byte[] data = new byte[bytes];
                Marshal.Copy(bitmapData.Scan0, data, 0, bytes);
                return data;
            }
            finally
            {
                image.UnlockBits(bitmapData); // Гарантированное освобождение
            }
        }

        private void SetImageData(Bitmap image, byte[] data)
        {
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var bitmapData = image.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            await SaveImageAsync();
        }

        private Task SaveImageAsync()
        {
            if (_resultImage == null) return Task.CompletedTask;

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return Task.Run(() =>
                    {
                        _resultImage.Save(saveFileDialog.FileName, ImageFormat.Png);
                    });
                }
            }

            return Task.CompletedTask;
        }

        private void btnShowHistogram_Click(object sender, EventArgs e)
        {

        }



        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _resultImage?.Dispose();
        //        foreach (var layer in _layers)
        //        {
        //            layer.Image?.Dispose();
        //        }

        //        if (components != null)
        //        {
        //            components.Dispose();
        //        }
        //    }
        //    base.Dispose(disposing);
        //}
    }
}