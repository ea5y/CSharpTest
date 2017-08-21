using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace Easy.CsharpTest
{
    public class SocketClientTest
    {

        private static Socket _socket; 

        public static void Run()
        {
            CheckConnection();
            var data = new RegisterData();
            data.Username = "easy";
            data.Password = "123456";

			PackageHead head = new PackageHead();
			head.ActionId = 1010;
			head.MsgId = 2;

			RegisterData data2 = new RegisterData();
			data2.Username = "easy";
			data2.Password = "343434";
			var sendBytes = PackageFactory.Create(head, data2);

            Send(sendBytes);
            Reieve();
        }

        //1.Check  connection
        private static void CheckConnection()
        {
            if (_socket == null || !_socket.Connected)
                OpenConnection();
        }

        //2.Open connection
        private static void OpenConnection()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Connect("127.0.0.1", 9001);
            }
            catch(SocketException se)
            {
                _socket = null;
                var msg = string.Format("Error: {0}", se.Message);
                Console.WriteLine(msg);
            }
        }

        //3.Close connection
        private static void CloseConnection()
        {
            _socket.Close();
            _socket = null;
        }
        
        //4.Start heartbeat
        private static void BuildHeartbeat()
        {

        }

        //5.send
        private static void Send(byte[] data)
        {
            Console.WriteLine("===>Send");
            if(_socket == null)
            {
                Console.WriteLine("Error: socket is null!");
                return;
            }
            _socket.Send(data);
        }

        //6.recieve
        private static void Reieve()
        {
            byte[] prefix = new byte[4];
            int recnum = _socket.Receive(prefix);

            if(recnum == 4)
            {
                int dataLength = BitConverter.ToInt32(prefix, 0);
                byte[] data = new byte[dataLength];

                int recieveDataLength = _socket.Receive(data);

                var str = Encoding.ASCII.GetString(data, 0, recieveDataLength);
                Console.WriteLine("Res: {0}", str);
            }
        }
    }

    public class PackageHead
    {
        public int MsgId = 0;
        public int ActionId = 0;
    }

    public class BaseReqData
    {

    }

    public class RegisterData : BaseReqData
    {
        public string Username;
        public string Password;
    }

    public class PackageFactory
    {
        private static string _sendStr;
        private static int MsgCounter = 0;

        public static byte[] Create(PackageHead head, BaseReqData data)
        {
            WriteHead(head);
            WriteData(data);

            return WriteBytesLength();
        }

        public static byte[] Create(int actionId, BaseReqData data)
        {
            var head = new PackageHead() { ActionId = actionId, MsgId = ++MsgCounter };
            WriteHead(head);
            WriteData(data);
            return WriteBytesLength();
        }

        private static void WriteHead(PackageHead head)
        {
            _sendStr += string.Format("MsgId={0}&ActionId={1}", head.MsgId, head.ActionId);
        }

        private static void WriteData(BaseReqData data)
        {
            _sendStr += string.Format("&data={0}", JsonMapper.ToJson(data));
        }

        private static byte[] WriteBytesLength()
        {
            byte[] tempBytes = Encoding.ASCII.GetBytes(_sendStr);
            byte[] len = BitConverter.GetBytes(tempBytes.Length);
            byte[] resultBytes = new byte[tempBytes.Length + len.Length];
            Buffer.BlockCopy(len, 0, resultBytes, 0, len.Length);
            Buffer.BlockCopy(tempBytes, 0, resultBytes, len.Length, tempBytes.Length);
            return resultBytes;
        }
    }
}
