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

namespace WpfTransducer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ScaleTransform scaleTransform = new ScaleTransform();
        public TranslateTransform translateTransform = new TranslateTransform();
        public TransformGroup transformGroup = new TransformGroup();



        ControlWindow controlWindow;

        public double canvasHeight
        {
            get { return _canvas.Height; }
        }

        public double canvasWidth
        {
            get { return _canvas.Width; }
        }


        public Canvas canvas
        {
            get { return _canvas; }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            controlWindow = new ControlWindow();
            controlWindow.mainWindow = this;
            controlWindow.Show();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
        }

        public bool isNowClosing = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isNowClosing = true;
            if(!controlWindow.isNowClosing)controlWindow.Close();
        }
    }
}
