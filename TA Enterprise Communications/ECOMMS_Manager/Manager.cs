using ECOMMS_Client;
using ECOMMS_Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ECOMMS_Manager
{
    public interface IClientFactory
    {
        IClient getClientFor(
            string address, 
            Role role, 
            ECOMMS_Entity.Type type,
            SubType subType);
    }
    public interface IManager
    {
    }
    public class Manager : Entity, IManager
    {
        IList<IClient> _clients = new List<IClient>();
        IClientFactory _clientFactory;

        public IList<IClient> clients { get { return _clients; } }
        public Manager(IClientFactory factory = null) : base("", Role.Service, ECOMMS_Entity.Type.Address)
        {
            _clientFactory = factory;
        }

        public override void init()
        {
            base.init();

            registerHeartbeatListener((s, a) =>
            {
                string heartbeat = Encoding.UTF8.GetString(a.Message.Data, 0, a.Message.Data.Length);

                var found_list = _clients.Where((participant) => participant.id.Equals(heartbeat)).ToList();

                //we dont have a client for the heartbeat we just saw
                //so create one
                if (found_list.Count == 0)
                {
                    IClient client;

                    //create a client for this participant and add to list
                    client = new Client(heartbeat, Role.None, ECOMMS_Entity.Type.None);
                    client.connect(server);
                    client.init();

                    //NEED TO MAKE INIT BLOCK UNTIL ITS DONE
                    Thread.Sleep(2000);

                    //use this base client to find out the role of the client that
                    //just connected

                    //if its an instrument then create an instrument client
                    //and add it
                    //if we have a factory instance then use it otherwise
                    //create a base class entity
                    if(client.role == Role.Instrument && _clientFactory == null)
                    {
                        var instrumentClient = new InstrumentClient(heartbeat, client.type);
                        instrumentClient.connect(server);
                        instrumentClient.init();

                        client = instrumentClient;
                    }
                    else if(_clientFactory != null)
                    {
                        IClient tempClient = _clientFactory.getClientFor(heartbeat, client.role, client.type, SubType.None);
                        if (tempClient  != null)
                        {
                            tempClient.connect(server);
                            tempClient.init();
                            client = tempClient;
                        }
                    }

                    _clients.Add(client);

                    client.addObserver(new ObserverAdapter((o, h) =>
                    {
                        if ((h as string) == "ONLINE_CHANGED")
                            if (!client.online)
                            {
                                _clients.Remove(client);
                                notify("CLIENTS_CHANGED");
                            }
                    }));

                    notify("CLIENTS_CHANGED");
                    notify("CONNECTED", client);
                }
            });
        }
    }
}
