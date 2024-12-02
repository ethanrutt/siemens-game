/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

/**
 * Fetches all items from the `store` table in the database.
 *
 * @param {Object} client - The PostgreSQL client used to interact with the database.
 * @returns {Promise<Object[]>} - Returns an array of objects, each representing an item from the store table.
 * @throws {Error} - Throws an error if the query fails.
 */
const getAllItems = async (client) => {
    try {
        const result = await client.query('SELECT * FROM store');
        console.log("All items retrieved from the store.");
        return result.rows;
    } catch (err) {
        console.error('Error fetching items from store:', err);
        throw err;
    }
};

/**
 * Lambda function handler to retrieve all items from the store.
 *
 * @returns {Promise<Object>} - Returns an object containing the HTTP status code and a JSON body with the list of items.
 * @throws {Error} - Throws an error if database connection or query fails.
 */
export const handler = async () => {
    let client;
    try {
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        const items = await getAllItems(client);
        return {
            statusCode: 200,
            body: JSON.stringify(items),
        };
    } catch (err) {
        console.error('Error querying the database:', err);
        return {
            statusCode: 500,
            body: 'Error querying the database',
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
