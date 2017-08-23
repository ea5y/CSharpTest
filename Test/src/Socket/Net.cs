using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy.CsharpTest;

namespace Easy.CsharpTest
{
    public class Net
    {
        public static void Login(Action<BaseResData> callback)
        {
            var data = new RegisterData() { Username = "easy", Password = "443322" };
            var bytes = PackageFactory.Pack(1010, data, callback);
            SocketClient.Send(bytes);
        }
    }
}
