const assert = require('assert');
const app = require('../../src/app');

describe('\'instruments\' service', () => {
  it('registered the service', () => {
    const service = app.service('instruments');

    assert.ok(service, 'Registered the service');
  });
});
