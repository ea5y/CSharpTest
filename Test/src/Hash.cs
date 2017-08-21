//================================
//===Author: easy
//===Email: gopiny@live.com
//===Date: 2017-07-16 15:43
//================================

using System;
namespace Hash
{
	public class HashTest
	{
		private class CarID
		{
			//private string _preFix;
			//private int number;

			public void DisplayHashCode()
			{
				Console.WriteLine("HashCode: {0}", this.GetHashCode());
			}
		}

		public static void GetCarIdHashCode()
		{
			var carId = new CarID();
			carId.DisplayHashCode();
		}

		public static void Run()
		{
			GetCarIdHashCode();
		}
	}
}
