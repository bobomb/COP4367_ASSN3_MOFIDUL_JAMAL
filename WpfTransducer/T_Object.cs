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
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace T_Objects
{
    public abstract class T_Object
    {
        public FrameworkElement element
        {
            get { return _element; }
            set 
            {
                _element = value;
                rotateTransform = new RotateTransform();
                rotateTransform.CenterX = _element.Width / 2.0;
                rotateTransform.CenterY = _element.Height / 2.0;
                scaleTransform = new ScaleTransform();
                scaleTransform.CenterX = _element.Width / 2.0;
                scaleTransform.CenterY = _element.Height / 2.0;
                translateTransform = new TranslateTransform();
                transformGroup = new TransformGroup();
                transformGroup.Children.Add(rotateTransform);
                transformGroup.Children.Add(scaleTransform);
                transformGroup.Children.Add(translateTransform);
                if (canvasTransform != null)
                {
                    transformGroup.Children.Add(canvasTransform);
                }
                _element.RenderTransform = transformGroup;
                if (_canvas != null)
                {
                    _canvas.Children.Add(_element);
                }
                if (_effect != null)
                {
                    _element.Effect = _effect;
                }
            }
        }

        public double x
        {
            set 
            {
                _x = value;
                translateTransform.X = _x - element.Width / 2.0;
            }
            get { return _x; }
        }

        double _x;

        public double y
        {
            set 
            { 
                _y = value;
                translateTransform.Y = _y - element.Height / 2.0;
            }
            get { return _y; }
        }

        double _y;


        private FrameworkElement _element = null;


        public TransformGroup canvasTransform
        {
            get { return _canvasTransform; }
            set
            {
                _canvasTransform = value;
                if (transformGroup != null)
                {
                    transformGroup.Children.Add(_canvasTransform);
                }
            }
        }

        TransformGroup _canvasTransform = null;


        public RotateTransform rotateTransform
        {
            get;
            set;
        }

        public ScaleTransform scaleTransform
        {
            get;
            set;
        }

        public TranslateTransform translateTransform
        {
            get;
            set;
        }

        public TransformGroup transformGroup
        {
            get { return _transformGroup; }
            set { _transformGroup = value; }
        }

        private TransformGroup _transformGroup = null;

        public Effect effect
        {
            get
            {
                return _effect;
            }
            set
            {
                _effect = value;
                if(_element!=null)
                {
                    _element.Effect = _effect;
                }
            }
        }

        Effect _effect=null;

        public Canvas canvas
        {
            get
            {
                return _canvas;
            }
            set
            {
                _canvas=value;
                if (_element != null)
                {
                    _canvas.Children.Add(_element);
                }
            }
        }
        private Canvas _canvas = null;


        public static TransformGroup makeCanvasTransform(double x1, double y1, double x2, double y2, double u1, double v1, double u2, double v2, bool aspectRatioLock, double angle)
        {
            double scaleX = (u2 - u1) / (x2 - x1);
            double scaleY = (v2 - v1) / (y2 - y1); ;

            double extraX = 0.0f;
            double extraY = 0.0f;

            if (aspectRatioLock)
            {
                if (Math.Abs(scaleX) < Math.Abs(scaleY))
                {
                    double oldRange = (v2 - v1) / scaleY;
                    scaleY = Math.Abs(scaleX) * Math.Sign(scaleY);
                    double newRange = (v2 - v1) / scaleY;
                    extraY = newRange - oldRange;
                }
                else
                {
                    double oldRange = (u2 - u1) / scaleX;
                    scaleX = Math.Abs(scaleY) * Math.Sign(scaleX);
                    double newRange = (u2 - u1) / scaleX;
                    extraX = newRange - oldRange;
                }
            }





            ScaleTransform canvasScaleTransform = new ScaleTransform();
            RotateTransform canvasRotateTransform = new RotateTransform();
            TranslateTransform canvasTranslateTransform = new TranslateTransform();



            canvasScaleTransform.ScaleX = scaleX;
            canvasScaleTransform.ScaleY = scaleY;

            canvasRotateTransform.Angle = angle;

            //canvasTranslateTransform.X = -x1 * scaleX + extraX;
            //canvasTranslateTransform.Y = -y1 * scaleY + extraY;
            canvasTranslateTransform.X = (-x1+extraX/2.0) * scaleX;
            canvasTranslateTransform.Y = (-y1+extraY/2.0) * scaleY;

            TransformGroup canvasTransform = new TransformGroup();

            canvasTransform.Children.Add(canvasScaleTransform);
            canvasTransform.Children.Add(canvasRotateTransform);
            canvasTransform.Children.Add(canvasTranslateTransform);
            return canvasTransform;
        }


        public DispatcherTimer timer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Tick+=_timer_Tick;
                }
                return _timer;
            }

            set
            {
                _timer = value;
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            update();
        }


        private DispatcherTimer _timer = null;

        virtual public void update()
        {
        }

        static public void drawAxis(double x1, double y1, double x2, double y2, Canvas canvas, TransformGroup canvasTransform)
        {
            Line yAxis = new Line();
            yAxis.X1 = 0.0;
            yAxis.Y1 = y1;
            yAxis.X2 = 0.0;
            yAxis.Y2 = y2;

            yAxis.Stroke = Brushes.Red;
            yAxis.StrokeThickness = Math.Abs(x2-x1)/150.0;

            yAxis.RenderTransform = canvasTransform;
            canvas.Children.Add(yAxis);

            Line xAxis = new Line();
            xAxis.X1 = x1;
            xAxis.Y1 = 0.0;
            xAxis.X2 = x2;
            xAxis.Y2 = 0.0;

            xAxis.Stroke = Brushes.Orange;
            xAxis.StrokeThickness = Math.Abs(x2 - x1) / 150.0;

            xAxis.RenderTransform = canvasTransform;
            canvas.Children.Add(xAxis);
        }



    }
}
