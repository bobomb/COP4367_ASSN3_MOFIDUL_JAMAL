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

        int sampleSize = 1000;

        Canvas genCanvas = null;

        public ArtEvaluator()
        {
            targetBitmap = new BitmapImage(new Uri(@"..\..\..\..\VG03.jpg",UriKind.Relative));

            GetByteArrayFromBitmap(targetBitmap,ref targetPixelArray);

            pixelArrayStride = targetBitmap.PixelWidth * 4;

        }

        public double evaluate(List<double> solution)
        {
            GenotypeToPhenotype(solution);
            double fitness = FitnessFunction();
            return fitness;
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
        /*
        public Canvas GenotypeToPhenotypeShrinkingCircles2(List<double> solution)
        {
            // Make a new canvas of the same size as the target bitmap.
            if (genCanvas == null)
            {
                genCanvas = new Canvas();
                genCanvas.Width = targetBitmap.Width;
                genCanvas.Height = targetBitmap.Height;
            }

            genCanvas.Children.Clear();

            //The sizeFactor allows the phenotype to implement smaller and smaller ellipses as it implements the list of ellipses.
            double sizeFactor = solution.Count() / 400;


            // Here we loop through all the doubles in the solution 8 at a time.
            // Each time through the loop creates another ellipse.
            // The number of ellipses is controlled by the size of the solutions as set near line 
            for (int i = 0; i <= solution.Count - 8; i += 8)
            {
                // In this case we are using 8 doubles for each ellipse.
                // The position of the ellipse is at x,y.
                // The Height and Width is h and w.
                // The color is determined by the a,r,g and b
                double x = solution[i + 0];
                double y = solution[i + 1];
                double h = solution[i + 2];
                double w = solution[i + 3];
                double r = solution[i + 4];
                double g = solution[i + 5];
                double b = solution[i + 6];
                double a = solution[i + 7];


                // Normalize the values for position ,height and width.
                //x = (genCanvas.Width * 2 * x) - genCanvas.Width;
                //y = (genCanvas.Height * 2 * y) - genCanvas.Height;
                //h = clamp((genCanvas.Height * 0.5 * h), 0.0, genCanvas.Height);
                //w = clamp((genCanvas.Height * 0.5 * w), 0.0, genCanvas.Height); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?




                x = (genCanvas.Width * 1.2 * x) - (genCanvas.Width * 0.2);
                y = (genCanvas.Height * 1.2 * y) - (genCanvas.Height * 0.2);
                h = clamp((genCanvas.Height * 0.2 * sizeFactor * h), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor);
                w = clamp((genCanvas.Height * 0.2 * sizeFactor * w), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?


                sizeFactor *= 0.99;



                // Normalize and clamp values for colors.
                byte rb = Convert.ToByte(clamp(r * 256.0, 0, 255));
                byte gb = Convert.ToByte(clamp(g * 256.0, 0, 255));
                byte bb = Convert.ToByte(clamp(b * 256.0, 0, 255));
                byte ab = Convert.ToByte(clamp(a * 256.0, 0, 255));


                // Create an ellipse.
                Ellipse ellipse = new Ellipse();

                // This allows positioning the ellipse without the need for TranslationTransforms
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                // Set the size of the ellipse and the color.
                ellipse.Height = h;
                ellipse.Width = w;
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(ab, rb, gb, bb));

                // Add it to the canvas.
                genCanvas.Children.Add(ellipse);
            }

            return genCanvas;
        }
        */


        private double FitnessFunction()
        {
            //Convert the genCanvas to the genBitmap
            ConvertFEtoRTB(genCanvas, ref genBitmap);
            //Convert the genBitmap to the genPixelArray
            GetByteArrayFromBitmap(genBitmap, ref genPixelArray);

            double fitness = 0.0;

            // acumualte the fitness from all the sample points
            for (int i = 0; i < sampleSize; i++)
            {
                // Get the random sample point
                int rx = rand.Next(0, (int)targetBitmap.Width);
                int ry = rand.Next(0, (int)targetBitmap.Height);

                //Target Color Values
                byte tRed;
                byte tGreen;
                byte tBlue;
                byte tAlpha;

                //Generated Color Values
                byte gRed;
                byte gGreen;
                byte gBlue;
                byte gAlpha;

                // Get the Target color values
                getColorFromPixelArray(targetPixelArray, rx, ry, out tRed, out tGreen, out tBlue, out tAlpha);

                //Get the Generated color values
                getColorFromPixelArray(genPixelArray, rx, ry, out gRed, out gGreen, out gBlue, out gAlpha);

                //Ge the color differences
                double RedError = Math.Abs(tRed - gRed);
                double GreenError = Math.Abs(tGreen - gGreen);
                double BlueError = Math.Abs(tBlue - gBlue);

                ////square each of the color differences (optional but probably a good idea, as mentioned in class on July 9th 2014)
                RedError *= RedError;
                GreenError *= GreenError;
                BlueError *= BlueError;

                //get the total error for the current pixel (rx,ry).
                double pixelError = (RedError + GreenError + BlueError) / 256.0;

                //square the total difference 

                pixelError *= pixelError;

                //add that to all the other pixel errors.
                fitness += pixelError;
            }
            //normalize by taking the average pixel error.
            fitness /= sampleSize;

            //return the negative of the sum of all the pixel errors because the GA expects higher values to be better.
            return -fitness;
        }
        
        /*
        public Canvas GenotypeToPhenotypeShrinkingCircles(List<double> solution)
        {
            // Make a new canvas of the same size as the target bitmap.
            if (genCanvas == null)
            {
                genCanvas = new Canvas();
                genCanvas.Width = targetBitmap.Width;
                genCanvas.Height = targetBitmap.Height;
            }

            genCanvas.Children.Clear();

            //The sizeFactor allows the phenotype to implement smaller and smaller ellipses as it implements the list of ellipses.
            double sizeFactor = solution.Count() / 400;


            // Here we loop through all the doubles in the solution 8 at a time.
            // Each time through the loop creates another ellipse.
            // The number of ellipses is controlled by the size of the solutions as set near line 
            for (int i = 0; i <= solution.Count - 8; i += 8)
            {
                // In this case we are using 8 doubles for each ellipse.
                // The position of the ellipse is at x,y.
                // The Height and Width is h and w.
                // The color is determined by the a,r,g and b
                double x = solution[i + 0];
                double y = solution[i + 1];
                double h = solution[i + 2];
                double w = solution[i + 3];
                double r = solution[i + 4];
                double g = solution[i + 5];
                double b = solution[i + 6];
                double a = solution[i + 7];


                // Normalize the values for position ,height and width.
                //x = (genCanvas.Width * 2 * x) - genCanvas.Width;
                //y = (genCanvas.Height * 2 * y) - genCanvas.Height;
                //h = clamp((genCanvas.Height * 0.5 * h), 0.0, genCanvas.Height);
                //w = clamp((genCanvas.Height * 0.5 * w), 0.0, genCanvas.Height); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?




                x = (genCanvas.Width * 1.2 * x) - (genCanvas.Width * 0.2);
                y = (genCanvas.Height * 1.2 * y) - (genCanvas.Height * 0.2);
                h = clamp((genCanvas.Height * 0.2 * sizeFactor * h), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor);
                w = clamp((genCanvas.Height * 0.2 * sizeFactor * w), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?


                sizeFactor *= 0.99;



                // Normalize and clamp values for colors.
                byte rb = Convert.ToByte(clamp(r * 256.0, 0, 255));
                byte gb = Convert.ToByte(clamp(g * 256.0, 0, 255));
                byte bb = Convert.ToByte(clamp(b * 256.0, 0, 255));
                byte ab = Convert.ToByte(clamp(a * 256.0, 0, 255));


                // Create an ellipse.
                Ellipse ellipse = new Ellipse();

                // This allows positioning the ellipse without the need for TranslationTransforms
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                // Set the size of the ellipse and the color.
                ellipse.Height = h;
                ellipse.Width = w;
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(ab, rb, gb, bb));

                // Add it to the canvas.
                genCanvas.Children.Add(ellipse);
            }

            return genCanvas;
        }
*/

        public Canvas GenotypeToPhenotype(List<double> solution)
        {
            // Make a new canvas of the same size as the target bitmap.
            if (genCanvas == null)
            {
                genCanvas = new Canvas();
                genCanvas.Width = targetBitmap.Width;
                genCanvas.Height = targetBitmap.Height;
                for (int i = 0; i < solution.Count() / 8; i++)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = new SolidColorBrush();
                    genCanvas.Children.Add(ellipse);
                }
            }

            //genCanvas.Children.Clear();

            //The sizeFactor allows the phenotype to implement smaller and smaller ellipses as it implements the list of ellipses.
            //double sizeFactor = solution.Count() / 400;
            double sizeFactor = .50;
            int circleCount = 0;

            int solutionIndex=0;
            foreach (UIElement uie in genCanvas.Children)
            {
                Ellipse ellipse = uie as Ellipse;
                
                // In this case we are using 8 doubles for each ellipse.
                // The position of the ellipse is at x,y.
                // The Height and Width is h and w.
                // The color is determined by the a,r,g and b
                double x = solution[solutionIndex + 0];
                double y = solution[solutionIndex + 1];
                double h = solution[solutionIndex + 2];
                double w = solution[solutionIndex + 3];
                double r = solution[solutionIndex + 4];
                double g = solution[solutionIndex + 5];
                double b = solution[solutionIndex + 6];
                double a = solution[solutionIndex + 7];


                solutionIndex += 8;

                // Normalize the values for position ,height and width.
                //x = (genCanvas.Width * 2 * x) - genCanvas.Width;
                //y = (genCanvas.Height * 2 * y) - genCanvas.Height;
                //h = clamp((genCanvas.Height * 0.5 * h), 0.0, genCanvas.Height);
                //w = clamp((genCanvas.Height * 0.5 * w), 0.0, genCanvas.Height); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?




                x = (genCanvas.Width * 1.2 * x) - (genCanvas.Width * 0.2);
                y = (genCanvas.Height * 1.2 * y) - (genCanvas.Height * 0.2);
                h = clamp((genCanvas.Height * 0.2 * sizeFactor * h), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor);
                w = clamp((genCanvas.Height * 0.2 * sizeFactor * w), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?


                //sizeFactor *= 0.99;
                if (circleCount > 40) sizeFactor = 1.0;
                if (circleCount > 100) sizeFactor = 0.5;
                if (circleCount > 200) sizeFactor = 0.25;


                // Normalize and clamp values for colors.
                byte rb = Convert.ToByte(clamp(r * 256.0, 0, 255));
                byte gb = Convert.ToByte(clamp(g * 256.0, 0, 255));
                byte bb = Convert.ToByte(clamp(b * 256.0, 0, 255));
                byte ab = Convert.ToByte(clamp(a * 256.0, 0, 255));


 
                // This allows positioning the ellipse without the need for TranslationTransforms
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                // Set the size of the ellipse and the color.
                ellipse.Height = h;
                ellipse.Width = w;

                //ellipse.Fill = new SolidColorBrush(Color.FromArgb(ab, rb, gb, bb));

                SolidColorBrush scb = ellipse.Fill as SolidColorBrush;

                scb.Color = Color.FromArgb(ab, rb, gb, bb);

                circleCount++;
            }

            return genCanvas;
        }

        /*
        static public Canvas GenotypeToPhenotypeSimple(List<double> solution)
        {
            // Make a new canvas of the same size as the target bitmap.
            Canvas genCanvas = new Canvas();
            genCanvas.Width = targetBitmap.Width;
            genCanvas.Height = targetBitmap.Height;


            // Here we loop through all the doubles in the solution 8 at a time.
            // Each time through the loop creates another ellipse.
            // The number of ellipses is controlled by the size of the solutions as set near line 
            for (int i = 0; i <= solution.Count - 8; i += 8)
            {
                // In this case we are using 8 doubles for each ellipse.
                // The position of the ellipse is at x,y.
                // The Height and Width is h and w.
                // The color is determined by the a,r,g and b
                double x = solution[i + 0];
                double y = solution[i + 1];
                double h = solution[i + 2];
                double w = solution[i + 3];
                double r = solution[i + 4];
                double g = solution[i + 5];
                double b = solution[i + 6];
                double a = solution[i + 7];


                // Normalize the values for position ,height and width.
                //x = (genCanvas.Width * 2 * x) - genCanvas.Width;
                //y = (genCanvas.Height * 2 * y) - genCanvas.Height;
                //h = clamp((genCanvas.Height * 0.5 * h), 0.0, genCanvas.Height);
                //w = clamp((genCanvas.Height * 0.5 * w), 0.0, genCanvas.Height); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?

                x = (genCanvas.Width * 1.2 * x) - (genCanvas.Width * 0.2);
                y = (genCanvas.Height * 1.2 * y) - (genCanvas.Height * 0.2);
                h = clamp((genCanvas.Height * 0.2 * h), genCanvas.Height * 0.01, genCanvas.Height * 0.2);
                w = clamp((genCanvas.Height * 0.2 * w), genCanvas.Height * 0.01, genCanvas.Height * 0.2); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?


                // Normalize and clamp values for colors.
                byte rb = Convert.ToByte(clamp(r * 256.0, 0, 255));
                byte gb = Convert.ToByte(clamp(g * 256.0, 0, 255));
                byte bb = Convert.ToByte(clamp(b * 256.0, 0, 255));
                byte ab = Convert.ToByte(clamp(a * 256.0, 0, 255));


                // Create an ellipse.
                Ellipse ellipse = new Ellipse();

                // This allows positioning the ellipse without the need for TranslationTransforms
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                // Set the size of the ellipse and the color.
                ellipse.Height = h;
                ellipse.Width = w;
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(ab, rb, gb, bb));

                // Add it to the canvas.
                genCanvas.Children.Add(ellipse);
            }

            return genCanvas;
        }
        */


        public static void ConvertFEtoRTB(FrameworkElement visual, ref RenderTargetBitmap genBitmap)
        {
            double scaleFactor = 1.0;
            if (genBitmap == null)
            {
                genBitmap = new RenderTargetBitmap(
                    (int)(targetBitmap .Width * scaleFactor),
                    (int)(targetBitmap.Height * scaleFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);
            }
            genBitmap.Clear();
            Size size = new Size(targetBitmap.Width * scaleFactor, targetBitmap.Height * scaleFactor);

            visual.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);

            visual.Measure(size);
            visual.Arrange(new Rect(size));

            genBitmap.Render(visual);
        }

        //static void ConvertFEtoRTB(FrameworkElement visual, ref RenderTargetBitmap genBitmap)
        //{
        //    double scaleFactor = 1.0;
        //    if (genBitmap == null)
        //    {
        //        genBitmap = new RenderTargetBitmap(
        //            (int)(visual.Width * scaleFactor),
        //            (int)(visual.Height * scaleFactor),
        //            96,
        //            96,
        //            PixelFormats.Pbgra32);
        //    }
        //    genBitmap.Clear();
        //    Size size = new Size(visual.Width * scaleFactor, visual.Height * scaleFactor);

        //    visual.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);

        //    visual.Measure(size);
        //    visual.Arrange(new Rect(size));

        //    genBitmap.Render(visual);
        //}

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
