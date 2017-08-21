using System;
using System.Collections;
using System.Text;
using WhatsNewAttributes;

[assembly: SupportsWhatNewAttribute]
namespace VectorClass
{
    [LastModifiedAttribute("14 Feb 2010", "IEnumerable interface implemented " + 
            "So Vector can now be treated as a collection")]
    [LastModifiedAttribute("10 Feb 2010", "IEnumerable interface implemented " + 
            "So Vector now responds to format specifiers N and VE")]
    public class Vector : IFormattable, IEnumerable
    {
        public double X, Y, Z;
        public Vector(double x, double y, double z)
        {
            this.X = x; 
            this.Y = y;
            this.Z = z;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if(format == null)
            {
                return ToString();
            }

        }
    }
}
