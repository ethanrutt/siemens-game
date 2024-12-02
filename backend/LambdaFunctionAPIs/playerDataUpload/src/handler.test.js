import { jest } from '@jest/globals';
import { handler } from './index.mjs';
import { SecretsManagerClient, GetSecretValueCommand } from '@aws-sdk/client-secrets-manager';
import pkg from 'pg';

const { Client } = pkg;

// Mock PostgreSQL Client
jest.mock('pg', () => ({
    Client: jest.fn(() => ({
        connect: jest.fn(),
        query: jest.fn(),
        end: jest.fn(),
    })),
}));




describe('Lambda Tests', () => {
    let secretsManagerMock;
    let dbClientMock;

    beforeEach(() => {
        jest.clearAllMocks();

        // Create a real instance of SecretsManagerClient to mock
        secretsManagerMock = new SecretsManagerClient();
        jest.spyOn(secretsManagerMock, 'send').mockResolvedValue({
            SecretString: JSON.stringify({ username: 'mock_user', password: 'mock_password' }),
        });

        dbClientMock = new Client();
    });

   

    it('should return error when user is not found', async () => {

        const event = {
            body: JSON.stringify({
                user_id: 999, // Non-existent user_id
                current_coins: 100,
                items_owned: [200, 300],
                items_equipped: [200],
                cards_owned: [101, 102],
                achievements_complete: [1, 2, 3],
                achievements: { tasksCompleted: 5, quests: 2 },
                has_finished_cutscene: true,
                location_x: 5.2,
                location_y: 7.3,
                current_scene: 'MainMenu',
                interactions: { actionA: 3, actionB: 5 },
            }),
        };

        const result = await handler(event);

        expect(result.statusCode).toBe(400);
        expect(JSON.parse(result.body).error).toBe('No user found with user_id: 999');
    });

    it('should return error for missing request body', async () => {
        const event = {};
        const result = await handler(event);

        expect(result.statusCode).toBe(400);
        expect(JSON.parse(result.body).error).toBe('Request body is missing.');
    });

    it('should return error for missing user_id', async () => {
        const event = {
            body: JSON.stringify({
                current_coins: 100,
                items_owned: [200, 300],
                items_equipped: [200],
                cards_owned: [101, 102],
                achievements_complete: [1, 2, 3],
                achievements: { tasksCompleted: 5, quests: 2 },
                has_finished_cutscene: true,
                location_x: 5.2,
                location_y: 7.3,
                current_scene: 'MainMenu',
                interactions: { actionA: 3, actionB: 5 },
            }),
        };

        const result = await handler(event);

        expect(result.statusCode).toBe(400);
        expect(JSON.parse(result.body).error).toBe('Missing required parameter: user_id.');
    });
});
