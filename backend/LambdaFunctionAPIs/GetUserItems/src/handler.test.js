import { jest } from '@jest/globals';
import { handler } from '../src/index.mjs';

// Mocking shared utility functions
jest.mock('../../shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      if (query.includes('SELECT * FROM users WHERE employee_id = $1')) {
        if (values[0] === '123') {
          return {
            rows: [
              {
                user_id: 273,
                user_name: 'testuser',
                employee_id: '123',
                current_coins: 0,
                total_coins: 0,
                items_owned: [100, 101],
                items_equipped: [100],
              },
            ],
          };
        }
        return { rows: [] };
      }

      if (query.includes('SELECT item_id, item_name FROM store WHERE item_id = ANY($1::int[])')) {
        const mockStoreItems = [
          { item_id: 100, item_name: 'Orange Hard Hat' },
          { item_id: 101, item_name: 'White Hard Hat' },
        ];
        return {
          rows: mockStoreItems.filter((item) => values[0].includes(item.item_id)),
        };
      }

      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('Get User Items Lambda Tests', () => {
  it('should return user details and items', async () => {
    const mockEvent = {
      body: JSON.stringify({ employee_id: '123' }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);

    const responseBody = JSON.parse(response.body);
    expect(responseBody).toMatchObject({
    user_id: expect.any(Number), // Expect a number
    user_name: expect.any(String), // Expect a string
    employee_id: expect.any(String), // Expect a string
    current_coins: expect.any(Number), // Expect a number
    total_coins: expect.any(Number), // Expect a number
    items_owned: expect.any(Object), // Expect an object or array
    items_equipped: expect.any(Object), // Expect an object or array
  });
  });

  it('should return 400 error if employee_id is missing', async () => {
    const mockEvent = { body: JSON.stringify({}) };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const error = JSON.parse(response.body);
    expect(error.error).toBe('Employee ID is missing or invalid.');
  });

  it('should return 400 error if user not found', async () => {
    const mockEvent = {
      body: JSON.stringify({ employee_id: 'non_existent' }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const error = JSON.parse(response.body);
    expect(error.error).toBe('No users found with the specified employee_id');
  });

  it('should return 400 error if body is missing', async () => {
    const response = await handler({});

    expect(response.statusCode).toBe(400);
    const error = JSON.parse(response.body);
    expect(error.error).toBe('Request body is missing.');
  });
});
