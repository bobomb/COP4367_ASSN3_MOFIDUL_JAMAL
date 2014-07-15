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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;
using T_Objects;

namespace WpfTransducer
{
    class Functionality
    {

        public MainWindow mainWindow;
        public TargetWindow targetWindow;
        public TargetWindow genWindow;
        
        GeneticAlgorithm ga;
        OctoPolyArtEvaluator sombrero = new OctoPolyArtEvaluator();

        //TransformGroup canvasTransform;

        public Functionality()
        {

        }

        public void setup()
        {
            ga = new GeneticAlgorithm();
            // The population size is set to 1000 individuals of 1600 doubles each. 
            // Scince the doubles are used in groups of 8 to make ellipses this will result in 200 ellipses.
            ga.populate(100, 800, 0.0, 1.0);

            // The target window displays the target image.
            targetWindow = new TargetWindow();
            targetWindow.Height = OctoPolyArtEvaluator.targetBitmap.Height * 1.1;
            targetWindow.Width = OctoPolyArtEvaluator.targetBitmap.Width * 1.1;
            targetWindow.Background = new ImageBrush(OctoPolyArtEvaluator.targetBitmap);
            targetWindow.Show();

            // The target window displays the target image.
            genWindow = new TargetWindow();
            genWindow.Height = OctoPolyArtEvaluator.targetBitmap.Height * 1.1;
            genWindow.Width = OctoPolyArtEvaluator.targetBitmap.Width * 1.1;
            genWindow.Title = "GenWindow";
            genWindow.Show();

            //The main window is set to the same size as the target window
            mainWindow.Height = targetWindow.Height*3;
            mainWindow.Width = targetWindow.Width*3;

        }



        public string runGA(int count)
        {
            for (int i = 0; i < count; i++)
            {
                System.Diagnostics.Debug.WriteLine("iteration " + i);
                
                ga.scoreOfLastSolution = sombrero.evaluate(ga.solution);
            }
            return ga.bestScoreSoFar.ToString();
        }


        public string plotGA()
        {
            Canvas canvas = sombrero.GenotypeToPhenotype(ga.bestSolutionSoFar);

            //Rectangle rectangle = new Rectangle();
            //Canvas.SetLeft(rectangle, 0);
            //Canvas.SetTop(rectangle, 0);
            //rectangle.Width = OctoPolyArtEvaluator.targetBitmap.Width;
            //rectangle.Height = OctoPolyArtEvaluator.targetBitmap.Height;
            //rectangle.Stroke = Brushes.Black;
            //rectangle.StrokeThickness = 5.0;
            //canvas.Children.Add(rectangle);
            //mainWindow.Content = canvas;

            RenderTargetBitmap rtb=null;
            OctoPolyArtEvaluator.ConvertFEtoRTB(canvas,ref rtb);
            genWindow.Background = new ImageBrush(rtb);
            return "GA Plotted";
        }


        internal string randBackground()
        {
            string result = "";
            byte r = (byte)G.random.Next(256);
            byte g = (byte)G.random.Next(256);
            byte b = (byte)G.random.Next(256);
            mainWindow.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            result = "Back color changed to RGB(" + r.ToString() + "," + g.ToString() + "," + b.ToString() + ")";
            return result;
        }



    }
}
