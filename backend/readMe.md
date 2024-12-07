# ByteCity Backend System Design

This document outlines the backend system design for ByteCity, detailing the architecture, components, and their interactions.

---

## Architecture Overview

The backend is designed to support a mobile game with real-time data updates, secure database interactions, and scalable API endpoints. It leverages AWS services for serverless computing, database management, and API delivery.

### Key Features
- Scalable API endpoints using **AWS API Gateway**.
- Serverless functions implemented with **AWS Lambda**.
- Secure database connections via **RDS Proxy** and **Secrets Manager**.
- Data persistence using **Amazon RDS** (PostgreSQL).
- High availability and fault tolerance.

---

## System Components

### 1. **API Gateway**
- Acts as the entry point for all client requests.
- Manages HTTP API routes and integrates with Lambda functions.
- Supports **CORS** to enable secure communication with the frontend.

### 2. **Lambda Functions**
- Implements the backend logic:
  - Updating user coins and task points.
  - Retrieving leaderboard data.
  - Managing user authentication and profile updates.
- Written in **Node.js** for efficient asynchronous handling.
- Stateless design ensures scalability.

### 3. **RDS Proxy**
- Optimizes database connection management for Lambda functions.
- Reduces the number of direct connections to the database.
- Provides secure and efficient access to the **RDS instance**.

### 4. **Amazon RDS**
- Hosts the PostgreSQL database for persistent storage.
- Contains tables for users, tasks, points, and other game data.
- Configured for **multi-AZ deployment** to ensure high availability.

### 5. **AWS Secrets Manager**
- Manages and rotates database credentials.
- Provides secure access to secrets for Lambda functions.

### 6. **IAM Roles and Policies**
- Lambda functions have specific permissions for accessing RDS Proxy and Secrets Manager.
- API Gateway uses IAM roles for secure integration with Lambda.

## Data Flow

### User Authentication
1. **API Gateway** receives login/signup requests.
2. **Lambda Function** validates user credentials.
3. Retrieves user data from **RDS** via **RDS Proxy**.

### Updating Coins
1. Task completion data is sent to **API Gateway**.
2. A **Lambda Function**:
   - Retrieves task points from `taskpoints`.
   - Updates the user's coins in the `users` table.
3. Another **Lambda Function** 
   - Retrieves job points from `jobpoints`.
   - Updates the user's coins in the `users` table.

---

## Backend API Endpoints

### 1. **Authentication**
- **POST** `/auth/login`: Validates user credentials.

### 2. **User Management**
- **GET** `/user/{id}`: Fetches user details.
- **PATCH** `/user/{id}/update`: Updates user profile or coins.

### 3. **Tasks**
- **GET** `/tasks`: Retrieves task data for the user.
- **POST** `/tasks/complete`: Updates task completion status.

### 4. **Leaderboard**
- **GET** `/leaderboard`: Fetches the leaderboard data.

---

## Deployment

### Steps
1. Deploy **API Gateway** with defined routes.
2. Deploy **Lambda Functions** with IAM roles.
3. Configure **RDS Proxy** and link it to the **RDS Instance**.
4. Set up **Secrets Manager** to manage database credentials.
5. Link all components through the **VPC**.

---

## Future Considerations

1. **Scaling**:
   - Use AWS Auto Scaling for RDS and Lambda concurrency limits.
2. **Monitoring**:
   - Enable **CloudWatch Logs** for API Gateway, Lambda, and RDS.
3. **Security Enhancements**:
   - Enforce stricter IAM policies.
   - Enable database encryption at rest and in transit.

---

# Backend API Endpoints

## 1. **Equip API**
- **POST** `/Equip`
  - **Purpose:** Update the `items_equipped` for a user based on the provided action (`equip` or `unequip`).
  - **Request Payload:**
    ```json
    {
      "employee_id": "string",
      "items": ["number"],
      "action": "string" // Options: "equip" or "unequip"
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "statusCode": 200,
        "message": "Successfully equipped/unequipped items for employee ID <employee_id>",
        "items_equipped": ["number"]
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 2. **GetAllItems API**
- **GET** `/GetAllItems`
  - **Purpose:** Fetch all items from the `store` table.
  - **Request Payload:** None
  - **Response:**
    - **Success (200):**
      ```json
      [
        {
          "item_id": "number",
          "item_name": "string",
          "item_type": "string",
          ...additional item properties
        }
      ]
      ```
    - **Error (500):**
      ```json
      "Error querying the database"
      ```

---

## 3. **GetItemsByCategory API**
- **POST** `/GetItemsByCategory`
  - **Purpose:** Fetch items from the `store` table based on the specified category.
  - **Request Payload:**
    ```json
    {
      "category": "string"
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "category": "string",
        "items": [
          {
            "item_id": "number",
            "item_name": "string",
            "item_type": "string",
            ...additional item properties
          }
        ]
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 4. **GetUserItems API**
- **POST** `/GetUserItems`
  - **Purpose:** Retrieve user details, including their owned and equipped items.
  - **Request Payload:**
    ```json
    {
      "employee_id": "string"
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "user_id": "number",
        "user_name": "string",
        "employee_id": "string",
        "current_coins": "number",
        "total_coins": "number",
        "items_owned": { "item_id": "item_name" },
        "items_equipped": { "item_id": "item_name" }
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 5. **HighScore API**
- **POST** `/HighScore`
  - **Purpose:** Retrieve a user's score and rank in a specific game.
  - **Request Payload:**
    ```json
    {
      "user_id": "number",
      "game_id": "number"
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "score": "number",
        "rank": "number"
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 6. **Leaderboard API**
- **POST** `/Leaderboard`
  - **Purpose:** Retrieve the top 10 scores for a specific game.
  - **Request Payload:**
    ```json
    {
      "game_id": "number"
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      [
        {
          "user_name": "string",
          "score": "number"
        }
      ]
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 7. **Login API**
- **POST** `/Login`
  - **Purpose:** Handle user login and signup operations.
  - **Request Payload:**
    ```json
    {
      "user_name": "string",
      "user_password": "string",
      "employee_id": "string" // Optional, required for signup
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "message": "Login/Signup successful",
        "user": {
          "user_id": "number",
          "user_name": "string",
          "employee_id": "string",
          "current_coins": "number",
          "total_coins": "number",
          ...other user properties
        }
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 8. **PlayerDataUpload API**
- **POST** `/PlayerDataUpload`
  - **Purpose:** Update the player’s data in the database.
  - **Request Payload:**
    ```json
    {
      "user_id": "number",
      "current_coins": "number",
      "items_owned": ["number"],
      "items_equipped": ["number"],
      "cards_owned": ["number"],
      "achievements_complete": ["number"],
      "achievements": { "string": "string" },
      "has_finished_cutscene": "boolean",
      "location_x": "number",
      "location_y": "number",
      "current_scene": "string",
      "interactions": { "string": "number" }
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "message": "Player data updated successfully",
        "user": {
          "user_id": "number",
          ...updated user data
        }
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

## 9. **ScoreUpload API**
- **POST** `/ScoreUpload`
  - **Purpose:** Handle game score upload operations with conditional logic based on the game.
  - **Request Payload:**
    ```json
    {
      "user_id": "number",
      "game_id": "number",
      "score": "number" // Optional for game_id = 6
    }
    ```
  - **Response:**
    - **Success (200):**
      ```json
      {
        "message": "Score operation completed successfully",
        "score": {
          "user_id": "number",
          "game_id": "number",
          "score": "number"
        }
      }
      ```
    - **Error (400):**
      ```json
      {
        "error": "Error message describing the issue."
      }
      ```

---

# Backend Installation

### Prerequisites
1. **AWS Account**: Access to the AWS Management Console.
2. **AWS CLI Installed**: [Install AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html).
3. **IAM Permissions**: Permissions to create resources:
   - RDS
   - Secrets Manager
   - API Gateway
   - Lambda
   - IAM roles
4. **VPC Setup**: Access to a VPC with subnets and security groups.

---

### Architecture Components
The backend includes the following AWS services:
1. **Amazon RDS**: A PostgreSQL database for storing ByteCity data.
2. **Secrets Manager**: Securely stores database credentials.
3. **RDS Proxy**: Optimizes and secures Lambda database connections.
4. **Lambda Functions**: Handle backend logic.
5. **API Gateway**: Provides REST/HTTP APIs to the frontend.

---

## Step-by-Step Setup


### 1. Create an RDS Database

1. Go to **RDS → Create Database**.
2. Select **PostgreSQL** as the engine.
3. Configure:
   - Instance class: `db.t3.micro` (for development).
   - Storage: Adjust as needed.
   - Public Access: **No** (for security).
   - Enable IAM database authentication.
4. Create a database user and password for the instance.
5. Set up a **security group** to allow traffic from the RDS Proxy(make it allow all traffic, 0.0.0.0/0, unless Siemens says otherwise).

---

### 2. Set Up Secrets in AWS Secrets Manager

1. Go to **Secrets Manager → Store a new secret**.
2. Choose **RDS Database Credentials** and enter:
   - Database username and password.
3. Name the secret
4. Attach an **IAM policy** to allow Lambda to access this secret:
   ```json
   {
       "Version": "2012-10-17",
       "Statement": [
           {
               "Effect": "Allow",
               "Action": "secretsmanager:GetSecretValue",
               "Resource": "arn:aws:secretsmanager:<region>:<account_id>:secret:<secretname>"
           }
       ]
   }
---

## 3. Setup Before Deploying Lambda Functions 
### Configure AWS Locally
1. run aws configure on your local machine
2. give the corresponding values from your aws account 
  - Values can be found:
  1. Go to AWS Dashboard
  2. Click top right on your account name
  3. Click security credentials
  4. Scroll to Access keys, create one if there isn't one available

### Create Lambda role:
1. Go to IAM then go to Roles
2. Create a role and attach the following policies:
- AWSLambdaBasicExecutionRole
- AWSLambdaVPCAccessExecutionRole
- CloudFrontFullAccess  (for logging)
- SecretsManagerReadWrite
- Custom Inline Role:
```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "rds-db:connect"
            ],
            "Resource": "arn:aws:rds:us-east-1:<awsaccountid>:db-proxy:<proxyid>"
        }
    ]
}
```

### Create Lambda Functions:
1. For every single folder in siemens-game/backend/LambdaFunctionAPIs except for "shared" and "Test": create a lambda function(step 2)
2. Go to the lambda console in AWS
    -  click "Create Function"
    -  Choose a Function Name, it should equal the variable "const functionName" in the build.js(line 17) file in the corresponding folder
    - If you choose a different function name, make sure build.js(line 17) has the same name, as that is the deployment script
    - Runtime: Node.js
    - Architecture x86_64
    - Change default Excecution Role and choose the role you created above
    - Create Function
2. Example environment variables:
   - `DB_SECRET_ARN`: ARN of the secret in Secrets Manager.
   - `DB_PROXY_ENDPOINT`: Endpoint of the RDS Proxy.

### Deploy the Functions:
#### BEFORE: Fix Credentials:
1. Navigates to siemens-game/backend/LambdaFunctionAPIs/shared/utils.mjs
2. Change DBCredentials to the your database values
3. Change secret_ame to your secret name
build.js is the deployment script 

1. Navigate to the folder within LambdaFunctionAPIs you want to deploy(Ex: Equip )
2. `npm init -y`
3. `npm install archiver aws-sdk pg @aws-sdk/client-secrets-manager`
4. `npm install jest @jest/globals --save-dev`
5. `npm run deploy` (runs the build.js file and should upload your function lambda)

---

## 4. Set Up API Gateway (Don't do this for JobToCoinsConverter & TaskToCoinsConverter)
### Create an API Gateway:
1. Go to **API Gateway** → **Create API**.
2. Choose **HTTP API**.

### Create Routes:
1. Map each Lambda function to a route.
2. Example routes:
   - look at the `APIPayload.txt` to see the type and how its structured,
   or look above at Backend API Endpoints section
   - EX: `/Login` → Login Lambda Function.

### Integrate API with Lambda:
1. For each route, set the integration type to **Lambda Function**.
2. Deploy the API and note the endpoint URL(You will need to replace these in the Unity code).
  - Search for "string url = " in the repositoty
  - Replace it with the correct values: string url = "https://<APIURL(CHANGE THIS)>/default/<endpoint>";


## 5. Configure Networking
### VPC Setup:
1. **VPC Configuration**:
   - Navigate to **VPC** in the AWS Management Console.
   - Ensure that the VPC used for the **RDS instance**, **RDS Proxy**, and **Lambda functions** is the same.
   - This ensures that all components can communicate within the private network without requiring public endpoints.

2. **Subnets**:
   - Verify that your VPC has private subnets in multiple availability zones for redundancy.
   - Ensure that the subnets are associated with the **RDS instance**, **RDS Proxy**, and **Lambda functions**.

3. **Route Tables**:
   - Check that the subnets have proper route tables.
   - If your Lambda needs access to external services (e.g., Secrets Manager or external APIs), configure a NAT gateway or NAT instance for internet access.

---

### Security Groups:
1. **RDS Proxy Security Group**:
   - Navigate to **RDS Proxy** → **Modify** → **Security Groups**.
   - Add the security group for Lambda, allowing inbound traffic on port `5432`.

2. **Lambda Security Group**:
   - Navigate to **Lambda Function** → **Configuration** → **VPC**.
   - Attach the same security group used by the **RDS Proxy**.
   - Ensure that the security group allows outbound traffic to the RDS Proxy on port `5432`.

3. **RDS Security Group**:
   - Navigate to **RDS Instance** → **Modify** → **Security Groups**.
   - Ensure that the RDS instance has a security group allowing inbound traffic from the **RDS Proxy Security Group**.

4. **Testing Security Groups**:
   - Test connectivity by temporarily adding your IP address to the RDS instance or Proxy security group.
   - Use tools like **telnet** or **nc** to check if the port `5432` is accessible.

---
## Task and Job Data System Overview and Integration Recommendations
### Overview

The integration logic processes **tasks** and **jobs** separately:

1. **Tasks**: Fetch completed tasks, map task types to points, and update the `users` table with aggregated points.
2. **Jobs**: Fetch completed job events, map PRA IDs to points, and update the `users` table with aggregated points.

---

### Task Points Integration

1. **Retrieve Completed Tasks**:
   - Query the `taskdata` table for tasks with `taskisdone = 1` and `history_datetime_cdt` within the last 30 minutes.

2. **Map Task Types to Points**:
   - Query the `taskpoints` table to fetch points corresponding to `tasktype`.

3. **Aggregate Points by Employee**:
   - Sum the points for each `history_employee_original` (employee ID).

4. **Update User Coins**:
   - Update the `current_coins` and `total_coins` in the `users` table for each `employee_id`.

---

### Job Points Integration

1. **Retrieve Job Events**:
   - Query the `jobevent` table for events (`'PANDOORCOMPLETE'`, `'JOBCOMPLETE'`) and `jbe_timestamp` within the last 30 minutes.

2. **Map Job PRA IDs to Points**:
   - Query the `jobpoints` table to fetch points corresponding to `jbe_pra_id`.

3. **Aggregate Points by Employee**:
   - Sum the points for each `jbe_emp_id` (employee ID).

4. **Update User Coins**:
   - Update the `current_coins` and `total_coins` in the `users` table for each `employee_id`.

---

### Data Flow Visualization

#### Task Points Flow

+---------------+
|   taskdata    |
+---------------+
        |
        v
+---------------+   +---------------+
| Filter tasks  |   | taskpoints    |
| last 30 mins  |   | (tasktype)    |
+---------------+   +---------------+
        |                  |
        +------------------+
               |
               v
+-----------------------+
| Aggregate Points by   |
| Employee ID           |
+-----------------------+
               |
               v
+-----------------------+
| Update users Table    |
| (current_coins,       |
|  total_coins)         |
+-----------------------+


#### Job Points Flow

+---------------+
|   jobevent    |
+---------------+
        |
        v
+---------------+   +---------------+
| Filter jobs   |   | jobpoints     |
| last 30 mins  |   | (jbe_pra_id)  |
+---------------+   +---------------+
        |                  |
        +------------------+
               |
               v
+-----------------------+
| Aggregate Points by   |
| Employee ID           |
+-----------------------+
               |
               v
+-----------------------+
| Update users Table    |
| (current_coins,       |
|  total_coins)         |
+-----------------------+


## Recommendations to Integrate

### Step 1: Update Database Credentials

1. **Edit `index.mjs`**:
   - Locate the `DBCREDENTIALS` constant.
   - Update the `host`, `port`, and `database` values with the actual Siemens RDS database endpoint.

   ```javascript
   export const DBCREDENTIALS = {
       host: "siemensdb.example.us-east-1.rds.amazonaws.com",  // RDS Proxy endpoint
       port: 5432,
       database: "siemens",
   };


### Step 3: Schedule Lambda Execution
1. **Create a CloudWatch Rule**:
  - Navigate to CloudWatch → Rules → Create Rule.
  - Select Event Source: Fixed rate of 30 minutes.
  - Link Lambda to Rule:

Choose the Lambda function created earlier as the target.
Ensure the Lambda function is triggered every 30 minutes.

---

### Key Points:
- Keeping all components within the same VPC ensures low latency and secure communication.
- Security groups act as firewalls to regulate traffic, so configuring them correctly is essential for both security and functionality.

---

