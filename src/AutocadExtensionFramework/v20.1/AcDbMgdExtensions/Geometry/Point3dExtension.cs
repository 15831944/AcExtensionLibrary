﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.AutoCAD.Geometry
{
   public static class Point3dExtension
    {
       public static Point2d ToPoint2D(this Point3d pnt)
       {
           return new Point2d(pnt.X, pnt.Y);
       }
    }
}
