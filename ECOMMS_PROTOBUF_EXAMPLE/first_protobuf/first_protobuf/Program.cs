using Google.Protobuf.Examples.AddressBook;
using Google.Protobuf;
using System;
using System.IO;
using ECOMMS_Participant;
using ECOMMS_Client;
using ECOMMS_Entity;
using NATS.Client;

namespace first_protobuf
{
    class Program
    {
        class PersonServiceParticipant : Participant
        {
            Person _person = null;
            public PersonServiceParticipant(string id) : 
                base(id, Role.Service, ECOMMS_Entity.Type.None)
            {
            }

            public override void set(string what, string payload, Msg message)
            {
                base.set(what, payload, message);

                switch(what)
                {
                    case "person":
                        _person = Person.Parser.ParseFrom(message.Data);
                        replyTo(message, "success");
                        break;
                }
            }

            public override void get(string what, Msg message)
            {
                base.get(what, message);

                switch(what)
                {
                    case "person":
                        ack(message);
                        byte[] bytes = _person.ToByteArray();
                        replyTo(message, bytes);

                        break;
                }
            }
        }

        class PersonClient : Client
        {
            public PersonClient(string id) :
                base(id, Role.Service, ECOMMS_Entity.Type.None)
            {
            }
        }

        static Person john = new Person
        {
            Id = 1234,
            Name = "stephen n eshelman",
            Email = "objectthink@comcast.net",
            Phones = { new Person.Types.PhoneNumber { Number = "555-4321", Type = Person.Types.PhoneType.Home } }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            byte[] bytes;

            bytes = Program.john.ToByteArray();

            var sne = Person.Parser.ParseFrom(bytes);

            var address = Guid.NewGuid().ToString();

            //create participant
            var personService = new PersonServiceParticipant(address);
            personService.connect();
            personService.init();

            //create client
            var personClient = new PersonClient(address);
            personClient.connect();
            personClient.init();

            personClient.doSet("person", bytes, (s) =>
            {
                Console.WriteLine(s);
            });

            personClient.doGet("person", new ResponseCompletionAdapter(
                ack:        ()      => { },
                nak:        (error) => { },
                response:   (data)  => 
                {
                    var person = Person.Parser.ParseFrom(data);

                    Console.WriteLine(person.Name);
                    Console.WriteLine(person);
                }
            ));

            Console.ReadKey();
        }
    }
}
