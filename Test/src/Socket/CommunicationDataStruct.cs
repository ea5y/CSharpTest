using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.CsharpTest
{
    public class BaseReqData
    {

    }

    public class BaseResData
    {

    }

    public class RegisterData : BaseReqData
    {
        public string Username;
        public string Password;
    }

    public class UserData 
    {
        public int UserId;
        public string NickName;
        public int Hp;
        public double PosX;
        public double PosY;
        public double PosZ;
    }

    public class LoginDataRes : BaseResData
    {
        public string SessionId;
        public UserData UserData;
    }
}
