using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Easy.CsharpTest.ProtocolBuffers.Person.Types;
using Google.Protobuf;
using System.IO;
using LitJson;
using ProtoBuf;

namespace Easy.CsharpTest.ProtocolBuffers
{
    public class ProtocolBuffersTest
    {
        public static void Run()
        {
            //TestSerializeProtocol();
            //TestParseProtocol();
            //TestProtoContractAttribute();
            TestProtoContractAttributeDeserialize();
        }

        private static void TestSerializeProtocol()
        {
            Person john = new Person
            {
                Id = 1234,
                Name = "John Doe",
                Email = "jdoe@example.com",
                //Phones = { new Person.Types.PhoneNumber { Number = "555-4321", Type = Person.Types.PhoneType.Home } }
                Phones = { new PhoneNumber { Number = "555-4321", Type = PhoneType.Home} }
            };         
            using(var output = File.Create("john.bat"))
            {
                john.WriteTo(output);
            }
        }

        private static void TestParseProtocol()
        {
            Person john;
            using(var input = File.OpenRead("john.bat"))
            {
                john = Person.Parser.ParseFrom(input);
            }

            //Console.WriteLine("Person: {0}\n Id: {1}\n Name: {2}\n Email: {3}\n Phones:{4}\n", "john", john.Id, john.Name, john.Email, john.Phones);
            Console.WriteLine("Data: {0}", john);
        }

        [Serializable, ProtoContract]
        public class PersonNew
        {
            [ProtoMember(1)]
            public int Id;
            [ProtoMember(2)]
            public string Name;
            [ProtoMember(3)]
            public Address Address;
        }

        [Serializable, ProtoContract]
        public class Address
        {
            [ProtoMember(1)]
            public string Line1;
            [ProtoMember(2)]
            public string Line2;
        }

        private static void TestProtoContractAttribute()
        {
            var personNew = new PersonNew()
            {
                Id = 1,
                Name = "Gopuny",
                Address = new Address { Line1 = "Line1", Line2 = "Line2" }
            };
            using(var file = File.Create("PersonNew.bin"))
            {
                //ProtoBuf.Serializer.Serialize(file, personNew);
                //ProtoBuf.Serializer<PersonNew>(file, personNew);
                ProtoBuf.Serializer.Serialize<PersonNew>(file, personNew);
            }
        }

        private static void TestProtoContractAttributeDeserialize()
        {
            PersonNew john;
            using(var file = File.OpenRead("PersonNew.bin"))
            {
                john = ProtoBuf.Serializer.Deserialize<PersonNew>(file);
            }
            //Console.WriteLine("Data: {0}", person);
            Console.WriteLine("Person: \n Id: {0}\n Name: {1}\n Address: {2}\n", john.Id, john.Name, john.Address);

        }
    }
}
