import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Query to update users' coins based on job events
const UPDATE_COINS_QUERY = `
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
      AND 
        je.jbe_timestamp >= NOW()::timestamp(0) - INTERVAL '330 minutes'
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
UPDATE users 
SET 
    current_coins = users.current_coins + updated_users.total_job_points,
    total_coins = users.total_coins + updated_users.total_job_points
FROM 
    updated_users
WHERE 
    users.user_id = updated_users.user_id;
`;

// Lambda Handler
export const handler = async () => {
    let client;

    try {
        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client
        client = createDbClient(secret);
        await client.connect();
        console.log("Connected to the database");

        // Execute the update query
        await client.query(UPDATE_COINS_QUERY);
        console.log("User coins updated successfully");

        return {
            statusCode: 200,
            body: JSON.stringify({ message: 'User coins updated successfully' }),
        };
    } catch (err) {
        console.error('Error updating coins:', err);
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
