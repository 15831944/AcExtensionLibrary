using System;
using System.Collections.Generic;
using System.Text;

namespace Autodesk.AutoCAD
{
   public class AcExtTransaction: IDisposable

    {
       private AcExtTransaction()
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
