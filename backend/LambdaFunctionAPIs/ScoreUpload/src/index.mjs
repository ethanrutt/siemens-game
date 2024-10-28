import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Function to check if the user exists in the database
const checkUserExists = async (client, userId) => {
    const result = await client.query('SELECT * FROM users WHERE user_id = $1', [userId]);
    return result.rows.length > 0;
};

// Function to get the existing score
const getExistingScore = async (client, userId, gameId) => {
    const result = await client.query(
        'SELECT score FROM game_scores WHERE user_id = $1 AND game_id = $2',
        [userId, gameId]
    );
    return result.rows[0]?.score || null;
};

// Function to insert or update the score using UPSERT logic
const upsertGameScore = async (client, userId, gameId, score) => {
    try {
        const query = `
            INSERT INTO game_scores (user_id, game_id, score)
            VALUES ($1, $2, $3)
            ON CONFLICT (user_id, game_id) 
            DO UPDATE SET score = $3
            RETURNING *;
        `;
        const result = await client.query(query, [userId, gameId, score]);
        return result.rows[0];
    } catch (err) {
        console.error('Error upserting game score:', err);
        throw err;
    }
};

// Function to handle the score logic based on game_id conditions
const handleScoreInsert = async (client, userId, gameId, newScore) => {
    const existingScore = await getExistingScore(client, userId, gameId);

    if (gameId === 5 || gameId === 7) {
        // Insert if the new score is lower than the existing score
        if (existingScore !== null && newScore >= existingScore) {
            return { message: "No Score Uploaded, not the lowest score." };
        }
    } else if (gameId === 6) {
        // Add 1 to the existing score
        const updatedScore = (existingScore || 0) + 1;
        return await upsertGameScore(client, userId, gameId, updatedScore);
    }

    // Insert or update for other game_ids
    return await upsertGameScore(client, userId, gameId, newScore);
};

export const handler = async (event) => {
    let client;

    try {
        if (!event.body) {
            throw new Error('Request body is missing.');
        }

        const requestBody = JSON.parse(event.body);
        const { user_id, game_id, score } = requestBody;

        if (!user_id || !game_id || (score === undefined && game_id !== 6)) {
            throw new Error('User ID, game ID, and score must be provided.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Check if the user exists
        const userExists = await checkUserExists(client, user_id);
        if (!userExists) {
            throw new Error(`User with ID ${user_id} does not exist.`);
        }

        // Handle the score insertion logic
        const upsertedScore = await handleScoreInsert(client, user_id, game_id, score);

        return {
            statusCode: 200,
            body: JSON.stringify({
                message: 'Score operation completed successfully',
                score: upsertedScore,
            }),
        };
    } catch (err) {
        console.error('Error processing request:', err);
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
