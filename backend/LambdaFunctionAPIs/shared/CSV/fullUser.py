import psycopg2
import csv

DB_PARAMS = {
    'host': 'seimensgame.cluster-ro-cby6ieo44z27.us-east-1.rds.amazonaws.com',
    'port': '5432',
    'dbname': 'seimensgame',
    'user': 'seimens',
    'password': 'l-bnJv*QHu~oJlWB4uZ->8OzH9)%'
}

def get_db_connection():
    try:
        connection = psycopg2.connect(**DB_PARAMS)
        return connection
    except Exception as e:
        print(f"Error connecting to the database: {e}")
        return None

# Check if a username already exists
def check_user_exists(cursor, user_name):
    cursor.execute("SELECT COUNT(*) FROM users WHERE user_name = %s", (user_name,))
    return cursor.fetchone()[0] > 0

def update_users_from_employee_table(file_path):
    conn = get_db_connection()
    if not conn:
        return

    try:
        cursor = conn.cursor()

        with open(file_path, 'r') as file:
            reader = csv.DictReader(file)

            print(f"CSV Headers: {reader.fieldnames}")

            for row in reader:
                try:
                    emp_id = row['emp_badge_id'].strip()
                    first_name = row['emp_name'].strip().split()[0]
                except KeyError as e:
                    print(f"Error: {e} not found in the CSV row.")
                    continue

                user_name = first_name.lower()
                
                # Check if the user_name already exists and modify it if needed
                if check_user_exists(cursor, user_name):
                    user_name = f"{user_name}_{emp_id}"  # Ensure uniqueness

                user_password = "siemens"
                current_coins = 0
                total_coins = 0
                items_owned = '{}'
                items_equipped = '{}'

                query = """
                    INSERT INTO users (user_name, user_password, employee_id, current_coins, total_coins, items_owned, items_equipped)
                    VALUES (%s, %s, %s, %s, %s, %s::integer[], %s::integer[])
                    ON CONFLICT (employee_id) 
                    DO UPDATE SET 
                        user_name = EXCLUDED.user_name,
                        user_password = EXCLUDED.user_password,
                        current_coins = EXCLUDED.current_coins,
                        total_coins = EXCLUDED.total_coins,
                        items_owned = EXCLUDED.items_owned,
                        items_equipped = EXCLUDED.items_equipped;
                """

                cursor.execute(
                    query,
                    (user_name, user_password, emp_id, current_coins, total_coins, items_owned, items_equipped)
                )

        conn.commit()
        print("Users table successfully updated.")

    except Exception as e:
        print(f"Error updating users table: {e}")

    finally:
        if conn:
            cursor.close()
            conn.close()

# Provide the path to your CSV file
update_users_from_employee_table('Employee.csv')
