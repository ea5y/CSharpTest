﻿using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Easy.CsharpTest
{
    public class SocketClient
    {
        private static Socket _socket;
        private static Thread _recieveThread;

        private static byte[] _heartbeatBytes;
        private static Timer _heartbeatTimer;
        private const int _heartbeatInterval = 10000;

        public static Dictionary<int, PackageReqHead> SendDic = new Dictionary<int, PackageReqHead>();

        public static void Run()
        {
            Net.Login((res) =>
            {
                    //var data = res as LoginDataRes;
                    var data = res;
                Console.WriteLine("SessionId:{0}", data.SessionId);
                PackageReqHead.SessionId = data.SessionId;
                Console.WriteLine("Login callback!");
            });
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
            CreateSocketAndConnect();
            CreateHeartbeatTimer();
            CreateReceiveThread();
        }

        private static void CreateSocketAndConnect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Console.WriteLine("===>Connect");
                _socket.Connect("127.0.0.1", 9001);
            }
            catch (SocketException se)
            {
                CloseConnection();
                var msg = string.Format("Error: {0}", se.Message);
                Console.WriteLine(msg);
                throw se;
            }
        }

        private static void CreateHeartbeatTimer()
        {
            if (_heartbeatTimer == null)
            {
                BuildHeartbeatHeadPackage();
                _heartbeatTimer = new Timer(SendHeartbeat, null, _heartbeatInterval, _heartbeatInterval);
            }
        }

        private static void CreateReceiveThread()
        {
            if (_recieveThread == null)
            {
                _recieveThread = new Thread(new ThreadStart(Receive));
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
            if(_heartbeatTimer != null)
                _heartbeatTimer.Dispose();
        }

        //4.Start heartbeat
        private static void BuildHeartbeatHeadPackage()
        {
            _heartbeatBytes = PackageFactory.Pack(1, new BaseReqData());
        }

        //5.send
        public static void Send(byte[] data)
        {
            try
            {
                CheckConnection();
            }
            catch (SocketException se)
            {
                var msg = se.Message;
                Console.WriteLine(msg);
                return;
            }

            if (_socket == null)
                return;
            Console.WriteLine("===>Send");
            _socket.Send(data);
        }

        //6.recieve
        private static void Receive()
        {
            while (true)
            {
                if (_socket == null) break;
                if (_socket.Poll(5, SelectMode.SelectRead))
                {
                    try
                    {
                        int dataLen = GetResLength();
                        byte[] data = new byte[dataLen];
                        NetRead(data, dataLen);

                        PackageResHead headRes;
                        string strDataRes;
                        if (PackageFactory.Unpack(data, out headRes, out strDataRes))
                        {
                            OutputHeadRes(headRes);

                            PackageReqHead headReq;
                            if(SendDic.TryGetValue(headRes.MsgId, out headReq))
                            {
                                if(!GetError(headRes.StatusCode))
                                {
                                    headReq.callback(strDataRes);
                                    SendDic.Remove(headRes.MsgId);
                                    Console.WriteLine("SendDic count:{0}", SendDic.Count);
                                }
                            }
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        private static void OutputHeadRes(PackageResHead headRes)
        {
            var msg = string.Format(
                    "HeadRes: \n ActionId:{0},Des:{1},MsgId:{2},SesId:{3},Code{4},St:{5},UserId:{6}",
                    headRes.ActionId, 
                    headRes.Description, 
                    headRes.MsgId, 
                    headRes.SessionId,
                    headRes.StatusCode, 
                    headRes.StrTime,
                    headRes.UserId);
            Console.WriteLine(msg);
        }

        private static bool GetError(int code)
        {
            if (code == 0)
                return false;
            else
            {
                var msg = string.Format("ReqStatus: {0}", ErrorCode.Dic[code]);
                Console.WriteLine(msg);
                return true;
            }
        }
        
        private static int GetResLength()
        {
            byte[] prefix = new byte[4];
            var recnum = NetRead(prefix, 4);
            return BitConverter.ToInt32(prefix, 0);
        }

        private static int NetRead(byte[] data, int length)
        {
            int startIndex = 0;
            int recnum = 0;
            try
            {
                do
                {
                    int rev = _socket.Receive(data, startIndex, length - recnum, SocketFlags.None);
                    recnum += rev;
                    startIndex += rev;
                } while (recnum != length);
            }
            catch(SocketException se)
            {
                Console.WriteLine("Recieve Error: {0}", se.Message);
                CloseConnection();
                throw;
            }
            return recnum;
        }
    }
}
