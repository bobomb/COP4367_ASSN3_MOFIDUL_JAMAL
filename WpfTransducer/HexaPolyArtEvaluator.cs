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
    public class HexaPolyArtEvaluator
    {
        static public BitmapImage targetBitmap;
        static public RenderTargetBitmap genBitmap=null;
        Random rand = new Random();

        byte[] targetPixelArray=null;
        byte[] genPixelArray=null;

        int pixelArrayStride;

        int sampleSize = 1000;

        Canvas genCanvas = null;

        public HexaPolyArtEvaluator()
        {
            targetBitmap = new BitmapImage(new Uri(@"..\..\..\..\ML01.jpg",UriKind.Relative));

            GetByteArrayFromBitmap(targetBitmap,ref targetPixelArray);

            pixelArrayStride = targetBitmap.PixelWidth * 4;

        }

        private double FitnessFunction()
        {
            //Convert the genCanvas to the genBitmap
            ConvertFEtoRTB(genCanvas, ref genBitmap);
            //Convert the genBitmap to the genPixelArray
            GetByteArrayFromBitmap(genBitmap, ref genPixelArray);

            double fitness = 0.0;
            int sampleSize = 0;

            //we will divide the input and output image into 10x10 squares and then take the average color of that square  by sampling
            //a 5 x 5 pixels evenly distributed in the square and compare
            for (int x = 0; x < (int)targetBitmap.Width-10; x+=10)
            {
                int targRed = 0;
                int targGreen = 0;
                int targBlue = 0;
                int genRed = 0;
                int genGreen = 0;
                int genBlue = 0;
                
                byte r, g, b, a;
                for(int y = 0; y < (int)targetBitmap.Height-10; y+=10)
                { 
                    targRed = 0;
                    targGreen = 0;
                    targBlue = 0;
                    genRed = 0;
                    genBlue = 0;
                    genGreen = 0;
                    //get the 25 sample points for this 10x10 block
                    for (int ix = x; ix < (x + 10); ix+=2)
                    {
                        for(int iy = y; iy < (y + 10); iy+=2)
                        {
                            getColorFromPixelArray(targetPixelArray, ix, iy, out r, out g, out b, out a);
                            targRed += r;
                            targGreen += g;
                            targBlue += b;
                            getColorFromPixelArray(genPixelArray, ix, iy, out r, out g, out b, out a);
                            genRed += r;
                            genGreen += g;
                            genBlue += b;
                        }
                    }

                    //average for the 10x10 block (using the 25 evenly spaced sample points)
                    targRed /= 25;
                    targBlue /= 25;
                    targGreen /= 25;
                    genRed /= 25;
                    genGreen /= 25;
                    genBlue /= 25;

                    //Get the color differences
                    double RedError = Math.Abs(targRed - genRed);
                    double GreenError = Math.Abs(targGreen - genGreen);
                    double BlueError = Math.Abs(targBlue - genBlue);

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
                    sampleSize++;
                } ///end of inner y loop
                  
            }//end of outer x loop

            //normalize by taking the average pixel error.
            fitness /= sampleSize;

            //return the negative of the sum of all the pixel errors because the GA expects higher values to be better.
            return -fitness;
        }
        
        public Canvas GenotypeToPhenotype(List<double> solution)
        {
            // Make a new canvas of the same size as the target bitmap.
            if (genCanvas == null)
            {
                genCanvas = new Canvas();
                genCanvas.Width = targetBitmap.Width;
                genCanvas.Height = targetBitmap.Height;
                for (int i = 0; i < solution.Count() / 16; i++)
                {
                    Polygon polygon = new Polygon();
                    //polygon.Fill = new LinearGradientBrush();
                    polygon.Fill = new SolidColorBrush();
                    genCanvas.Children.Add(polygon);
                }
            }
            

            //The sizeFactor allows the phenotype to implement smaller and smaller ellipses as it implements the list of ellipses.
            //double sizeFactor = solution.Count() / 400;
            double sizeFactor = .50;
            int circleCount = 0;

            int solutionIndex=0;
            foreach (UIElement uie in genCanvas.Children)
            {
                Polygon polygon = uie as Polygon;
                
                // In this case we are using 16 doubles for each 6-pointed polygon
                // The polygon has points at x1,y1 to x6,y6
                // The color is determined by the a,r,g and b
                
                double x1 = solution[solutionIndex + 0];
                double y1 = solution[solutionIndex + 1];
                double x2 = solution[solutionIndex + 2];
                double y2 = solution[solutionIndex + 3];
                double x3 = solution[solutionIndex + 4];
                double y3 = solution[solutionIndex + 5];
                double x4 = solution[solutionIndex + 6];
                double y4 = solution[solutionIndex + 7];
                double x5 = solution[solutionIndex + 8];
                double y5 = solution[solutionIndex + 9];
                double x6 = solution[solutionIndex + 10];
                double y6 = solution[solutionIndex + 11];
                double a = solution[solutionIndex + 12];
                double r = solution[solutionIndex + 13];
                double g = solution[solutionIndex + 14];
                double b = solution[solutionIndex + 15];


                solutionIndex += 16;

                // Normalize the values for position ,height and width.
                //x = (genCanvas.Width * 2 * x) - genCanvas.Width;
                //y = (genCanvas.Height * 2 * y) - genCanvas.Height;
                //h = clamp((genCanvas.Height * 0.5 * h), 0.0, genCanvas.Height);
                //w = clamp((genCanvas.Height * 0.5 * w), 0.0, genCanvas.Height); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?
                
                x1 = ArtEvaluator.clamp(x1, 0, genCanvas.Width);
                x2 = ArtEvaluator.clamp(x2, 0, genCanvas.Width);
                x3 = ArtEvaluator.clamp(x3, 0, genCanvas.Width);
                x4 = ArtEvaluator.clamp(x4, 0, genCanvas.Width);
                x5 = ArtEvaluator.clamp(x5, 0, genCanvas.Width);
                x6 = ArtEvaluator.clamp(x6, 0, genCanvas.Width);

                y1 = ArtEvaluator.clamp(y1, 0, genCanvas.Height);
                y2 = ArtEvaluator.clamp(y2, 0, genCanvas.Height);
                y3 = ArtEvaluator.clamp(y3, 0, genCanvas.Height);
                y4 = ArtEvaluator.clamp(y4, 0, genCanvas.Height);
                y5 = ArtEvaluator.clamp(y5, 0, genCanvas.Height);
                y6 = ArtEvaluator.clamp(y6, 0, genCanvas.Height);
                
                x1 = (genCanvas.Width * 1.2 * x1) - (genCanvas.Width * 0.4);
                y1 = (genCanvas.Height * 1.2 * y1) - (genCanvas.Height * 0.4);

                x2 = (genCanvas.Width * 1.4 * x2) - (genCanvas.Width * 0.4);
                y2 = (genCanvas.Height * 1.4 * y2) - (genCanvas.Height * 0.4);

                x3 = (genCanvas.Width * 1.8 * x3) - (genCanvas.Width * 0.6);
                y3 = (genCanvas.Height * 1.8 * y3) - (genCanvas.Height * 0.6);

                x4 = (genCanvas.Width * 1.2 * x4) - (genCanvas.Width * 0.4);
                y4 = (genCanvas.Height * 1.2 * y4) - (genCanvas.Height * 0.4);

                x5 = (genCanvas.Width * 1.4 * x5) - (genCanvas.Width * 0.8);
                y5 = (genCanvas.Height * 1.4 * y5) - (genCanvas.Height * 0.8);

                x6 = (genCanvas.Width * 1.2 * x6);// -(genCanvas.Width * 0.1);
                y6 = (genCanvas.Height * 1.2 * y6); //-(genCanvas.Height * 0.1);
                /*
                h = ArtEvaluator.clamp((genCanvas.Height * 0.2 * sizeFactor * h), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor);
                w = ArtEvaluator.clamp((genCanvas.Height * 0.2 * sizeFactor * w), genCanvas.Height * 0.01, genCanvas.Height * 0.2 * sizeFactor); //I used genCanvas.Height instead of genCanvas.Width. Can anyone figure out why?
                */

                // Normalize and clamp values for colors.
                byte rb = Convert.ToByte(ArtEvaluator.clamp(r * 256.0, 0, 255));
                byte gb = Convert.ToByte(ArtEvaluator.clamp(g * 256.0, 0, 255));
                byte bb = Convert.ToByte(ArtEvaluator.clamp(b * 256.0, 0, 255));
                byte ab = Convert.ToByte(ArtEvaluator.clamp(a * 256.0, 0, 128));

                polygon.Points.Clear();
                    polygon.Points.Add(new Point(x1, y1));
                    polygon.Points.Add(new Point(x2, y2));
                    polygon.Points.Add(new Point(x3, y3));
                    polygon.Points.Add(new Point(x4, y4));
                    polygon.Points.Add(new Point(x5, y5));
                    polygon.Points.Add(new Point(x6, y6));

                //ellipse.Fill = new SolidColorBrush(Color.FromArgb(ab, rb, gb, bb));

                SolidColorBrush scb = polygon.Fill as SolidColorBrush;

                scb.Color = Color.FromArgb(ab, rb, gb, bb);
            }

            return genCanvas;
        }

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

        void getColorFromPixelArray(byte[] pixelArray, int x, int y, out byte r, out byte g, out byte b, out byte a)
        {
            //System.Diagnostics.Debug.WriteLine("pix " + x + "," + y + pixelArray);
            int index = y * pixelArrayStride + 4 * x;
            r = pixelArray[index];
            g = pixelArray[index + 1];
            b = pixelArray[index + 2];
            a = pixelArray[index + 3];
        }

        public double evaluate(List<double> solution)
        {
            GenotypeToPhenotype(solution);
            double fitness = FitnessFunction();
            return fitness;
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
    }
}
