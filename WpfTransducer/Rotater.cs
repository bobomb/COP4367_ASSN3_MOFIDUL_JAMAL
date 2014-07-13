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
    class Rotateter:T_Object
    {
        public override void update()
        {
            rotateTransform.Angle++;
        }
    }
}
