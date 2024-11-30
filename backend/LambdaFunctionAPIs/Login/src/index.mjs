/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

const validateLoginCredentials = async (client, user_name, user_password) => {
    const query = `
        SELECT * FROM users 
        WHERE user_name = $1 AND user_password = $2
    `;
    const values = [user_name, user_password];
    const result = await client.query(query, values);
    console.log(`Login validation for user '${user_name}' completed.`);

    if (result.rows.length > 0) {
        return result.rows[0]; // Return user details if login is successful
    } else {
        throw new Error('Invalid credentials.');
    }
};

const createUser = async (client, employee_id, user_name, user_password) => {

    const checkQuery = `
        SELECT * FROM users 
        WHERE employee_id = $1
    `;
    const result = await client.query(checkQuery, [employee_id]);

    if (result.rows.length > 0) {
        throw new Error('Employee ID already exists.');
    }

    const employeeCheckQuery = `
        SELECT * FROM employee 
        WHERE emp_badge_id = $1
    `;
    const employeeCheckResult = await client.query(employeeCheckQuery, [employee_id]);

    if (employeeCheckResult.rows.length === 0) {
        throw new Error('Invalid employee ID: No matching employee found.');
    }

    const insertQuery = `
        INSERT INTO users 
        (
            user_name, 
            user_password, 
            employee_id, 
            current_coins, 
            total_coins, 
            items_owned, 
            items_equipped, 
            cards_owned, 
            achievements_complete, 
            achievements, 
            has_finished_cutscene, 
            location_x, 
            location_y, 
            current_scene
        ) 
        VALUES 
        ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14) 
        RETURNING *
    `;
    const values = [
        user_name, 
        user_password, 
        employee_id, 
        0, 
        0, 
        '{}', 
        '{}', 
        '{}', 
        '{}', 
        '{}', 
        false, 
        0.0, 
        0.0, 
        'MainMenu'
    ];

    const insertResult = await client.query(insertQuery, values);
    console.log(`User '${user_name}' created successfully.`);
    return insertResult.rows[0];
};

const errorCheck = (event) => {
    if (!event.body) {
        throw new Error('Request body is missing.');
    }

    const requestBody = JSON.parse(event.body);
    const { 
        user_name, 
        user_password, 
        employee_id 
    } = requestBody;

    if (!user_name || !user_password) {
        throw new Error('Missing required parameters: user_name or user_password.');
    }

    return { 
        user_name, 
        user_password, 
        employee_id 
    };
};

export const handler = async (event) => {
    let client;

    try {
        // Extract parameters from the request body
        const { 
            user_name, 
            user_password, 
            employee_id 
        } = errorCheck(event);

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        let user;
        if (employee_id) {
            // If employee_id is present, proceed with signup
            user = await createUser(client, employee_id, user_name, user_password);
        } else {
            // Otherwise, validate login credentials
            user = await validateLoginCredentials(client, user_name, user_password);
        }

        // Successful response
        return {
            statusCode: 200,
            body: JSON.stringify({
                message: employee_id ? 'Signup successful' : 'Login successful',
                user: {
                    user_id: user.user_id,
                    user_name: user.user_name,
                    employee_id: user.employee_id,
                    current_coins: user.current_coins,
                    total_coins: user.total_coins,
                    items_owned: user.items_owned,
                    items_equipped: user.items_equipped,
                    cards_owned: user.cards_owned,
                    achievements_complete: user.achievements_complete,
                    achievements: user.achievements,
                    has_finished_cutscene: user.has_finished_cutscene,
                    location_x: user.location_x,
                    location_y: user.location_y,
                    current_scene: user.current_scene,
                },
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
