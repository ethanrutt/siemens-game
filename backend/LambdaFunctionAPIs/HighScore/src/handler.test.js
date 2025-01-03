import { jest } from '@jest/globals';
import { handler } from '../src/index.mjs';

// Mock the utils.mjs functions
jest.mock('../../shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      if (query.includes('MIN(score)')) {
        return { rows: [{ target_score: '4.43' }] }; // Return as string to match DB output
      }
      if (query.includes('MAX(score)')) {
        return { rows: [{ target_score: '14.00' }] }; // Return as string
      }
      if (query.includes('COUNT(*) + 1 AS rank')) {
        // Adjust rank based on test case input for lowest score of 4.43 in game_id 7
        if (values[1] === '4.43' && values[0] === 7) {
          return { rows: [{ rank: 1 }] }; // Lowest score should rank 1
        }
        return { rows: [{ rank: 3 }] }; // Default for other scenarios
      }
      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('getScoreAndRank Lambda Tests', () => {
  it('should return the lowest score and rank for game_id 7', async () => {
    const mockEvent = { body: JSON.stringify({ user_id: 274, game_id: 7 }) };

    const response = await handler(mockEvent, "test_game_scores");
    expect(response.statusCode).toBe(200);

    const responseBody = JSON.parse(response.body);
    expect(parseFloat(responseBody.score)).toBe(4.43); // Convert to float for comparison
    expect(responseBody.rank).toBe("1"); // Adjusted expectation
  });

  it('should return the highest score and rank for game_id 6', async () => {
    const mockEvent = { body: JSON.stringify({ user_id: 276, game_id: 6 }) };

    const response = await handler(mockEvent, "test_game_scores");
    expect(response.statusCode).toBe(200);

    const responseBody = JSON.parse(response.body);
    expect(parseFloat(responseBody.score)).toBe(14.00); // Convert to float for comparison
    expect(responseBody.rank).toBe("1");
  });

  it('should return 400 if request payload is invalid', async () => {
    const mockEvent = { body: JSON.stringify({}) }; // Missing user_id and game_id

    const response = await handler(mockEvent, "test_game_scores");
    expect(response.statusCode).toBe(400);

    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('user_id and game_id are required.');
  });
});
