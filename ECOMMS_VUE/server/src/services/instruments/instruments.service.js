// Initializes the `instruments` service on path `/instruments`
const createService = require('feathers-memory');
const hooks = require('./instruments.hooks');

///////////////////////////////////////////////////////////////////////////////
//PROTOCOL BUFFERS TEST
var pbjs = require('pbjs');
var fs = require('fs');
var protobuf = require("protobufjs");

var schema = pbjs.parseSchema([
  'message Demo {',
  '  optional int32 x = 1;',
  '  optional float y = 2;',
  '}',
].join('\n')).compile();

var buffer = schema.encodeDemo({x: 1, y: 2});
console.log(buffer);

var message = schema.decodeDemo(buffer);
console.log(message);

var signals_pb = require('./mercuryrealtimesignals_pb');
var signalNames_pb = require('./mercuryrealtimesignalnames_pb');

///////////////////////////////////////////////////////////////////////////////
var _instruments = {};

const Service = require('feathers-memory').Service;

class InstrumentsService extends Service {
  constructor(options, nats)
  {
    super(options);

    this.nats = nats;
  }

  create(data, params) {
    data.created_at = new Date();

    return super.create(data, params);
  }

  update(id,data,params){
    if(params)
    {
      //console.log(params);
      if(params.query)
      {
        console.log(params.query);
        switch(params.query.action){
          case 'action':
            this.nats.request(id + '.action', params.query.data, {'max':1},
            function(status)
            {
              console.log(status);
            });
            break;
          case 'set':
            break;
        }
      }
    }

    return super.update(id, data, params);
  }
}

module.exports = function (app) {

  const paginate = app.get('paginate');

  const options = {
    paginate
  };

  ///////////////////////////////////////////////////////////////////////////////
  //NATS INTERFACE
  var NATS = require('nats');

  //get the nats url from the app default.json
  var nats = NATS.connect(app.get('nats_url'));
  console.log(app.get('nats_url'));
  ///////////////////////////////////////////////////////////////////////////////

  // Initialize our service with any options it requires
  //app.use('/instruments', createService(options));
  app.use('/instruments', new InstrumentsService(options, nats));

  // Get our initialized service so that we can register hooks
  const service = app.service('instruments');

  service.hooks(hooks);

  console.log('starting up');

  // Simple Subscriber
  nats.subscribe('heartbeat', function(heartbeat) {

    if(!(heartbeat in _instruments))
    {
      //create entry in our list to check against
      //could just query the service ( me )
      _instruments[heartbeat] =
        {
          id:heartbeat,
          lastAdvertisement: new Date(),
          name: '---',
          role: '',
          type: '---',
          subtype: '---',
          location: '---',
          currentTemperature: 0,
          signals:[],
          signalNames:[],
          status: ''
        }

      //add object here to avoid requesting role for instruments we have

      //get the participants role
      nats.request(heartbeat + '.get', 'role', {'max':1},
      function(role)
      {
        //test
        //if(role == 'Service')
        //{
        //  nats.request(heartbeat + '.get', 'name', {'max':1},
        //  function(name)
        //  {
        //    console.log('name: ' + name);
        //  });
        //}

        //if its an instrument get its name and location
        //and create an entry in the instruments dictionary
        if(role == 'Instrument')
        {
          _instruments[heartbeat] =
            {
              id:heartbeat,
              lastAdvertisement: new Date(),
              name: '---',
              role: 'Instrument',
              type: '---',
              subtype: '---',
              location: '---',
              currentTemperature: 0,
              signals:[],
              signalNames:[],
              status:''
            }

          service.create(_instruments[heartbeat]);

          nats.request(heartbeat + '.get', 'name', {'max':1},
          function(name)
          {
            console.log('name: ' + name);

            _instruments[heartbeat].name = name;

            service.update(heartbeat, _instruments[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'type', {'max':1},
          function(type)
          {
            console.log('type: ' + type);

            _instruments[heartbeat].type = type;

            service.update(heartbeat, _instruments[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'subtype', {'max':1},
          function(subtype)
          {
            console.log('subtype: ' + subtype);

            _instruments[heartbeat].subtype = subtype;

            service.update(heartbeat, _instruments[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'location', {'max':1},
          function(location)
          {
            console.log('location: ' + location);

            _instruments[heartbeat].location = location;

            service.update(heartbeat, _instruments[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'realtimesignalnames', {'max':1},
          function(bytes)
          {
            var names = signalNames_pb.MercuryRealTimeSignalNamesPB.deserializeBinary(bytes);

            _instruments[heartbeat].signalNames = names.getNamesList();
          });

          nats.subscribe(heartbeat + '.status.realtimesignalsstatus',(data)=> {
            console.log(heartbeat + ':realtimesignalssttatus:');

            try
            {
              //setTimeout(()=>{
              //}, 2000);

              var signals = signals_pb.MercuryRealTimeSignalsPB.deserializeBinary(data);

              _instruments[heartbeat].currentTemperature = signals.getDataList()[8];

              _instruments[heartbeat].signals = signals.getDataList();

              service.update(heartbeat, _instruments[heartbeat]);

              console.log(signals.getDataList()[8]);
            }
            catch(err)
            {
              console.log(err);
            }
          });

          nats.subscribe(heartbeat + '.status.runstate',(s)=> {
            console.log(heartbeat + ':runstate:' + s);

            _instruments[heartbeat].status = s;

            service.update(heartbeat, _instruments[heartbeat]);
          });

        }
      });

    }
    else
    {
      //update lastAdvertisement
      _instruments[heartbeat].lastAdvertisement = new Date();

      console.log(_instruments[heartbeat].role);
      //console.log(_instruments[heartbeat].signalNames);

      if(_instruments[heartbeat].role == 'Instrument')
      {
        if(_instruments[heartbeat].signalNames.length == 0)
        {

          //did we get the signal names?
          //will do this on the shim but for now just ask it again
          nats.request(heartbeat + '.get', 'realtimesignalnames', {'max':1},
          function(bytes)
          {
            var names = signalNames_pb.MercuryRealTimeSignalNamesPB.deserializeBinary(bytes);

            //console.log(names.getNamesList().length);

            if(names.getNamesList().length != 0)
              _instruments[heartbeat].signalNames = names.getNamesList();
            //else
            //  _instruments[heartbeat].signalNames = [];

          });
        }
      }
    }
  });

  //remove stale heartbeats
  function staleAdvertisementCheck() {

    setTimeout(function()
    {
      var now = new Date();

      for (var hb in _instruments)
      {

        //console.log(mac);
        //console.log(now - instrumentAdvertisementDict[mac]);

        if( (now - _instruments[hb].lastAdvertisement) > 4000)
        {
          //remove stale advertisement
          console.log('removing stale heartbeat:' + hb + 'with last: ' + _instruments[hb].lastAdvertisement);

          service.remove(hb);

          delete _instruments[hb];

          //delete instrumentAdvertisementDict[mac];
          //delete instrumentInfoDict[mac];

          //for(var index in instrumentSubscriptions[mac].subscriptions)
          //{
          //  nats.unsubscribe(instrumentSubscriptions[mac].subscriptions[index])
          //}

          //instrumentSubscriptions[mac].subscriptions.forEach(function(sid)
          //{
          //  nats.unsubscribe(sid)
          //});

          //delete instrumentSubscriptions[mac];

          //itemService.remove(mac);
        }
      }
      staleAdvertisementCheck();
    },
    1000);
  }

  //////////////
  staleAdvertisementCheck();
  // Simple Publisher
  //nats.publish('foo', 'hello cruel world');
};
