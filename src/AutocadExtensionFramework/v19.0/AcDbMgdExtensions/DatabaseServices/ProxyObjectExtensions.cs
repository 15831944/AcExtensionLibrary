﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class ProxyObjectExtensions
    {
        public static bool IsErasable(this ProxyObject proxy)
        {
            return (proxy.ProxyFlags & 1) == 1;
        }
    }
}
