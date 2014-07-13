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

namespace T_Objects
{
    class Pulser : T_Object
    {
        double scaleFactor=1.3;
        double scaleLimit = 3.0;
        public override void update()
        {
            scaleTransform.ScaleX = scaleTransform.ScaleX * scaleFactor;
            scaleTransform.ScaleY = scaleTransform.ScaleY * scaleFactor;
            if (scaleTransform.ScaleX > scaleLimit) scaleFactor = 1/scaleFactor;
            if (scaleTransform.ScaleX <= 0.5 ) scaleFactor = 1/scaleFactor;
        }

    }
}
