//================================
//===Author: easy
//===Email: gopiny@live.com
//===Date: 2017-07-15 11:11
//================================

using System;
using System.Windows;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace Easy.CsharpTest
{
	
	public class Test
	{
		static void Main()
		{
			Console.WriteLine("Hello world");
            //StringFormatTest.StringFormatTest.Run();
            //Hash.HashTest.Run();
            //TestSerializable();
            //SerializableTest.Run();
            //ThreadTest.Run();
            //SortTest.Run();
            //TT.SocketClientTest.Run();
            //SocketClient.Run();
            var http = new HttpClient();
            http.Run();
            //AttritbuteTest.Run();
            //ProtocolBuffers.ProtocolBuffersTest.Run();
            Console.ReadKey();
		}

		public static void TestSerializable()
		{
			TestSimpleObject obj = new TestSimpleObject();

			Console.WriteLine("Before serialization the object contains: ");
			obj.Print();

			Stream stream = File.Open("data.xml", FileMode.Create);
			SoapFormatter formatter = new SoapFormatter();

			formatter.Serialize(stream, obj);
			stream.Close();

			obj = null;

			stream = File.Open("data.xml", FileMode.Open);
			formatter = new SoapFormatter();

			obj = (TestSimpleObject)formatter.Deserialize(stream);
			stream.Close();

			Console.WriteLine("");
			Console.WriteLine("After deserialization the object contains: ");
			obj.Print();
		}

		[SerializableAttribute]
		public class TestSimpleObject
		{
			public int member1;
			public string member2;
			public string member3;
			public double member4;

			[NonSerializedAttribute]
			public string member5;

			public TestSimpleObject()
			{
				member1 = 11;
				member2 = "hello";
				member3 = "hello";
				member4 = 3.1415926;
				member5 = "hello world!";
			}

			public void Print()
			{
				Console.WriteLine("member1 = {0}", member1);
				Console.WriteLine("member2 = {0}", member2);
				Console.WriteLine("member3 = {0}", member3);
				Console.WriteLine("member4 = {0}", member4);
				Console.WriteLine("member5 = {0}", member5);
			}
		}
	}
}
