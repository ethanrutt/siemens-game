/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */


/**
 * Retrieves the score and rank for a user in a specific game.
 *
 * @param {Object} client - The PostgreSQL client for database operations.
 * @param {number} userId - The ID of the user.
 * @param {number} gameId - The ID of the game.
 * @param {string} tableName - The name of the table to query (default: 'game_scores').
 * @returns {Promise<Object>} - An object containing the target score and rank of the user.
 * @throws {Error} - Throws an error if the query fails or no scores are found for the user.
 */
const getScoreAndRank = async (client, userId, gameId, tableName) => {
    try {
        let scoreQuery, rankQuery;
        
        if (gameId === 7 || gameId === 5) {
            // For game_id = 7, get the lowest score and rank based on ascending order
            scoreQuery = `SELECT MIN(score) AS target_score FROM ${tableName} WHERE user_id = $1 AND game_id = $2`;
            rankQuery = `SELECT COUNT(*) + 1 AS rank FROM ${tableName} WHERE game_id = $1 AND score < $2`;
        } else {
            // For other games, get the highest score and rank based on descending order
            scoreQuery = `SELECT MAX(score) AS target_score FROM ${tableName} WHERE user_id = $1 AND game_id = $2`;
            rankQuery = `SELECT COUNT(*) + 1 AS rank FROM ${tableName} WHERE game_id = $1 AND score > $2`;
        }

        // Get the target score (either highest or lowest depending on the game)
        const scoreResult = await client.query(scoreQuery, [userId, gameId]);
        const targetScore = scoreResult.rows[0]?.target_score;

        if (!targetScore) {
            throw new Error('No scores found for the given user and game.');
        }

        // Calculate the rank based on the target score
        const rankResult = await client.query(rankQuery, [gameId, targetScore]);
        const rank = rankResult.rows[0]?.rank;

        return { targetScore, rank };
    } catch (err) {
        console.error('Error fetching score and rank:', err);
        throw err;
    }
};

/**
 * Lambda function handler for retrieving a user's score and rank in a specific game.
 *
 * @param {Object} event - The Lambda event object containing the request data.
 * @param {string} tableName - The name of the database table to query (default: 'game_scores').
 * @returns {Promise<Object>} - Returns a response object containing the score and rank.
 * @throws {Error} - Throws an error if the request body is invalid or the database query fails.
 */
export const handler = async (event, tableName = 'game_scores') => {
    let client;

    try {
        if (!event.body) {
            throw new Error('Request body is missing.');
        }

        const requestBody = JSON.parse(event.body);
        const { user_id, game_id } = requestBody;

        if (!user_id || !game_id) {
            throw new Error('user_id and game_id are required.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        const { targetScore, rank } = await getScoreAndRank(client, user_id, game_id, tableName);

        return {
            statusCode: 200,
            body: JSON.stringify({
                score: targetScore,
                rank: rank,
            }),
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
