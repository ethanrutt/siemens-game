import { jest } from '@jest/globals';
import { handler } from '../src/index.mjs';

// Mock the utils.mjs functions
jest.mock('../../shared/utils.mjs', () => {
  return {
    __esModule: true,
    getSecret: jest.fn(() =>
      Promise.resolve({
        username: 'test_user',
        password: 'test_pass',
      })
    ),
    createDbClient: jest.fn(() => {
      const mockQuery = jest.fn(); // Use Jest mock function for query
      return {
        connect: jest.fn(),
        query: mockQuery,
        end: jest.fn(),
        mockQuery, // Expose the mock function to configure in tests
      };
    }),
  };
});

describe('Get All Items Lambda Tests', () => {
  it('should return the correct number of items from the store', async () => {
    const response = await handler({});
    expect(response.statusCode).toBe(200);

    const items = JSON.parse(response.body);
    expect(items).toHaveLength(35);

    expect(items[0]).toEqual({
      item_id: 100,
      item_name: 'Orange Hard Hat',
      item_type: 'hat',
      item_price: 100,
    });

    expect(items[1].item_name).toBe('White Hard Hat');
    expect(items[2].item_name).toBe('Yellow Hard Hat');
  });
});
