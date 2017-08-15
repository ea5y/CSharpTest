//================================
//===Author: easy
//===Email: gopiny@live.com
//===Date: 2017-07-25 23:22
//================================

using System;
using System.Collections.Generic;

namespace Easy.CsharpTest
{
	public class SortTest
	{
		public static void Run()
		{
			TestQuickSort();
		}

		private static void TestQuickSort()
		{
			var list = new List<int>(){3,2,1,4,5,6,4,8,9,0,7,6,5,4,4,3,22,44,66,43,64,2};
			//QuickSort(list, 0, list.Count - 1);
			BubbleSort(list);

			foreach(var item in list)
				Console.Write("{0},", item);
		}

		private static void QuickSort(List<int> list, int low, int high)
		{
			int pivot;
			if(low < high)
			{
				pivot = Partition(list, low, high);

				QuickSort(list, low, pivot - 1);
				QuickSort(list, pivot + 1, high);
			}
		}

		private static int Partition(List<int> list, int low, int high)
		{
			int pivotkey = list[low];
			while(low < high)
			{
				while(low < high && list[high] >= pivotkey)
					high--;
				Swap(list, low, high);
				while(low < high && list[low] <= pivotkey)
					low++;
				Swap(list, low, high);
			}
			return low;
		}

		private static void Swap(List<int> list, int low, int high)
		{
			var temp = list[high];
			list[high] = list[low];
			list[low] = temp;
		}
		
		private static void BubbleSort(List<int> list)
		{
			int i,j;
			for(i = 0; i < list.Count - 1; i++)
			{
				for(j = list.Count - 2; j >= i; j--)
				{
					if(list[j] > list[j+1])
					{
						Swap(list, j, j + 1);
					}
				}
			}
		}

	}
}

