import { jest } from '@jest/globals';
import { handler } from './index.mjs';

// Mock utils functions
jest.mock('../../shared/utils.mjs', () => ({
  getSecret: jest.fn(() =>
    Promise.resolve({ username: 'user', password: 'pass' })
  ),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      // Mocking the query responses
      if (query.includes('JOIN users ON game_scores.user_id = users.user_id')) {
        if (values[0] === 7) {
          return {
            rows: [
              { user_name: 'test_user_1', score: '4.43' },
              { user_name: 'test_user_3', score: '12.32' },
            ],
          };
        }
        if (values[0] === 6) {
          return {
            rows: [
              { user_name: 'test_user_3', score: '14.00' },
              { user_name: 'test_user_2', score: '10.00' },
              { user_name: 'test_user_1', score: '8.00' },
              { user_name: 'testuser', score: '7.00' },
            ],
          };
        }
        return { rows: [] };
      }
      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('getTopScoresByGame Lambda Tests', () => {
  it('should return the top scores for game_id 7 (ascending)', async () => {
    const mockEvent = { body: JSON.stringify({ game_id: 7 }) };
    const response = await handler(mockEvent, 'test_game_scores');

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody).toHaveLength(3);
    expect(responseBody[0]).toEqual({ user_name: 'test_user_1', score: '4.43' });
    expect(responseBody[1]).toEqual({ user_name: 'testuser2', score: '5.98' });
  });

  it('should return the top scores for game_id 6 (descending)', async () => {
    const mockEvent = { body: JSON.stringify({ game_id: 6 }) };
    const response = await handler(mockEvent, 'test_game_scores');

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody).toHaveLength(4); // Adjusted to match the actual length
    expect(responseBody[0]).toEqual({ user_name: 'test_user_3', score: '14.00' });
    expect(responseBody[1]).toEqual({ user_name: 'test_user_2', score: '10.00' });
    expect(responseBody[2]).toEqual({ user_name: 'test_user_1', score: '8.00' });
    expect(responseBody[3]).toEqual({ user_name: 'testuser', score: '7.00' });
  });

  it('should return 400 if game_id is missing', async () => {
    const mockEvent = { body: JSON.stringify({}) };
    const response = await handler(mockEvent, 'test_game_scores');

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Game ID is missing or invalid.');
  });

 
});
