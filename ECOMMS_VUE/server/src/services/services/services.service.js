// Initializes the `services` service on path `/services`
const createService = require('feathers-memory');
const hooks = require('./services.hooks');

var _services = {};

const Service = require('feathers-memory').Service;

class ServicesService extends Service {
  constructor(options, nats)
  {
    super(options);
    this.nats = nats;
  }

  create(data, params) {
    data.created_at = new Date();

    return super.create(data, params);
  }
}

module.exports = function (app) {

  const paginate = app.get('paginate');

  const options = {
    paginate
  };

  //NATS INTERFACE
  var NATS = require('nats');

  //get the nats url from the app default.json
  var nats = NATS.connect(app.get('nats_url'));
  console.log(app.get('nats_url'));

  // Initialize our service with any options it requires
  //app.use('/services', createService(options));
  app.use('/services', new ServicesService(options, nats));

  // Get our initialized service so that we can register hooks
  const service = app.service('services');

  service.hooks(hooks);

  console.log('starting up');

  // Simple Subscriber
  nats.subscribe('heartbeat', function(heartbeat) {
    //console.log(heartbeat);
    if(!(heartbeat in _services))
    {
      //get the participants role
      nats.request(heartbeat + '.get', 'role', {'max':1},
      function(role)
      {
        //test
        if(role == 'Service')
        {
          _services[heartbeat] =
            {
              id:heartbeat,
              lastAdvertisement: new Date(),
              name: '---',
              role: 'Service',
              type: '---',
              subtype: '---',
              info: 'information about this ecomms service'
            }

          service.create(_services[heartbeat]);

          nats.request(heartbeat + '.get', 'name', {'max':1},
          function(name)
          {
            console.log('name: ' + name);

            _services[heartbeat].name = name;

            service.update(heartbeat, _services[heartbeat]);
          });

          nats.request(heartbeat + '.get', 'type', {'max':1},
          function(type)
          {
            console.log('type: ' + type);

            _services[heartbeat].type = type;

            service.update(heartbeat, _services[heartbeat]);
          });
        }
      });

    }
    else
    {
      //update lastAdvertisement
      _services[heartbeat].lastAdvertisement = new Date();
      console.log(_services[heartbeat]);
    }
  });

  //remove stale heartbeats
  function staleAdvertisementCheck() {

    setTimeout(function()
    {
      var now = new Date();

      for (var hb in _services)
      {

        //console.log(mac);
        //console.log(now - instrumentAdvertisementDict[mac]);

        if( (now - _services[hb].lastAdvertisement) > 4000)
        {
          //remove stale advertisement
          console.log('removing stale heartbeat:' + hb + 'with last: ' + _services[hb].lastAdvertisement);

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
