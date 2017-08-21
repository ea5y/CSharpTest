using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.CsharpTest
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FieldNameAttribute:Attribute 
    {
        private string _name;
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public FieldNameAttribute(string name)
        {
            this._name = name;
        }
    }

    public class AttritbuteTest
    {
        [FieldName("UserId", Comment = "This is the primary key field")]
        public static string UserId{ get; set; }

        public static void Run()
        {
            var obj = new AttritbuteTest();
            obj.TestAttribute();
        }

        private void TestAttribute()
        {
            UserId = "1009";
            bool isDefined = this.GetType().GetProperty("UserId").IsDefined(typeof(FieldNameAttribute), false);
            if(isDefined)
                Console.WriteLine("FieldNameAttribute is applied to type {0}", "UserId");
            else
            {
                Console.WriteLine("error!");
            }
        }
    }
}
