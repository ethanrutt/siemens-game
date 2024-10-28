import { jest } from '@jest/globals';
import { handler } from './index.mjs';
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Mocking utilities
jest.mock('./shared/utils.mjs', () => ({
  getSecret: jest.fn(() =>
    Promise.resolve({ username: 'user', password: 'pass' })
  ),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      // Mock database behavior
      if (query.includes('SELECT current_coins, items_owned FROM users')) {
        if (values[0] === 'emp_001') {
          return { rows: [{ current_coins: 500, items_owned: [100] }] }; // User has 500 coins and owns item 100
        }
        if (values[0] === 'emp_002') {
          return { rows: [{ current_coins: 50, items_owned: [] }] }; // User with insufficient coins
        }
        return { rows: [] }; // No user found
      }

      if (query.includes('SELECT * FROM store WHERE item_id')) {
        if (values[0] === 200) {
          return { rows: [{ item_id: 200, item_price: 300 }] }; // Item exists and costs 300
        }
        return { rows: [] }; // Item not found
      }

      if (query.includes('UPDATE users SET current_coins')) {
        return { rowCount: 1 }; // Simulate successful update
      }

      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('Purchase Item Handler Tests', () => {
 

  it('should fail if the user already owns the item', async () => {
    const mockEvent = {
      body: JSON.stringify({
        employee_id: 'emp_001',
        item_id: 100, // User already owns item 100
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User already owns this item.');
  });

  it('should fail if the user does not have enough coins', async () => {
    const mockEvent = {
      body: JSON.stringify({
        employee_id: 'emp_002',
        item_id: 200, // Item costs 300, but the user only has 50 coins
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Not enough coins to purchase this item.');
  });

  it('should fail if the item is not found', async () => {
    const mockEvent = {
      body: JSON.stringify({
        employee_id: 'emp_001',
        item_id: 999, // Non-existent item
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Item not found in the store.');
  });

  it('should return 400 if employee_id or item_id is missing', async () => {
    const mockEvent = {
      body: JSON.stringify({}),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Employee ID and item ID must be provided.');
  });

  it('should return 400 if the user is not found', async () => {
    const mockEvent = {
      body: JSON.stringify({
        employee_id: 'non_existent_emp',
        item_id: 200,
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User not found with the specified employee_id.');
  });

  it('should return 400 if request body is missing', async () => {
    const mockEvent = {};

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Request body is missing.');
  });
});
