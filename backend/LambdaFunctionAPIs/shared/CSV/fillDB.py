import psycopg2

# Database connection parameters
DB_PARAMS = {
    'host': 'seimensgame.cluster-ro-cby6ieo44z27.us-east-1.rds.amazonaws.com',
    'port': '5432',
    'dbname': 'seimensgame',
    'user': 'seimens',
    'password': 'l-bnJv*QHu~oJlWB4uZ->8OzH9)%'
}

# Full item database
item_database = {
    100: {"name": "Orange Hard Hat", "type": "hat", "price": 100},
    101: {"name": "White Hard Hat", "type": "hat", "price": 100},
    102: {"name": "Yellow Hard Hat", "type": "hat", "price": 100},
    103: {"name": "Orange Party Hat", "type": "hat", "price": 250},
    104: {"name": "Purple Party Hat", "type": "hat", "price": 250},
    105: {"name": "Blue Party Hat", "type": "hat", "price": 250},
    106: {"name": "Cone", "type": "hat", "price": 500},
    107: {"name": "Medieval Helmet", "type": "hat", "price": 500},
    108: {"name": "A&M Football Helmet", "type": "hat", "price": 0},
    # Chest items
    200: {"name": "White Hoodie", "type": "chest", "price": 100},
    201: {"name": "Grey Hoodie", "type": "chest", "price": 100},
    202: {"name": "Black Hoodie", "type": "chest", "price": 100},
    203: {"name": "Blue Hoodie", "type": "chest", "price": 100},
    204: {"name": "Green Hoodie", "type": "chest", "price": 100},
    205: {"name": "Black Safety Vest", "type": "chest", "price": 250},
    206: {"name": "Grey Safety Vest", "type": "chest", "price": 250},
    207: {"name": "Orange Safety Vest", "type": "chest", "price": 250},
    208: {"name": "Medieval Armor", "type": "chest", "price": 500},
    # Leggings
    300: {"name": "Black Utility Pants", "type": "leggings", "price": 100},
    301: {"name": "Blue Utility Pants", "type": "leggings", "price": 100},
    302: {"name": "Grey Utility Pants", "type": "leggings", "price": 100},
    303: {"name": "White Nano-Fiber Leggings", "type": "leggings", "price": 250},
    304: {"name": "Grey Nano-Fiber Leggings", "type": "leggings", "price": 250},
    305: {"name": "Black Nano-Fiber Leggings", "type": "leggings", "price": 250},
    306: {"name": "Medieval Leggings", "type": "leggings", "price": 500},
    # Footwear
    400: {"name": "Brown Utility Boots", "type": "shoes", "price": 100},
    401: {"name": "Grey Utility Boots", "type": "shoes", "price": 100},
    402: {"name": "Black Utility Boots", "type": "shoes", "price": 100},
    403: {"name": "BW Jordan 1", "type": "shoes", "price": 250},
    404: {"name": "Retro Jordan 1", "type": "shoes", "price": 250},
    405: {"name": "UNC Jordan 1", "type": "shoes", "price": 250},
    406: {"name": "Medieval Boots", "type": "shoes", "price": 500},
    # Dances
    500: {"name": "Head-Ripper", "type": "dance", "price": 250},
    501: {"name": "Robot Dance", "type": "dance", "price": 250},
    502: {"name": "Zen Flip", "type": "dance", "price": 500},
   
}

def insert_items():
    conn = psycopg2.connect(**DB_PARAMS)
    try:
        # Connect to the database
        
        cursor = conn.cursor()

        # Insert each item into the store table
        for item_id, item_data in item_database.items():
            cursor.execute(
                """
                INSERT INTO store (item_id, item_name, item_type, item_price)
                VALUES (%s, %s, %s, %s)
                ON CONFLICT (item_id) DO NOTHING;
                """,
                (item_id, item_data["name"], item_data["type"], item_data["price"])
            )

        # Commit the transaction
        conn.commit()
        print("Items successfully inserted into the store table.")

    except Exception as e:
        print(f"Error inserting items: {e}")

    finally:
        if conn:
            cursor.close()
            conn.close()

# Run the insertion function
insert_items()
