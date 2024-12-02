/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

/**
 * Fetches user information from the `users` table based on the provided `employee_id`.
 *
 * @param {Object} client - The PostgreSQL client used to interact with the database.
 * @param {string} employeeId - The `employee_id` to query.
 * @returns {Promise<Object>} - Returns the user object if found.
 * @throws {Error} - Throws an error if the query fails or no user is found.
 */
const getUserByEmployeeId = async (client, employeeId) => {
    try {
        const result = await client.query(
            'SELECT * FROM users WHERE employee_id = $1',
            [employeeId]
        );
        console.log(`Query for employee_id '${employeeId}' finished.`);
        if (result.rows.length > 0) {
            return result.rows[0];
        } else {
            throw new Error('No users found with the specified employee_id');
        }
    } catch (err) {
        console.error('Error fetching user by employee_id:', err);
        throw err;
    }
};

/**
 * Fetches item names from the `store` table based on an array of item IDs.
 *
 * @param {Object} client - The PostgreSQL client used to interact with the database.
 * @param {number[]} itemIds - An array of item IDs to query.
 * @returns {Promise<Object>} - Returns a mapping of item IDs to item names.
 * @throws {Error} - Throws an error if the query fails.
 */
const getItemNamesByIds = async (client, itemIds) => {
    try {
        const result = await client.query(
            'SELECT item_id, item_name FROM store WHERE item_id = ANY($1::int[])',
            [itemIds]
        );
        const itemMap = {};
        result.rows.forEach(item => {
            itemMap[item.item_id] = item.item_name;
        });
        return itemMap;
    } catch (err) {
        console.error('Error fetching item names by item IDs:', err);
        throw err;
    }
};


/**
 * Lambda function handler to fetch user details and associated item names.
 *
 * @param {Object} event - The Lambda event object, containing the request body with the `employee_id`.
 * @returns {Promise<Object>} - Returns an object containing user details and item names.
 * @throws {Error} - Throws an error if the `employee_id` is invalid or database queries fail.
 */
export const handler = async (event) => {
    let client;

    try {
        if (!event.body) {
            throw new Error('Request body is missing.');
        }

        const requestBody = JSON.parse(event.body);
        const employeeId = requestBody.employee_id;

        if (!employeeId) {
            throw new Error('Employee ID is missing or invalid.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Get user information
        const user = await getUserByEmployeeId(client, employeeId);

        // Get item names for both items_owned and items_equipped
        const ownedItems = user.items_owned.length > 0 ? await getItemNamesByIds(client, user.items_owned) : {};
        const equippedItems = user.items_equipped.length > 0 ? await getItemNamesByIds(client, user.items_equipped) : {};

        // Format the response
        const response = {
            user_id: user.user_id,
            user_name: user.user_name,
            employee_id: user.employee_id,
            current_coins: user.current_coins,
            total_coins: user.total_coins,
            items_owned: ownedItems,
            items_equipped: equippedItems
        };

        return {
            statusCode: 200,
            body: JSON.stringify(response),
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
