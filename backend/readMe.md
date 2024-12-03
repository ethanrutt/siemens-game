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
### Retrieving Leaderboard Data
1. Leaderboard request hits **API Gateway**.
2. A **Lambda Function** queries **RDS** for top scores.
3. Returns the leaderboard data as a JSON response.

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
