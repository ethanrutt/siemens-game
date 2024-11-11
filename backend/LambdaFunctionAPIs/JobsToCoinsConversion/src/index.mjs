import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Query function to update users' coins
const updateUserCoins = async (client) => {
    try {
        // Start a transaction
        await client.query('BEGIN');

        // Lock the relevant rows with FOR UPDATE
        await client.query(`
            WITH job_points_data AS (
                SELECT 
                    e.emp_badge_id, 
                    COALESCE(jp.jbp_pt, 1) AS job_points
                FROM 
                    jobevent je
                JOIN 
                    employee e ON je.jbe_emp_id = e.emp_id
                LEFT JOIN 
                    jobpoints jp ON je.jbe_pra_id = jp.jbp_pra_id
                WHERE 
                    je.jbe_event IN ('PANDOORCOMPLETE', 'JOBCOMPLETE')
                    AND je.jbe_timestamp >= NOW()::timestamp(0) - INTERVAL '330 minutes'
            ),
            updated_users AS (
                SELECT 
                    u.user_id, 
                    SUM(jd.job_points) AS total_job_points
                FROM 
                    users u
                JOIN 
                    job_points_data jd ON u.employee_id = jd.emp_badge_id
                GROUP BY 
                    u.user_id
            )
            SELECT 
                u.user_id, u.current_coins, u.total_coins, uu.total_job_points
            FROM 
                users u
            JOIN 
                updated_users uu ON u.user_id = uu.user_id
            FOR UPDATE;
        `);

        // Perform the update
        await client.query(`
            UPDATE users 
            SET 
                current_coins = users.current_coins + uu.total_job_points,
                total_coins = users.total_coins + uu.total_job_points
            FROM 
                updated_users uu
            WHERE 
                users.user_id = uu.user_id;
        `);

        // Commit the transaction
        await client.query('COMMIT');
        console.log('User coins updated successfully');
    } catch (err) {
        // Rollback on error
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

        // Update user coins
        await updateUserCoins(client);

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
