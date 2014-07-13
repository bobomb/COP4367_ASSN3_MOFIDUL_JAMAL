//*********************************************************
//
// (c) Copyright 2014 Dr. Thomas Fernandez
// 
// All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace WpfTransducer
{
    class G
    {
        static public Random random = new Random();
        static public double randDouble()
        {
            return random.NextDouble();
        }
        static public double randDouble(double max)
        {
            return randDouble() * max;
        }
        static public double randDouble(double min, double max)
        {
            return randDouble(max - min) + min;
        }

        internal static Point randPoint(double p1, double p2, double p3, double p4)
        {
            return new Point(randDouble(p1, p2), randDouble(p3, p4));
        }

        public static UIElement DeepCopy(UIElement element)
        {
            string shapestring = XamlWriter.Save(element);
            StringReader stringReader = new StringReader(shapestring);
            XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);
            return DeepCopyobject;
        }

    }
}