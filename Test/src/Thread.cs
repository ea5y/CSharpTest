//================================
//===Author: easy
//===Email: gopiny@live.com
//===Date: 2017-07-22 15:19
//================================

using System;
using System.Threading;
using System.Reflection;
using System.IO;

namespace Easy.CsharpTest
{
	public class Message
	{
		public void ShowMessage()
		{
			string message = string.Format("Async threadId is :{0}", 
					Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine(message);

			for(int i = 0; i < 10; i++)
			{
				Thread.Sleep(300);
				Console.WriteLine("The number is: {0}", i);
			}
		}

		public void ShowMessage(object obj)
		{
			if(obj != null)
			{
				Person person = (Person)obj;
				string message = string.Format("\n{0}'s age is {1}!\nAsync threadId is: {2}",
						person.Name, person.Age, Thread.CurrentThread.ManagedThreadId);
				Console.WriteLine(message);
			}

			for(int i = 0; i < 10; i++)
			{
				Thread.Sleep(300);
				Console.WriteLine("The number is: {0}", i);
			}
		}
	}

	public class Person
	{
		public string Name;
		public int Age;
	}

	public class ThreadTest
	{
		public static void Run()
		{
			//TestThreadStart();
			//TestHangThread();
			//TestStopThread();
			//TestThreadPool();
			ShowDelegateClassMember();
		}
		public static void GetCurrentThreadInfo()
		{
			Thread thread = Thread.CurrentThread;
			thread.Name = "Main Thread"; 
			string threadMessage = string.Format("Thread ID: {0}\n Current AppDomainId: {1}\n" + 
					" Current ContextId: {2}\n Thread Name: {3}\n" +
					" Thread State: {4}\n Thread Priority: {5}\n",
					thread.ManagedThreadId, Thread.GetDomainID(), Thread.CurrentContext.ContextID,
					thread.Name, thread.ThreadState, thread.Priority);
			Console.WriteLine(threadMessage);
		}

		public static void TestThreadStart()
		{
			Console.WriteLine("Main threadId is: {0}", 
					Thread.CurrentThread.ManagedThreadId);
			Message message = new Message();
			Thread thread = new Thread(new ThreadStart(message.ShowMessage));
			thread.Start();
			Console.WriteLine("Do something ...");
			Console.WriteLine("Main thread working is complete!");
		}

		private static void TestParameterizedThreadStart()
		{
			Console.WriteLine("Main threadId is: " + Thread.CurrentThread.ManagedThreadId);

			Message message = new Message();
			Thread thread = new Thread(new ParameterizedThreadStart(message.ShowMessage));
			Person person = new Person();
			person.Name = "Jack";
			person.Age = 21;
			thread.Start(person);

			Console.WriteLine("Do something ...");
			Console.WriteLine("Main thread working is complete!");
		}

		private static void TestHangThread()
		{
			Console.WriteLine("Main threadId is: " + Thread.CurrentThread.ManagedThreadId);

			Message message = new Message();
			Thread thread = new Thread(new ThreadStart(message.ShowMessage));
			thread.IsBackground = true;
			thread.Start();

			Console.WriteLine("Do something ...");
			Console.WriteLine("Main thread working is complete!");
			Console.WriteLine("Main thread sleep!");

			//Thread.Sleep(5000);
			thread.Join();
		}

		private static void AsyncThread()
		{
			try
			{
				string message = string.Format("\nAsync threadId is: {0}",
						Thread.CurrentThread.ManagedThreadId);
				Console.WriteLine(message);

				for(int i = 0; i < 10; i++)
				{
					if(i >= 4)
					{
						Thread.CurrentThread.Abort(i);
					}
					Thread.Sleep(300);
					Console.WriteLine("The number is: {0}", i); 
				}
			}
			catch(ThreadAbortException ex)
			{
				if(ex.ExceptionState != null)
					Console.WriteLine(string.Format("Thread abort when the number is: {0}!",
								ex.ExceptionState.ToString()));
				Thread.ResetAbort();			
				Console.WriteLine("Thread ResetAbort!");
			}

			Console.WriteLine("Thread Close!");
		}

		private static void TestStopThread()
		{
			Console.WriteLine("Main threadId is: " +
					Thread.CurrentThread.ManagedThreadId);
			Thread thread = new Thread(new ThreadStart(AsyncThread));
			thread.IsBackground = true;
			thread.Start();

			thread.Join();
		}

		private static void ThreadMessage(string data)
		{
			string message = string.Format("{0}\n CurrentThread is {1}",
					data, Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine(message);
		}

		private static void AsyncCallback(object state)
		{
			Thread.Sleep(200);
			ThreadMessage("AsyncCallback");

			string data = (string)state;
			Console.WriteLine("Async thread do work!\n {0}", data);
		}

		private static void TestThreadPool()
		{
			ThreadPool.SetMaxThreads(1000, 1000);
			ThreadMessage("Start");
			ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncCallback), "Hello Elva");
			//Thread.Sleep(300);
			Console.ReadKey();
		}

		delegate void MyDelegate();
		private static void ShowDelegateClassMember()
		{
			MyDelegate delegate1 = new MyDelegate(AsyncThread);

			var methods = delegate1.GetType().GetMethods();
			if(methods != null)
				foreach(MethodInfo info in methods)
					Console.WriteLine(info.Name);
			Console.ReadKey();
		}

		public class LoginData
		{
			public string Host;
			public string Username;
			public string Passward;
		}

		private static void LoginTask(object data)
		{
			var loginData = data as LoginData;
			//chmod(loginData.Host, loginData.Username, loginData.Passward);
		}

		private static void Login()
		{
			ThreadPool.SetMaxThreads(1000, 1000);
			
			string host = "";
			using(StreamReader reader = new StreamReader(@"text1.txt"))
			{
				host = reader.ReadLine();
				while(host != "" && host != null)
				{
					//lines.Add(host);
					Console.WriteLine(host);
					host = reader.ReadLine();

					var loginData = new LoginData(){ Host = host, Username = "", Passward = ""};
					ThreadPool.QueueUserWorkItem(new WaitCallback(LoginTask), loginData);
				}
			}

			Console.ReadKey();
		}

		//====================================I/O Thread=========================================
		private static void IOThreadCallback(IAsyncResult result)
		{
			Thread.Sleep(200);
			IOThreadPoolMessage("AsyncCallback");

			FileStream stream = (FileStream)result.AsyncState;
			stream.EndWrite(result);
			stream.Close();
		}

		private static void IOThreadPoolMessage(string data)
		{
			int a,b;
			ThreadPool.GetAvailableThreads(out a, out b);
			string message = string.Format("{0}\n CurrentThread is {1}\n " +
					"WokerThreads is: {2} CompletionPortThreads is: {3}",
					data, Thread.CurrentThread.ManagedThreadId, a, b);
			Console.WriteLine(message);
		}

		private static void TestIOThreadWrite()
		{
			//ThreadPool.SetMaxThreads(
		}
	}

}
