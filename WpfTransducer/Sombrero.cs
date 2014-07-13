//*********************************************************
//
// (c) Copyright 2014 Dr. Thomas Fernandez
// 
// All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTransducer
{
    class ArtEvaluator : EvaluatorForDoubles
    {

        static public BitmapImage targetBitmap;
        static public RenderTargetBitmap genBitmap=null;
        Random rand = new Random();

        byte[] targetPixelArray=null;
        byte[] genPixelArray=null;

        int pixelArrayStride;

        int sampleSize = 100;

        public ArtEvaluator()
        {
            targetBitmap = new BitmapImage(new Uri(@"C:\Users\Tom\Dropbox\Projects\Classes\GAD\WpfTransducerWithGA Backups\WpfTransducerWithGA 140702a\ML01.jpg"));

            GetByteArrayFromBitmap(targetBitmap,ref targetPixelArray);

            pixelArrayStride = targetBitmap.PixelWidth * 4;

        }


        void getColorFromPixelArray(byte[] pixelArray, int x, int y, out byte r, out byte g, out byte b, out byte a)
        {
            int index = y * pixelArrayStride + 4 * x;
            r = pixelArray[index];
            g = pixelArray[index + 1];
            b = pixelArray[index + 2];
            a = pixelArray[index + 3];
        }


        private void GetByteArrayFromBitmap(BitmapSource b, ref byte[] bArray)
        {
            int size; 
            if (bArray == null)
            {
                pixelArrayStride = b.PixelWidth * 4;
                size = b.PixelHeight * pixelArrayStride;
                bArray = new byte[size];
            }
            b.CopyPixels(bArray, pixelArrayStride, 0);
        }


        static public double clamp(double v, double min, double max)
        {
            if (v < min) v = min;
            if (v > max) v = max;
            return v;
        }


        
        public double evaluate(List<double> solution)
        {
            Canvas genCanvas = GenotypeToPhenotype(solution);
            return FitnessFunction(genCanvas);
        }

        private double FitnessFunction(Canvas genCanvas)
        {
            ConvertFEtoRTB(genCanvas, ref genBitmap);
            GetByteArrayFromBitmap(genBitmap, ref genPixelArray);




            double fitness = 0.0;



            for (int i = 0; i < sampleSize; i++)
            {
                int rx = rand.Next(0, (int)targetBitmap.Width);
                int ry = rand.Next(0, (int)targetBitmap.Height);

                byte tRed;
                byte tGreen;
                byte tBlue;
                byte tAlpha;

                byte gRed;
                byte gGreen;
                byte gBlue;
                byte gAlpha;


                getColorFromPixelArray(targetPixelArray, rx, ry, out tRed, out tGreen, out tBlue, out tAlpha);

                getColorFromPixelArray(genPixelArray, rx, ry, out gRed, out gGreen, out gBlue, out gAlpha);

                double RedError = Math.Abs(tRed - gRed);
                double GreenError = Math.Abs(tGreen - gGreen);
                double BlueError = Math.Abs(tBlue - gBlue);

                //square the errors
                RedError *= RedError;
                GreenError *= GreenError;
                BlueError *= BlueError;

                double pixelError = RedError + GreenError + BlueError;

                fitness += pixelError;
            }
            fitness /= sampleSize;
            return -fitness;
        }

        static public Canvas GenotypeToPhenotype(List<double> solution)
        {
            Canvas genCanvas = new Canvas();
            genCanvas.Width = targetBitmap.Width;
            genCanvas.Height = targetBitmap.Height;
            for (int i = 0; i <= solution.Count - 8; i += 8)
            {
                double x = solution[i + 0];
                double y = solution[i + 1];
                double h = solution[i + 2];
                double w = solution[i + 3];
                double r = solution[i + 4];
                double g = solution[i + 5];
                double b = solution[i + 6];
                double a = solution[i + 6];


                x = (genCanvas.Width*2*x)-genCanvas.Width;
                y = (genCanvas.Height*2*y)-genCanvas.Height;
                h *= genCanvas.Height / 2.0;
                w *= genCanvas.Height / 2.0;

                byte rb = Convert.ToByte(clamp(r * 256.0, 0, 255));
                byte gb = Convert.ToByte(clamp(g * 256.0, 0, 255));
                byte bb = Convert.ToByte(clamp(b * 256.0, 0, 255));
                byte ab = Convert.ToByte(clamp(a * 256.0, 0, 255));

                Brush brush = new SolidColorBrush(Color.FromArgb(ab, rb, gb, bb));

                Ellipse ellipse = new Ellipse();
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                ellipse.Height = clamp(h, 0.0, genCanvas.Height);
                ellipse.Width = clamp(w, 0.0, genCanvas.Width);
                ellipse.Fill = brush;

                genCanvas.Children.Add(ellipse);
            }
            return genCanvas;
        }



        static void ConvertFEtoRTB(FrameworkElement visual, ref RenderTargetBitmap genBitmap)
        {
            double scaleFactor = 1.0;
            if (genBitmap == null)
            {
                genBitmap = new RenderTargetBitmap(
                    (int)(visual.Width * scaleFactor),
                    (int)(visual.Height * scaleFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);
            }
            genBitmap.Clear();
            Size size = new Size(visual.Width * scaleFactor, visual.Height * scaleFactor);

            visual.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);

            visual.Measure(size);
            visual.Arrange(new Rect(size));

            genBitmap.Render(visual);
        }

        //static RenderTargetBitmap ConvertFEtoRTB(FrameworkElement visual)
        //{
        //    double scaleFactor = 1.0;
        //    RenderTargetBitmap bitmap = new RenderTargetBitmap(
        //        (int)(visual.ActualWidth * scaleFactor),
        //        (int)(visual.ActualHeight * scaleFactor),
        //        96,
        //        96,
        //        PixelFormats.Pbgra32);

        //    Size size = new Size(visual.ActualWidth * scaleFactor, visual.ActualHeight * scaleFactor);

        //    //visual.RenderTransform = new ScaleTransform(2.0, 2.0);
        //    visual.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);

        //    visual.Measure(size);
        //    visual.Arrange(new Rect(size));

        //    bitmap.Render(visual);
        //    return bitmap;
        //}
    }
}
