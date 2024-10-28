import { jest } from '@jest/globals';
import { handler } from './index.mjs';
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Mock utilities
jest.mock('./shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      // Mock database behavior
      if (query.includes('SELECT * FROM users WHERE user_id')) {
        return values[0] === 1 ? { rows: [{}] } : { rows: [] }; // User exists if user_id is 1
      }

      if (query.includes('SELECT score FROM game_scores')) {
        return { rows: [{ score: 10 }] }; // Existing score of 10 for the user
      }

      if (query.includes('INSERT INTO game_scores')) {
        return { rows: [{ user_id: values[0], game_id: values[1], score: values[2] }] }; // Simulate upsert
      }

      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

describe('Score Handler Tests', () => {
  it('should successfully insert a new score if lower for game_id 7', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 1, game_id: 7, score: 5 }), // New lower score
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.message).toBe('Score operation completed successfully');
    expect(responseBody.score.score).toBe(5);
  });

  it('should not insert a new score if higher for game_id 7', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 1, game_id: 7, score: 15 }), // New higher score
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.score.message).toBe('No Score Uploaded, not the lowest score.');
  });

  it('should increment the score by 1 for game_id 6', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 1, game_id: 6 }), // Game_id 6 increments score by 1
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.score.score).toBe(11); // Original score was 10
  });

  it('should return 400 if user_id is not found', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 999, game_id: 7, score: 5 }), // Non-existent user_id
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User with ID 999 does not exist.');
  });

  it('should return 400 if request body is missing', async () => {
    const mockEvent = {};

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Request body is missing.');
  });

  it('should return 400 if required parameters are missing', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 1 }), // Missing game_id and score
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User ID, game ID, and score must be provided.');
  });

  it('should successfully upsert a new score for other game_ids', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 273, game_id: 8, score: 20 }), // New score for other game_id
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.score.score).toBe(20);
  });
});
