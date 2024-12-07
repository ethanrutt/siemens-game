/* jslint ignore:start */
import { getSecret, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */


export const DBCREDENTIALS = {
    host: "seimensgame.cluster-ro-cby6ieo44z27.us-east-1.rds.amazonaws.com",  // RDS Proxy endpoint
    port: 5432,
    database: "seimensgame",
};

const createDbClient = (credentials) => {
    return new Client({
        host: DBCREDENTIALS.host,
        user: credentials.username,
        password: credentials.password,
        database: DBCREDENTIALS.database,
        port: DBCREDENTIALS.port,
    });
};

const getTaskData = async (client) => {
    try {
        const query = `
            SELECT tasktype, history_employee_original
            FROM taskdata
            WHERE taskisdone = 1
              AND history_datetime_cdt >= NOW() - INTERVAL '30 minutes';
        `;

        const result = await client.query(query);
        return result.rows;
    } catch (err) {
        console.error('Error fetching task data:', err);
        throw err;
    }
};

const getTaskPoints = async (client, taskTypes) => {
    try {
        const query = `
            SELECT tasktype, task_pt
            FROM taskpoints
            WHERE tasktype = ANY($1::int[]);
        `;

        const result = await client.query(query, [taskTypes]);
        const taskPointsMap = {};

        result.rows.forEach(row => {
            taskPointsMap[row.tasktype] = row.task_pt;
        });

        return taskPointsMap;
    } catch (err) {
        console.error('Error fetching task points:', err);
        throw err;
    }
};

const updateUserCoins = async (client, updates) => {
    try {
        await client.query('BEGIN');

        for (const { employee_id, points } of updates) {
            await client.query(
                `UPDATE users
                 SET current_coins = current_coins + $1,
                     total_coins = total_coins + $1
                 WHERE employee_id = $2;`,
                [points, employee_id]
            );
        }

        await client.query('COMMIT');
        console.log('User coins updated successfully.');
    } catch (err) {
        await client.query('ROLLBACK');
        console.error('Error updating user coins:', err);
        throw err;
    }
};

export const handler = async () => {
    let client;

    try {
        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Fetch task data from the last 30 minutes
        const taskData = await getTaskData(client);

        if (taskData.length === 0) {
            console.log('No tasks found in the last 30 minutes.');
            return {
                statusCode: 200,
                body: JSON.stringify({ message: 'No tasks to process.' }),
            };
        }

        // Extract unique task types
        const taskTypes = [...new Set(taskData.map(task => task.tasktype))];

        // Fetch corresponding task points for task types
        const taskPointsMap = await getTaskPoints(client, taskTypes);

        // Aggregate points by employee
        const employeePointsMap = {};
        taskData.forEach(({ tasktype, history_employee_original: employee_id }) => {
            const points = taskPointsMap[tasktype] || 0;
            if (!employeePointsMap[employee_id]) {
                employeePointsMap[employee_id] = 0;
            }
            employeePointsMap[employee_id] += points;
        });

        // Prepare updates for users table
        const updates = Object.entries(employeePointsMap).map(([employee_id, points]) => ({
            employee_id,
            points,
        }));

        // Update user coins in the database
        await updateUserCoins(client, updates);

        return {
            statusCode: 200,
            body: JSON.stringify({ message: 'User coins updated successfully' }),
        };
    } catch (err) {
        console.error('Error processing request:', err);
        return {
            statusCode: 500,
            body: JSON.stringify({ error: err.message }),
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
