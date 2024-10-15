import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Function to get top 10 scores for a given game_id
const getTopScoresByGame = async (client, gameId) => {
    try {
        const query = `
            SELECT users.user_name, game_scores.score
            FROM game_scores
            JOIN users ON game_scores.user_id = users.user_id
            WHERE game_scores.game_id = $1
            ORDER BY game_scores.score DESC
            LIMIT 10
        `;

        const result = await client.query(query, [gameId]);
        return result.rows;
    } catch (err) {
        console.error('Error fetching top scores by game:', err);
        throw err;
    }
};

export const handler = async (event) => {
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
        const topScores = await getTopScoresByGame(client, game_id);

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
