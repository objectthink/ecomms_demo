using NATS.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECOMMS_Entity
{
    #region observer
    /// <summary>
    /// define a simple observer pattern interface ab implementation
    /// that can be duplicated across all supported languages
    /// </summary>
    public interface IObserver
    {
        void update(IObservable observable, object hint);
    }

    /// <summary>
    /// define an observer that takes data along with hint in update
    /// </summary>
    public interface IObserverEx
    {
        void update(IObservableEx observable, object hint, object data);
    }

    /// <summary>
    /// observable interface
    /// </summary>
    public interface IObservable
    {
        void addObserver(IObserver observer);
        void removeObserver(IObserver observer);
        void notify(string hint);
    }

    public interface IObservableEx : IObservable
    {
        void addObserver(IObserverEx observer);
        void removeObserver(IObserverEx observer);
        void notify(string hint, object data);
    }

    /// <summary>
    /// observer adapter
    /// create an instance and pass to addObserver
    /// </summary>
    public class ObserverAdapter : IObserver
    {
        Action<IObservable,object> _observerWrapper;
        public ObserverAdapter(Action<IObservable, object> ow)
        {
            _observerWrapper = ow;
        }
        public void update(IObservable observable, object hint)
        {
            _observerWrapper(observable, hint);
        }
    }

    /// <summary>
    /// observer adapter
    /// create an instance and pass to addObserver
    /// </summary>
    public class ObserverAdapterEx : IObserverEx
    {
        Action<IObservableEx, object, object> _observerWrapper;
        public ObserverAdapterEx(Action<IObservableEx, object, object> ow)
        {
            _observerWrapper = ow;
        }
        public void update(IObservableEx observable, object hint, object data)
        {
            _observerWrapper(observable, hint, data);
        }
    }

    #endregion

    #region enums
    public enum Role
    {
        None = 0,
        Instrument,
        Equipment,
        Service
    }

    public enum Type
    {
        None = 0,

        //instrument types
        Rheology,
        Thermal,

        //equipment types
        Balance,
        BarCodeReader,

        //service types
        Address,
        PersistentStore,
        Calculator
    }

    public enum SubType
    {
        None = 0,

        //thermal instrument sub types
        DSC,
        TGA,
        SDT

        //rheology instrument sub types
    }
    #endregion

    #region response completion
    /// <summary>
    /// define an interface for response completion
    /// that is easily created inline
    /// </summary>
    public interface IResponseCompletion
    {
        Action ack { get; set; }
        Action<int> nak { get; set; }
        Action<byte[]> response { get; set; }
    }

    /// <summary>
    /// concrete response completion that implements interface
    /// inherit from here to implement only those callbacks
    /// you are interested in
    /// </summary>
    public class ResponseCompletionAdapter : IResponseCompletion
    {
        public Action ack               { get; set; }
        public Action<int> nak          { get; set;  }
        public Action<byte[]> response  { get; set; }

        public ResponseCompletionAdapter(Action ack = null, Action<int> nak = null, Action<byte[]> response = null)
        {
            this.ack = ack;
            this.nak = nak;
            this.response = response;
        }
    }
    #endregion

    #region IEntity
    /// <summary>
    /// entity has a role, type, and id
    /// </summary>
    public interface IEntity : IObservable, IObserver, IObservableEx, IObserverEx
    {
        string server { get; }
        bool connect(string natsip = null);
        string request(string subject, string payload);
        byte[] request(string subject, byte[] payload);
        void request(string subject, string payload, Action<string> action);
        void request(string subject, byte[] payload, Action<string> action);
        void request(string subject, byte[] payload, IResponseCompletion callbacks);
        void protocolRequest(string subject, string data, IResponseCompletion completion = null);
        void protocolRequest(string subject, byte[] data, IResponseCompletion completion = null);
        Role role { get; }
        Type type { get; }
        SubType subType { get; }
        String id { get; }
        String name { get; }
        void registerHeartbeatListener(EventHandler<MsgHandlerEventArgs> action);
    }
    #endregion

    #region Entity
    /// <summary>
    /// nats entity
    /// WIP will include items in implementation that are being tested
    /// that will be moved to the interface
    /// 
    /// Entity includes methods that will be used by Clients and Participants
    /// it is important to note that those methods that use the id in a message
    /// subject may be participant publishing as itself or a client
    /// subscribing to its associated participant's messages
    /// </summary>
    public class Entity : IEntity
    {
        #region properties
        string _server;
        IConnection _connection;
        public string server { get { return _server; } }

        public Role role { get; protected set; }
        public Type type { get; protected set; }
        public SubType subType { get; protected set; }
        public String id { get; protected set; }
        public String name { get; protected set; }
        #endregion

        #region constructor
        public Entity(string anid, Role arole, Type atype, String aname = null)
        {
            id = anid;
            role = arole;
            type = atype;
            name = aname;
        }
        #endregion

        #region connect and init
        /// <summary>
        /// initialize partipant
        /// start sending heatbeat
        /// </summary>
        public virtual void init()
        {
            //NOP
        }

        /// <summary>
        /// connect to nats
        /// </summary>
        /// <param name="natsip"></param>
        /// <returns></returns>
        public bool connect(string natsip = null)
        {
            bool _success = false;
            try
            {
                //if nats url is not provided assume localhost port 4222
                _server = @"nats://localhost:4222";

                if (natsip != null)
                    _server = natsip;

                ConnectionFactory cf = new ConnectionFactory();
                _connection = cf.CreateConnection(_server);

                _success = true;
            }
            catch (NATSNoServersException nse)
            {
                _success = false;
            }
            catch(NATSConnectionException ce)
            {
                _success = false;
            }
            finally
            {
            }

            if(!_success)
            {
                //try again
                //IT LOOKS LIKE CONNECT WILL OCCASIONALLY FAIL IF CALLED
                //TOO QUICKLY
                Thread.Sleep(1000);
                _success = connect(natsip);
            }

            return _success;
        }
        #endregion

        #region advertisement
        /// <summary>
        /// publish our heartbeat
        /// </summary>
        public void advertisement()
        {
            _connection.Publish("heartbeat", Encoding.UTF8.GetBytes(id));
        }

        /// <summary>
        /// listen to all heartbeat advertisements
        /// </summary>
        /// <param name="action"></param>
        public void registerHeartbeatListener(EventHandler<MsgHandlerEventArgs> action)
        {
            _connection.SubscribeAsync("heartbeat", action);
        }
        #endregion

        #region register
        /// <summary>
        /// when you see this topic for me call the passed action with the message payload
        /// add id to subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="payload"></param>
        public void register(string subject, EventHandler<MsgHandlerEventArgs> action)
        {
            _connection.SubscribeAsync(id + "." + subject, action);
        }
        #endregion

        #region publish
        /// <summary>
        /// publish to this subject the passed object
        /// makes no assumption as to who is publishing
        /// participant must add id ( as in sendStatus ) if publishing as participant
        /// </summary>
        /// <param name="subject">subject</param>
        /// <param name="s">string payload</param>
        public void publish(string subject, string s)
        {
            _connection.Publish(subject, Encoding.UTF8.GetBytes(s));
        }

        /// <summary>
        /// publish to this subject the passed object
        /// makes no assumption as to who is publishing
        /// participant must add id ( as in sendStatus ) if publishing as participant
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="bytes"></param>
        public void publish(string subject, byte[] bytes)
        {
            _connection.Publish(subject, bytes);
        }
        #endregion

        #region request
        /// <summary>
        /// this is the nats library provided request
        /// will be used by a client to send a request to its associated participant 
        /// EXPECTED TO BE USED BY CLIENTS - CLIENT ID IS ADDED TO SUBJECT
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="payload"></param>
        /// <param name="action">callback</param>
        public void request(string subject, string payload, Action<string> callback)
        {
            Task.Factory.StartNew(() =>
            {
                var reply = _connection.Request(id + "." + subject, Encoding.UTF8.GetBytes(payload));
                callback(Encoding.UTF8.GetString(reply.Data, 0, reply.Data.Length));
            });
        }

        public void request(string subject, byte[] payload, Action<string> callback)
        {
            Task.Factory.StartNew(() =>
            {
                var reply = _connection.Request(id + "." + subject, payload);
                callback(Encoding.UTF8.GetString(reply.Data, 0, reply.Data.Length));
            });
        }

        /// <summary>
        /// this is our request that implements the ack/nak/response interaction
        /// in the ta enterprise communications protocol this is how get's are implemented
        /// adds id to subject and is expected to be used by Clients
        /// </summary>
        /// <param name="subject">subject is what we are requesting</param>
        /// <param name="data">data to send with request</param>
        /// <param name="completion">ack/response callback instance</param>
        public void protocolRequest(string  subject, byte[] data, IResponseCompletion completion = null)
        {
            Task.Factory.StartNew( ()=>
            {
                //create a reply to mailbox
                var replyTo = "TA_MBOX:" + Guid.NewGuid().ToString();

                if(completion != null)
                //subscribe to reply to mailbox as this is where
                //ack and reponse will be directed to
                _connection.SubscribeAsync(replyTo, (s, a) =>
                {
                    var dataAsString = Encoding.UTF8.GetString(a.Message.Data, 0, a.Message.Data.Length);

                    //call through ack when ack is received
                    if (dataAsString == "ACK")
                    {
                        completion.ack?.Invoke(); //2 means not implemented, need enum...
                    }
                    else if (dataAsString == "NAK")
                    {
                        completion.nak?.Invoke(2);
                    }
                    else
                    {
                        //call through response ( complete )
                        completion.response?.Invoke(a.Message.Data);

                        //unsubscribe from temporary mailbox
                        a.Message.ArrivalSubcription.Unsubscribe();
                    }
                });

                //publish to subject with reply to
                var message = new Msg(id + "." + subject, replyTo, data);
                _connection.Publish(message);
            });
        }

        /// <summary>
        /// this is our request that implements the ack/nak/response interaction
        /// adds id to subject and is expected to be used by Clients
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="replyTo"></param>
        /// <param name="data"></param>
        /// <param name=""></param>
        public void protocolRequest(string subject, string data, IResponseCompletion completion = null)
        {
            Task.Factory.StartNew(() =>
            {
                //create a reply to mailbox
                var replyTo = "TA_MBOX:" + Guid.NewGuid().ToString();

                if(completion != null)
                //subscribe to reply to mailbox as this is where
                //ack and reponse will be directed to
                _connection.SubscribeAsync(replyTo, (s, a) =>
                {
                    var dataAsString = Encoding.UTF8.GetString(a.Message.Data, 0, a.Message.Data.Length);

                    //call through ack when ack is received
                    if (dataAsString == "ACK")
                        completion.ack?.Invoke();
                    else if (dataAsString == "NAK")
                        if (completion.nak != null)
                            completion.nak?.Invoke(2); //2 means not implemented, need enum...
                        else
                        {
                            //call through response ( complete )
                            completion.response(a.Message.Data);
                            //unsubscribe from temporary mailbox
                            a.Message.ArrivalSubcription.Unsubscribe();
                        }
                });

                //publish to subject with reply to
                var message = new Msg(id + "." + subject, replyTo, Encoding.UTF8.GetBytes(data));
                _connection.Publish(message);
            });
        }

        /// <summary>
        /// original request
        /// wraps library request/reply mechanism
        /// adds id to subject and is expected to be used by Clients
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="payload"></param>
        /// <returns>reply as string</returns>
        public string request(string subject, string payload)
        {
            var reply = _connection.Request(id + "." + subject, Encoding.UTF8.GetBytes(payload));
            return Encoding.UTF8.GetString(reply.Data, 0, reply.Data.Length);
        }

        /// <summary>
        /// original request
        /// wraps library request/reply mechanism
        /// adds id to subject and is expected to be used by Clients
        /// BLOCKS!
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="payload">payload as byte[]</param>
        /// <returns>reply as bytes</returns>
        public byte[] request(string subject, byte[] payload)
        {
            var reply = _connection.Request(id + "." + subject, payload);
            return reply.Data;
        }
    
        /// <summary>
        /// send request and be called back for ack/nak/response
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="payload"></param>
        /// <param name="callbacks"></param>
        public void request(string subject, byte[] payload, IResponseCompletion callbacks)
        {
            protocolRequest(subject, payload, callbacks);
        }

        #endregion

        #region observer
        IList<IObserver>    _observers  = new List<IObserver>();
        IList<IObserverEx> _observersex = new List<IObserverEx>();

        public void addObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void removeObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void notify(string hint)
        {
            foreach (IObserver o in _observers)
                o.update(this, hint);
        }

        public void update(IObservable observable, object hint)
        {
            //expected to be implemented in observer
            throw new NotImplementedException();
        }

        public void addObserver(IObserverEx observer)
        {
            _observersex.Add(observer);
        }

        public void removeObserver(IObserverEx observer)
        {
            _observersex.Remove(observer);
        }

        public void notify(string hint, object data)
        {
            foreach (IObserverEx o in _observersex)
                o.update(this, hint, data);
        }

        public void update(IObservableEx observable, object hint, object data)
        {
            //expected to be implemented by derived class
            throw new NotImplementedException();
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return string.Format("{0, 20}{1, 20}{2,20}", name, type, subType);
        }
        #endregion
    }
    #endregion
}
