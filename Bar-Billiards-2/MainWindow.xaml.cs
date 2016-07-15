﻿using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.FFMPEG;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge;

namespace Bar_Billiards_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int minHue = 0, maxHue = 100;
        private int minSat = 0, maxSat = 1;
        private int minLum = 0, maxLum = 1;
        private int x = 220;
        private int y = 100;
        private int w = 930;
        private int h = 540;
        private int minRed = 0, maxRed = 255;
        private int minGreen = 0, maxGreen = 255;
        private int minBlue = 0, maxBlue = 255;
        private Font font = new Font("Arial", 16);
        private Brush brush = new SolidBrush(Color.Blue);


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VideoFileSource videoSource = new VideoFileSource(@"D:\Temp\shares\2016-07-13 12-51-56.372.wmv");
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var img = cropImage((Bitmap)eventArgs.Frame.Clone(), new Rectangle(x, y, w, h));
                var img2 = cropImage((Bitmap)eventArgs.Frame.Clone(), new Rectangle(x, y, w, h));

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate { image1.Source = bi; }));

                Bitmap filteredImage = hsl(img2);

                BlobCounter blobCounter = new BlobCounter();
                blobCounter.MinHeight = 20;
                blobCounter.MinWidth = 20;

                blobCounter.ProcessImage(filteredImage);
                Blob[] blobs = blobCounter.GetObjectsInformation();

                var nonIndexed = Indexed2Image(filteredImage);

                if (blobs.Length > 0)
                {
                    using (var g = Graphics.FromImage(nonIndexed))
                    {
                        foreach (var blob in blobs.Where(o => o.Rectangle.Width > 20))
                        {
                            using (var pen = new Pen(Color.Red, width: 3))
                            {
                                g.DrawEllipse(pen, blob.Rectangle);
                                g.DrawString(string.Format("{0} {1}", blob.Rectangle.Width, blob.Rectangle.Height), font, brush, new PointF(blob.Rectangle.X, blob.Rectangle.Y));
                            }
                        }
                    }
                }

                MemoryStream ms2 = new MemoryStream();
                nonIndexed.Save(ms2, ImageFormat.Bmp);
                ms2.Seek(0, SeekOrigin.Begin);
                BitmapImage bi2 = new BitmapImage();
                bi2.BeginInit();
                bi2.StreamSource = ms2;
                bi2.EndInit();

                bi2.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    image2.Source = bi2;
                }));
            }
            catch (Exception ex)
            {
                lblError.Content = ex.ToString();
            }

        }

        #region Filters
        private Bitmap hsl(Bitmap img)
        {
            HSLFiltering HslFilter = new HSLFiltering();
            HslFilter.Hue = new IntRange(minHue, maxHue);
            HslFilter.Saturation = new Range(minSat, maxSat);
            HslFilter.Luminance = new Range(minLum, maxLum);
            HslFilter.ApplyInPlace(img);

            Grayscale grayFilter = new GrayscaleBT709();
            Bitmap grayImage = grayFilter.Apply(img);

            return grayImage;
        }

        private Bitmap rgb(Bitmap img)
        {
            ColorFiltering filter = new ColorFiltering();
            filter.Red = new IntRange(minRed, maxRed);
            filter.Green = new IntRange(minGreen, maxGreen);
            filter.Blue = new IntRange(minBlue, maxBlue);
            Bitmap filteredImage = filter.Apply(img);
            return filteredImage;
        }

        private Bitmap ycbr(Bitmap img)
        {
            // create filter
            YCbCrFiltering filter = new YCbCrFiltering();
            // set color ranges to keep
            filter.Cb = new Range(-0.2f, 0);
            filter.Cr = new Range(0.26f, 0.5f);
            filter.FillColor = YCbCr.FromRGB(new RGB(Color.Black));
            // apply the filter
            
            filter.ApplyInPlace(img);

            return img;
        }

        #endregion

        #region Helpers
        private static Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        public static Bitmap Indexed2Image(Bitmap img)
        {
            Bitmap clone = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(img, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return clone;
        }
        #endregion

        #region Events


        private void MinHue_Changed(object sender, EventArgs e)
        {
            minHue = Convert.ToInt32(MinHueSlider.Value);
        }

        private void MaxHue_Changed(object sender, EventArgs e)
        {
            maxHue = Convert.ToInt32(MaxHueSlider.Value);
        }

        private void MinLum_Changed(object sender, EventArgs e)
        {
            minLum = Convert.ToInt32(MinLumSlider.Value);
        }

        private void MaxLum_Changed(object sender, EventArgs e)
        {
            maxLum = Convert.ToInt32(MaxLumSlider.Value);
        }

        private void MinSat_Changed(object sender, EventArgs e)
        {
            minSat = Convert.ToInt32(MinSatSlider.Value);
        }

        private void MaxSat_Changed(object sender, EventArgs e)
        {
            maxSat = Convert.ToInt32(MaxSatSlider.Value);
        }

        private void X_Changed(object sender, EventArgs e)
        {
            x = Convert.ToInt32(XSlider.Value);
        }

        private void Y_Changed(object sender, EventArgs e)
        {
            y = Convert.ToInt32(YSlider.Value);
        }

        private void W_Changed(object sender, EventArgs e)
        {
            w = Convert.ToInt32(WSlider.Value);
        }

        private void H_Changed(object sender, EventArgs e)
        {
            h = Convert.ToInt32(HSlider.Value);
        }

        private void MaxRed_Changed(object sender, EventArgs e)
        {
            maxRed = Convert.ToInt32(MaxRedSlider.Value);
        }

        private void MaxGreen_Changed(object sender, EventArgs e)
        {
            maxGreen = Convert.ToInt32(MaxGreenSlider.Value);
        }

        private void MaxBlue_Changed(object sender, EventArgs e)
        {
            maxBlue = Convert.ToInt32(MaxBlueSlider.Value);
        }

        private void MinRed_Changed(object sender, EventArgs e)
        {
            maxRed = Convert.ToInt32(MinRedSlider.Value);
        }

        private void MinGreen_Changed(object sender, EventArgs e)
        {
            maxGreen = Convert.ToInt32(MinGreenSlider.Value);
        }

        private void MinBlue_Changed(object sender, EventArgs e)
        {
            maxBlue = Convert.ToInt32(MinBlueSlider.Value);
        }

        #endregion

    }
}
