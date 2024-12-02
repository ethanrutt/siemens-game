/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

/**
 * Updates the player data in the database.
 *
 * @param {Object} client - PostgreSQL client for database operations.
 * @param {number} user_id - The ID of the user.
 * @param {number} current_coins - The current coin count for the user.
 * @param {Array<number>} items_owned - Array of item IDs the user owns.
 * @param {Array<number>} items_equipped - Array of item IDs the user has equipped.
 * @param {Array<number>} cards_owned - Array of card IDs the user owns.
 * @param {Array<number>} achievements_complete - Array of completed achievement IDs.
 * @param {Object} achievements - Dictionary of achievements and their progress.
 * @param {boolean} has_finished_cutscene - Whether the user has finished the cutscene.
 * @param {number} location_x - The user's X-coordinate in the game world.
 * @param {number} location_y - The user's Y-coordinate in the game world.
 * @param {string} current_scene - The current scene the user is in.
 * @param {Object} interactions - Dictionary of user interactions and their counts.
 * @returns {Promise<Object>} - Resolves with the updated user data.
 * @throws {Error} - Throws an error if the user is not found or the query fails.
 */
const updatePlayerData = async (client, user_id, current_coins, items_owned, items_equipped, cards_owned, achievements_complete, achievements, has_finished_cutscene, location_x, location_y, current_scene, interactions) => {
    const query = `
        UPDATE users
        SET 
            current_coins = $1,
            items_owned = $2,
            items_equipped = $3,
            cards_owned = $4,
            achievements_complete = $5,
            achievements = $6,
            has_finished_cutscene = $7,
            location_x = $8,
            location_y = $9,
            current_scene = $10,
            interactions = $11
        WHERE user_id = $12
        RETURNING *;
    `;
    const values = [
        current_coins, 
        items_owned, 
        items_equipped, 
        cards_owned, 
        achievements_complete, 
        achievements, 
        has_finished_cutscene, 
        location_x, 
        location_y, 
        current_scene, 
        interactions,
        user_id
    ];

    const result = await client.query(query, values);
    if (result.rows.length === 0) {
        throw new Error(`No user found with user_id: ${user_id}`);
    }

    console.log(`Player data for user_id '${user_id}' updated successfully.`);
    return result.rows[0];
};

const errorCheck = (event) => {
    if (!event.body) {
        throw new Error('Request body is missing.');
    }

    const requestBody = JSON.parse(event.body);
    const { 
        user_id, 
        current_coins, 
        items_owned, 
        items_equipped, 
        cards_owned, 
        achievements_complete, 
        achievements, 
        has_finished_cutscene, 
        location_x, 
        location_y, 
        current_scene, 
        interactions 
    } = requestBody;

    if (!user_id) {
        throw new Error('Missing required parameter: user_id.');
    }

    return { 
        user_id, 
        current_coins, 
        items_owned, 
        items_equipped, 
        cards_owned, 
        achievements_complete, 
        achievements, 
        has_finished_cutscene, 
        location_x, 
        location_y, 
        current_scene, 
        interactions 
    };
};


/**
 * Validates and extracts the request body parameters.
 *
 * @param {Object} event - The Lambda event object containing the request data.
 * @returns {Object} - Extracted parameters from the request body.
 * @throws {Error} - Throws an error if the request body or required parameters are missing.
 */
export const handler = async (event) => {
    let client;

    try {
        // Extract parameters from the request body
        const { 
            user_id, 
            current_coins, 
            items_owned, 
            items_equipped, 
            cards_owned, 
            achievements_complete, 
            achievements, 
            has_finished_cutscene, 
            location_x, 
            location_y, 
            current_scene, 
            interactions 
        } = errorCheck(event);

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Update player data in the database
        const updatedUser = await updatePlayerData(
            client, 
            user_id, 
            current_coins, 
            items_owned, 
            items_equipped, 
            cards_owned, 
            achievements_complete, 
            achievements, 
            has_finished_cutscene, 
            location_x, 
            location_y, 
            current_scene, 
            interactions
        );

        // Successful response
        return {
            statusCode: 200,
            body: JSON.stringify({
                message: 'Player data updated successfully',
                user: updatedUser,
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


