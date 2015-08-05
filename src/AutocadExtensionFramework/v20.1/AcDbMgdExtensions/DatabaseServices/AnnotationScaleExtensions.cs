﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class AnnotationScaleExtensions
    {
        public static double GetScaleFactor(this AnnotationScale annoScale)
        {
            return (1.0 / annoScale.Scale);
        }
    }
}
