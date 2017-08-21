//================================
//===Author: easy
//===Email: gopiny@live.com
//===Date: 2017-07-16 15:20
//================================

using System;
using System.Text;
namespace StringFormatTest
{
	public class StringFormatTest
	{
		public static void Run()
		{
			Vector v1 = new Vector(1, 32, 5);
			Vector v2 = new Vector(845.4, 54.3, -7.8);

			Console.WriteLine("\nIn IJK format, \nv1 is {0, 30:IJK}\nv2 is {1, 30:IJK}", v1, v2);
			Console.WriteLine("\nIn default format, \nv1 is {0, 30}\nv2 is {1, 30}", v1, v2);
			Console.WriteLine("\nin VE format\nv1 is {0, 30:VE}\nv2 is {1, 30:VE}", v1, v2);
			Console.WriteLine("\nNorms are:\nv1 is {0, 20:N}\nv2 is {1, 20:N}", v1, v2);
		}
		struct Vector: IFormattable
		{
			private double x,y,z;
			public Vector(double x, double y, double z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
			
			public string ToString(string format, IFormatProvider formatProvider)
			{
				if(format == null)
				{
					return ToString();
				}

				string formatUpper = format.ToUpper();

				switch(formatUpper)
				{
					case "N":
						return "|| " +  Norm().ToString() + " ||";
					case "VE":
						return String.Format("( {0:E}, {1:E}, {2:E} )", x, y, z);
					case "IJK":
						StringBuilder sb = new StringBuilder(x.ToString(), 30);
						sb.AppendFormat(" i + ");
						sb.AppendFormat(y.ToString());
						sb.AppendFormat(" j + ");
						sb.AppendFormat(z.ToString());
						sb.AppendFormat(" k");
						return sb.ToString();
					default:
						return ToString();
				}
			}

			public override string ToString()
			{
				return "( " + x + ", " + y + ", " + z + " )";
			}

			public double Norm()
			{
				return x * + y * y + z * z;
			}
		}

	}
}
