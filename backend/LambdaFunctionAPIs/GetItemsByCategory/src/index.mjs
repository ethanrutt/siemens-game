/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

/**
 * Fetches items from the `store` table based on the specified category.
 *
 * @param {Object} client - The PostgreSQL client used to interact with the database.
 * @param {string} category - The category of items to fetch.
 * @returns {Promise<Object[]>} - Returns an array of objects, each representing an item from the specified category.
 * @throws {Error} - Throws an error if the query fails.
 */
const getItemsByCategory = async (client, category) => {
    try {
        const result = await client.query('SELECT * FROM store WHERE item_type = $1', [category]);
        console.log(`Items of category '${category}' retrieved from the store.`);
        return result.rows;
    } catch (err) {
        console.error('Error fetching items by category from store:', err);
        throw err;
    }
};

/**
 * Validates and extracts the category parameter from the event body.
 *
 * @param {Object} event - The Lambda event object.
 * @returns {string} - The category parameter from the request body.
 * @throws {Error} - Throws an error if the request body or category parameter is missing or invalid.
 */
const errorCheck = async (event) => {
    if (!event.body) {
        throw new Error('Request body is missing.');
    }
    const requestBody = JSON.parse(event.body);
    const category = requestBody.category;

    // Validate that the category parameter is provided
    if (!category) {
        throw new Error('Category parameter is missing or invalid.');
    }
    return category;
};

/**
 * Lambda function handler to fetch items from the `store` table by category.
 *
 * @param {Object} event - The Lambda event object, containing the request body with the category parameter.
 * @returns {Promise<Object>} - Returns an object containing the HTTP status code, the category, and a JSON body with the list of items.
 * @throws {Error} - Throws an error if database connection or query fails, or if the category parameter is invalid.
 */
export const handler = async (event) => {
    let client;

    try {
        // Await the errorCheck function to handle asynchronous code properly
        const category = await errorCheck(event);

        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        const items = await getItemsByCategory(client, category);

        return {
            statusCode: 200,
            category: category,
            body: JSON.stringify(items),
        };
    } catch (err) {
        console.error('Error querying the database:', err);
        return {
            statusCode: 400, // Use 400 status code to indicate a bad request
            body: JSON.stringify({ error: err.message }),
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
