using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LitJson;


namespace Easy.CsharpTest.TT
{
    public class SocketClientTest
    {

        private static Socket _socket;
        private static byte[] _heartbeatBytes;
        private static Timer _heartbeatTimer;
        private static int _heartbeatInterval = 10000;

        private static Thread _recieveThread;

        public static void Run()
        {
            for(int i = 0; i < 30; i++)
            {
                RegisterData data2 = new RegisterData();
                data2.Username = "easy";
                data2.Password = "343434";
                var sendBytes = PackageFactory.Create(1010, data2);
                Send(sendBytes);
            }
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
                Console.WriteLine("===>Connect");
                _socket.Connect("127.0.0.1", 9001);
            }
            catch(SocketException se)
            {
                _socket = null;
                var msg = string.Format("Error: {0}", se.Message);
                Console.WriteLine(msg);
                throw se;
            }

            if(_heartbeatTimer == null)
            {
                BuildHeartbeatHeadPackage();
                _heartbeatTimer = new Timer(SendHeartbeat, null, _heartbeatInterval, _heartbeatInterval);
            }

            if(_recieveThread == null)
            {
                _recieveThread = new Thread(new ThreadStart(Reieve));
                _recieveThread.Start();
            }
        }

        private static void SendHeartbeat(object state)
        {
            Send(_heartbeatBytes);
        }

        //3.Close connection
        private static void CloseConnection()
        {
            _socket.Close();
            _socket = null;
            _heartbeatTimer.Dispose();
        }
        
        //4.Start heartbeat
        private static void BuildHeartbeatHeadPackage()
        {
            _heartbeatBytes = PackageFactory.Create(1, new BaseReqData());
        }

        //5.send
        private static void Send(byte[] data)
        {
            try
            {
                CheckConnection();
            }
            catch(SocketException se)
            {
                return;
            }

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
            while(true)
            {
                if (_socket == null) continue;
                if (_socket.Connected && _socket.Poll(5, SelectMode.SelectRead))
                {
                    byte[] prefix = new byte[4];
                    int recnum = 0;
                    try
                    {
                        recnum = _socket.Receive(prefix);
                        Console.WriteLine("RecieveNum: {0}", recnum);
                    }
                    catch(SocketException se)
                    {
                        Console.WriteLine("Recieve: {0}", se.Message);
                        CloseConnection();
                    }

                    if (recnum == 4)
                    {
                        int dataLength = BitConverter.ToInt32(prefix, 0);
                        byte[] data = new byte[dataLength];
                        int startIndex = 0;
                        recnum = 0;
                        do
                        {
                            int rev = _socket.Receive(data, startIndex, dataLength - recnum, SocketFlags.None);
                            recnum += rev;
                            startIndex += rev;
                        } while (recnum != dataLength);

                        PackageResHead head;
                        byte[] bodyBytes;
                        if(Unpack(data, out head, out bodyBytes))
                        {
                            var str = Encoding.ASCII.GetString(bodyBytes, 0, bodyBytes.Length);
                            Console.WriteLine("Res:\nStatus:{0},MsgId:{1},Description:{2},ActionId:{3},StrTime:{4},Body:{5}",
                                head.StatusCode, head.MsgId, head.Description, head.ActionId, head.StrTime, str);
                        }
                    }
                }
                
                Thread.Sleep(100);
            }
        }

        public static bool Unpack(byte[] data, out PackageResHead head, out byte[] bodyBytes)
        {
            head = null;
            bodyBytes = null;

            int pos = 0;
            int dataLength = GetInt(data, ref pos);
            if (dataLength != data.Length)
            {
                return false;
            }

            head = new PackageResHead();
            head.StatusCode = GetInt(data, ref pos);
            head.MsgId = GetInt(data, ref pos);
            head.Description = GetString(data, ref pos);
            head.ActionId = GetInt(data, ref pos);
            head.StrTime = GetString(data, ref pos);
            //int bodyLen = data.Length - pos;
            int bodyLen = GetInt(data, ref pos);
            if (bodyLen > 0)
            {
                bodyBytes = new byte[bodyLen];
                Buffer.BlockCopy(data, pos, bodyBytes, 0, bodyLen);
            }
            else
            {
                bodyBytes = new byte[0];
            }
            return true;
        }

        private static int GetInt(byte[] data, ref int pos)
        {
            int val = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            return val;
        }

        private static string GetString(byte[] data, ref int pos)
        {
            string val = string.Empty;
            int len = GetInt(data, ref pos);
            if (len > 0)
            {
                val = Encoding.UTF8.GetString(data, pos, len);
                pos += len;
            }
            return val;
        }
    }

    public class PackageReqHead
    {
        public int MsgId = 0;
        public int ActionId = 0;
    }

    public class PackageResHead
    {
        public int StatusCode;
        public string Description;
        public int ActionId;
        public int MsgId;
        public string SessionId;
        public int UserId;
        public string StrTime;
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

        public static byte[] Create(PackageReqHead head, BaseReqData data)
        {
            WriteHead(head);
            WriteData(data);

            return WriteBytesLength();
        }

        public static byte[] Create(int actionId, BaseReqData data)
        {
            var head = new PackageReqHead() { ActionId = actionId, MsgId = ++MsgCounter };
            WriteHead(head);
            WriteData(data);
            Console.WriteLine("Send: {0}", _sendStr);
            var bytes = WriteBytesLength();

            _sendStr = "";
            return bytes;
        }

        private static void WriteHead(PackageReqHead head)
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
