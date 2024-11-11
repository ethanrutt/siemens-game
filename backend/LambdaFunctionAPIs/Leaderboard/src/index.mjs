import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Function to get top scores for a given game_id
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
