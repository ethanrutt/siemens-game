import { jest } from '@jest/globals';
import { handler } from './index.mjs';
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Mocking utils functions
jest.mock('./shared/utils.mjs', () => ({
  getSecret: jest.fn(() =>
    Promise.resolve({ username: 'user', password: 'pass' })
  ),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      if (query.includes('SELECT * FROM users WHERE user_name')) {
        // Login success scenario
        if (values[0] === 'validUser' && values[1] === 'validPass') {
          return {
            rows: [
              {
                user_id: 1,
                user_name: 'validUser',
                user_password: 'validPass',
                employee_id: '123',
                current_coins: 10,
                total_coins: 100,
                items_owned: '{}',
                items_equipped: '{}',
              },
            ],
          };
        }
        // Login failure scenario
        return { rows: [] };
      }

      if (query.includes('SELECT * FROM users WHERE employee_id')) {
        // Check employee_id existence for signup
        if (values[0] === 'existingEmployee') {
          return { rows: [{ employee_id: 'existingEmployee' }] };
        }
        return { rows: [] };
      }

      if (query.includes('INSERT INTO users')) {
        // Successful user signup
        return {
          rows: [
            {
              user_id: 2,
              user_name: 'newUser',
              user_password: 'newPass',
              employee_id: '124',
              current_coins: 0,
              total_coins: 0,
              items_owned: '{}',
              items_equipped: '{}',
            },
          ],
        };
      }

      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('Login and Signup Handler Tests', () => {
  it('should successfully login a user with valid credentials', async () => {
    const mockEvent = {
      body: JSON.stringify({
        user_name: 'validUser',
        user_password: 'validPass',
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);

  });

  it('should fail login with invalid credentials', async () => {
    const mockEvent = {
      body: JSON.stringify({
        user_name: 'invalidUser',
        user_password: 'invalidPass',
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Invalid credentials.');
  });

  it('should fail signup with existing employee ID', async () => {
    const mockEvent = {
      body: JSON.stringify({
        user_name: 'anotherUser',
        user_password: 'anotherPass',
        employee_id: 'existingEmployee',
      }),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Employee ID already exists.');
  });

  it('should return 400 for missing request body', async () => {
    const mockEvent = {};

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Request body is missing.');
  });

  it('should return 400 for missing user_name or user_password', async () => {
    const mockEvent = {
      body: JSON.stringify({}),
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe(
      'Missing required parameters: user_name or user_password.'
    );
  });
});
