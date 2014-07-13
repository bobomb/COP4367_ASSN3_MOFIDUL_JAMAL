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
    class SpinSlider:T_Object
    {
        public override void update()
        {
            if (x < 50.0)
            {
                x++;
                y++;
            }
            rotateTransform.Angle++;
        }
    }
}
