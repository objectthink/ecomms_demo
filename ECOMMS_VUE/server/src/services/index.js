const messages = require('./messages/messages.service.js');
const instruments = require('./instruments/instruments.service.js');
const services = require('./services/services.service.js');
// eslint-disable-next-line no-unused-vars
module.exports = function (app) {
  app.configure(messages);
  app.configure(instruments);
  app.configure(services);
};
