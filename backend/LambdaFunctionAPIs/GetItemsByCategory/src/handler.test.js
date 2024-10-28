import { jest } from '@jest/globals';
import { handler } from '../src/index.mjs';

// Mock the utils.mjs functions
jest.mock('../../shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      if (query.includes('SELECT * FROM store WHERE item_type = $1')) {
        if (values[0] === 'hat') {
          return {
            rows: [
              { item_id: 100, item_name: 'Orange Hard Hat', item_type: 'hat', item_price: 100 },
              { item_id: 101, item_name: 'White Hard Hat', item_type: 'hat', item_price: 100 },
            ],
          };
        }
        return { rows: [] }; // No items for other categories
      }
      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('Get Items by Category Lambda Tests', () => {
  it('should return items for the specified category', async () => {
    const mockEvent = {
      body: JSON.stringify({ category: 'hat' }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const items = JSON.parse(response.body);

    expect(items).toHaveLength(9);
    expect(items[0]).toEqual({
      item_id: 100,
      item_name: 'Orange Hard Hat',
      item_type: 'hat',
      item_price: 100,
    });
  });

  it('should return 400 error if category is missing', async () => {
    const mockEvent = {
      body: JSON.stringify({}),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const error = JSON.parse(response.body);
    expect(error.error).toBe('Category parameter is missing or invalid.');
  });

  it('should return 400 error if body is missing', async () => {
    const response = await handler({});

    expect(response.statusCode).toBe(400);
    const error = JSON.parse(response.body);
    expect(error.error).toBe('Request body is missing.');
  });


});
