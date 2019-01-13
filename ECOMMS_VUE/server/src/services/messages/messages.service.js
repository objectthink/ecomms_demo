// Initializes the `messages` service on path `/messages`
const createService = require('feathers-memory');
const hooks = require('./messages.hooks');

var heartbeats = [];

module.exports = function (app) {

  const paginate = app.get('paginate');

  const options = {
    paginate
  };

  // Initialize our service with any options it requires
  app.use('/messages', createService(options));

  // Get our initialized service so that we can register hooks
  const service = app.service('messages');

  service.hooks(hooks);

  console.log('starting up');

  //NATS INTERFACE
  var NATS = require('nats');

  //get the nats url from the app default.json
  var nats = NATS.connect(app.get('nats_url'));
  console.log(app.get('nats_url'));

  // Simple Subscriber
  nats.subscribe('heartbeat', function(msg) {

    if( !heartbeats.includes(msg))
    {
      //write to the console
      console.log('Received a message: ' + msg);

      //push heartbeat for lookup
      heartbeats.push(msg);

      //create a message with the heartbeat
      service.create({id: 1, text: msg});

      //listen for syslog status messages from
      //this participant
      nats.subscribe(msg + '.status.syslog', function(msg){
        service.create({text: msg});
      })
    }
  });

  // Simple Publisher
  nats.publish('foo', 'hello cruel world');
};
