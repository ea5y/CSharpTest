using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CsharpTest.WeakEventManagerTest
{
    public class CarInfoEventArgs : EventArgs
    {
        public string Car { get; set; }
        public CarInfoEventArgs(string car)
        {
            this.Car = car;
        }
    }

    public class CarDealer : IWeekEvent<CarInfoEventArgs>
    {
        public event EventHandler<CarInfoEventArgs> NewCarInfo = delegate { };

        public void DeleteEvent(EventHandler<CarInfoEventArgs> deliverEvent)
        {
            this.NewCarInfo -= deliverEvent;
        }

        public void NewCar(string car)
        {
            Console.WriteLine("CarDealer, new car {0}", car);
            if(NewCarInfo != null)
            {
                NewCarInfo(this, new CarInfoEventArgs(car));
            }
        }

        public void RegistEvent(EventHandler<CarInfoEventArgs> deliverEvent)
        {
            this.NewCarInfo += deliverEvent;
        }
    }

    public class Consumer : IWeakEventListener
    {
        private string name;
        public Consumer(string name)
        {
            this.name = name;
        }

        public void NewCarIsHere(object sender, CarInfoEventArgs e)
        {
            Console.WriteLine("{0}: car {1} is new", name, e.Car);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            NewCarIsHere(sender, e as CarInfoEventArgs);
            return true;
        }

        ~Consumer()
        {
            Console.WriteLine("Consumer " + name + " finalized");
        }
    }


    public class TestArgs_1 : EventArgs
    {
        public string TestArg { get; set; }
        public TestArgs_1(string str)
        {
            this.TestArg = str;
        }
    }

    public class TestDealer_1 : IWeekEvent<TestArgs_1>
    {
        public event EventHandler<TestArgs_1> Event = (s,e) =>{};//delegate { };
        public string TestArg = "";
        public void DeleteEvent(EventHandler<TestArgs_1> deliverEvent)
        {
            this.Event -= deliverEvent;
        }

        public void RegistEvent(EventHandler<TestArgs_1> deliverEvent)
        {
            this.Event += deliverEvent;
        }

        public void Test(string str)
        {
            this.TestArg = str;
            Console.WriteLine("Dealer_1: " + str);
            Event(this, new TestArgs_1(str));
        }
    }

    public class TestListener_1 : IWeakEventListener
    {
        public void Listener(object sender, TestArgs_1 args)
        {
            Console.WriteLine("Listener_1 : " + args.TestArg);
            Console.WriteLine((sender as TestDealer_1).TestArg);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            this.Listener(sender, e as TestArgs_1);
            return true;
        }

        ~TestListener_1()
        {
            Console.WriteLine("Listener_1: finalized");
        }
    }

    public interface IWeekEvent<Args> where Args : EventArgs
    {
        void RegistEvent(EventHandler<Args> deliverEvent);
        void DeleteEvent(EventHandler<Args> deliverEvent);
    }

    public class WeakEventManager<Dealer, Args> : WeakEventManager where Dealer : IWeekEvent<Args> where Args : EventArgs
    {
        public static void AddListener(object source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(object source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        public static WeakEventManager<Dealer, Args> CurrentManager 
        {
            get
            {
                WeakEventManager<Dealer, Args> manager = GetCurrentManager(typeof(WeakEventManager<Dealer, Args>)) as WeakEventManager<Dealer, Args>;
                if(manager == null)
                {
                    manager = new WeakEventManager<Dealer, Args>();
                    SetCurrentManager(typeof(WeakEventManager<Dealer, Args>), manager);
                }
                return manager;
            }
        }

        protected override void StartListening(object source)
        {
            ((Dealer)source).RegistEvent(DeliverEvent);
        }

        protected override void StopListening(object source)
        {
            ((Dealer)source).DeleteEvent(DeliverEvent);
        }
    }

    public class WeekEventManagerTest
    {
        public static void Test()
        {
            TestBetter();

            Console.ReadKey();
        }

        public static void TestBetter()
        {
            Console.WriteLine("====== Test Better ======");
            var dealer = new CarDealer();

            var michael = new Consumer("Michael");
            WeakEventManager<CarDealer, CarInfoEventArgs>.AddListener(dealer, michael);
            dealer.NewCar("Mercedes");

            var nick = new Consumer("Nick");
            WeakEventManager<CarDealer, CarInfoEventArgs>.AddListener(dealer, nick);
            dealer.NewCar("Ferrari");

            WeakEventManager<CarDealer, CarInfoEventArgs>.RemoveListener(dealer, michael);
            dealer.NewCar("Toyota");

            var dealer_1 = new TestDealer_1();
            var listener_1 = new TestListener_1();
            WeakEventManager<TestDealer_1, TestArgs_1>.AddListener(dealer_1, listener_1);
            dealer_1.Test("Hello");

            Console.WriteLine("Setting listener to null");
            michael = null;
            listener_1 = null;
            TriggerGC();

            dealer.NewCar("Mercedes");
            Console.WriteLine("Setting source to null");
            dealer = null;
            TriggerGC();

        }

        public static void TestBad()
        {
            Console.WriteLine("====== Test Bad ======");
            var dealer = new CarDealer();
            var michael = new Consumer("Michael");
            dealer.NewCarInfo += michael.NewCarIsHere;
            dealer.NewCar("Toyota");

            Console.WriteLine("Setting listener to null");
            michael = null;
            TriggerGC();

        }

        public static void TriggerGC()
        {
            Console.WriteLine("Starting GC.");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.WriteLine("GC finished.");
        }
    }
}
