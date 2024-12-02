/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

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


