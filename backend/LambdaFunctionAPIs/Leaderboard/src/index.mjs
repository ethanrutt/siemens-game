/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

// Function to get top scores for a given game_id
/**
 * Retrieves the top scores for a given game ID from the specified table.
 *
 * @param {Object} client - The PostgreSQL client for database operations.
 * @param {number} gameId - The ID of the game for which to fetch top scores.
 * @param {string} [tableName='game_scores'] - The name of the database table containing game scores.
 * @returns {Promise<Array>} - A promise that resolves to an array of objects containing user names and scores.
 * @throws {Error} - Throws an error if the query fails.
 */
const getTopScoresByGame = async (client, gameId, tableName = 'game_scores') => {
    try {
        const sortOrder = (gameId === 7 || gameId === 5) ? 'ASC' : 'DESC'; // Sort ascending for game_id 7 or 5

        const query = `
            SELECT users.user_name, ${tableName}.score
            FROM ${tableName}
            JOIN users ON ${tableName}.user_id = users.user_id
            WHERE ${tableName}.game_id = $1
            ORDER BY ${tableName}.score ${sortOrder}
            LIMIT 10
        `;

        const result = await client.query(query, [gameId]);
        return result.rows;
    } catch (err) {
        console.error('Error fetching top scores by game:', err);
        throw err;
    }
};

/**
 * Lambda function handler to retrieve the top scores for a specific game ID.
 *
 * @param {Object} event - The Lambda event object containing the request data.
 * @param {string} [tableName='game_scores'] - The name of the database table containing game scores.
 * @returns {Promise<Object>} - Returns a response object with HTTP status code and the top scores.
 * @throws {Error} - Throws an error if the request body is invalid or the database query fails.
 */
export const handler = async (event, tableName = 'game_scores') => {
    let client;

    try {
        if (!event.body) {
            throw new Error('Request body is missing.');
        }

        const requestBody = JSON.parse(event.body);
        const { game_id } = requestBody;

        if (!game_id) {
            throw new Error('Game ID is missing or invalid.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Fetch the top 10 scores for the provided game_id
        const topScores = await getTopScoresByGame(client, game_id, tableName);

        return {
            statusCode: 200,
            body: JSON.stringify(topScores),
        };
    } catch (err) {
        console.error('Error querying the database:', err);
        return {
            statusCode: 400,
            body: JSON.stringify({ error: err.message }),
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
