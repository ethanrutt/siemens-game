# ByteCity
## Siemens Manufacturing x Texas A&M Computer Science
## CSCE 482 - 932

## Table of Contents
- [Team Members](#team-members)
- [Background](#background)
- [Requirements](#requirements)
- [Proposal](#proposal)
- [Tech Stack](#tech-stack)
- [Frontend Installation](#frontend-installation)
- [Backend Installation](#backend-installation)
- [Testing](#testing)
- [Frontend Testing](#frontend-testing)
- [Backend Testing](#backend-testing)
- [Deployment](#deployment)
- [Android Deployment](#android-deployment)
- [iOS Deployment](#ios-deployment)
- [Future Development](#future-development)

---
## Team Members
* Rohan Ali
* Naveed Haq
* Ethan Rutt
* Rishi Santhanam

---
## Background
* Siemens came to Texas A&M requesting a project to improve employee morale and
  productivity in their factory line.
* Siemens already had a way to track which tasks needed to be done for an
  order. They had a system in place to have these tasks in some type of queue,
  load them in to the computers, assign them to a worker, and have the worker
  track when the tasks are complete.
* Siemens proposed to link this data to a game, where we could convert these
  task completion data to in game currency. The employees would also have a way
  to hang out with each other, and participate in minigames as well as see the
  leaderboards for who earned the most coins, who has the fastest time in the
  minigame, etc.

---
## Requirements
* Siemens wanted the game to be able to run on mobile devices, both iOS and
  Android.
* They also wanted to be able to sync in-game currency with what workers
  complete in the factory.
* They also wanted a fun experience that workers would enjoy playing. This was
  very open-ended.

---
## Proposal
* We proposed an open world RPG game inspired from Club Penguin.
* It involves a story, achievements, mini-games, an open world to explore, and
  more.

---
## Tech Stack
### Frontend
* Since this game is mainly developed for Android and iOS, we decided to use
  the [Unity Engine](https://unity.com/) to develop our game with
  [C#](https://dotnet.microsoft.com/en-us/languages/csharp) as the scripting
  language.
* We are using `Unity Editor Version 2022.3.45f1`.
### Backend
* We are using [nodejs](https://nodejs.org/en/) to define HTTP APIs to connect to the Database
* Code
  * NodeJS, npm
  * jest for testing
* AWS Resources
  * API Gateway for routing
  * AWS Lambda for API Definitions
  * Secrets Manager for authentication
  * RDS Proxy for Connection Management
  * RDS DB for the Database


## Frontend Installation
* Installing Unity is pretty straight forward. Go to
  `https://unity.com/download` or click [here](https://unity.com/download) and
  download `Unity Hub`.
* This will be the main area you launch your Unity Engine from.
* Once Unity Hub is installed, you'll want to make sure to download Unity
  Editor Version `2022.3.45f1` which is the version that we have developed
  ByteCity with.
* After this is installed, you'll want to clone this repository.
* Using https.
```bash
git clone https://github.com/ethanrutt/siemens-game.git
```
* Using ssh.
```bash
git clone git@github.com:ethanrutt/siemens-game.git
```
* Once this is complete, you'll want to
  * Open up Unity Hub.
  * Click `Add` in the top right.
  * Click `add project from disk`.
  * Find this repository that you just cloned.
  * Click add/open.
  * Finally, click on the new entry in the projects window of unity hub which
    should say `siemens-game`.
  * The first time you open up Unity, it will need to read all of the packages we
    have installed from the `Packages/manifest.json` and
    `Packages/package-lock.json`, as well as compile all of our scripts.
* Depending on the speed of your computer, this could take up to 10 minutes.
* Once this is finished, you should be ready to develop, build, and run
  ByteCity!

## Backend Installation

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
   ```
---

## 3. Deploy Lambda Functions
### Configure AWS Locally
1. run aws configure on your local machine
2. give the corresponding values from your aws account

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
    -  Choose a Function Name, it should equal  the variable "const functionName" in the build.js(line 17) file in the corresponding folder
    - Runtime: Node.js
    - Architecture x86_64
    - Change default Excecution Role and choose the role you created above
    - Create Function
2. Example environment variables:
   - `DB_SECRET_ARN`: ARN of the secret in Secrets Manager.
   - `DB_PROXY_ENDPOINT`: Endpoint of the RDS Proxy.

### Deploy the Functions:
1. Navigate to the folder you want to deploy(Ex: Equip )
2. `npm init -y`
3. `npm install archiver aws-sdk pg @aws-sdk/client-secrets-manager`
4. `npm install jest @jest/globals --save-dev`
5. `npm run deploy` (runs the build.js file and should upload your function lambda)

---

## 4. Set Up API Gateway
### Create an API Gateway:
1. Go to **API Gateway** → **Create API**.
2. Choose **HTTP API**.

### Create Routes:
1. Map each Lambda function to a route.
2. Example routes:
   - look at the `APIPayload.txt` to see the type and how its structure
   - `/login` → Login Lambda Function.

### Integrate API with Lambda:
1. For each route, set the integration type to **Lambda Function**.
2. Deploy the API and note the endpoint URL.

### Enable CORS:
- If the frontend interacts with this API, enable **CORS** for each route.

---

## 5. Configure Networking
### VPC Setup:
- Ensure that **RDS**, **RDS Proxy**, and **Lambda** are in the same **VPC**.

### Security Groups:
- Allow traffic between **Lambda** and **RDS Proxy** on port `5432`.

## Testing
### Frontend Testing
* We have a combination of github actions, combined with
  [game-ci](https://game.ci/) to automatically test whenever we push code.
* Inside of Unity itself, there are edit mode tests and play mode tests
  possible in [Unity Test Runner](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/index.html).
* Both are essential to testing functionality.
* The methodology we have used is
  * Play Mode Tests
    * These are for testing in-game functionality such as player movement,
      connecting wires, etc.
  * Edit Mode Tests
    * These are for making sure that assets can be properly loaded, they are in
      the right place and other things that you could test without actually
      playing the game.

### Backend Testing
* We use [jest](https://jestjs.io/) to test our backend code combined with
  github actions to get automated testing whenever we push code.
* Testing framework: [Jest](https://jestjs.io/).
* Automated testing with GitHub Actions for every push to the repository.
* Folders containing tests (siemens-game/backend/LambdaFunctionAPIs):
  * Equip
  * GetAllItems
  * GetItemsByCategory
  * GetUserItems
  * HighScore
  * Leaderboard
  * Login
  * playerDataUpload
  * ScoreUpload
* Focus areas:
  * Unit tests for functions like `updatePlayerData` and `getTopScoresByGame`.
  * Integration tests for AWS Lambda handlers.
  * API tests for endpoints.

---
## Deployment
### Android Deployment
* There should already be Android Build Support installed from our packages,
  but if there isn't then you can follow these steps.
* Open Unity Hub.
    * Click on `Installs` on the left.
    * Find the Unity Version used with this project `2022.3.45f1`.
    * Click the gear on the far right of the Unity Version.
    * Click `Add modules`.
    * Click `Android Build Support`.
    * Click `Install`.
* After you have installed Android Build Support, open up the Unity Editor by
  clicking on `siemens-game` in Unity Hub.
  * Go to File -> Build Settings.
  * Click `Android`.
  * Click `Switch Platform`.
  * Click `Player Settings` in the bottom left.
  * Make sure the `Company Name` is set to `Siemens`.
  * Make sure the `Product Name` is set to `Byte City`.
  * Set the version to whatever version you are releasing.
  * Click Build.
* Once the project has successfully been built, you should have a `.apk` file.
* This is how you will install the game onto an Android Device.
  * To distribute this `.apk` file, you can upload it to a file sharing service,
    or use a usb to directly transfer it to a device.
  * Once it's on the device, simply running it should install the game as an app.
* When on the android device
  * You might have to scan the file before installing it. This is normal.
  * You also might have to allow third-party apps to be installed.
      * For the most part, this will just pop up as a prompt whenever you install
        the `.apk`.
      * There is also an option in settings app to allow third-party app
        installations.

### iOS Deployment
* **note that you need to have a macbook or a way to launch xcode to build for
  iOS**.
* Open Unity Hub.
    * Click on `Installs` on the left.
    * Find the Unity Version used with this project `2022.3.45f1`.
    * Click the gear on the far right of the Unity Version.
    * Click `Add modules`.
    * Click `iOS Build Support`.
    * Click `Install`.
* After you have installed iOS Build Support, open up the Unity Editor by
  clicking on `siemens-game` in Unity Hub.
  * Go to File -> Build Settings.
  * Click `iOS`.
  * Click `Switch Platform`.
  * Click `Player Settings` in the bottom left.
  * Make sure the `Company Name` is set to `Siemens`.
  * Make sure the `Product Name` is set to `Byte City`.
  * Set the version to whatever version you are releasing.
  * Click Build.
* You should have an `.xcode` file that you will need to open in XCode.
* This file will allow you to build the game on iOS, and you should follow the
  steps that XCode gives you with regards to this process.

---
## Documentation
* We have generated our documentation using [doxygen](https://www.doxygen.nl/index.html).
* Make sure that any new code written has [javadoc style comments](https://www.oracle.com/technical-resources/articles/java/javadoc-tool.html).
* Generate new documentation using our preconfigured `Doxyfile` by running
```
doxygen Doxyfile
```
* View the documentation [here](https://ethanrutt.github.io/bytecity_docs/html/index.html)
* Some Notes about the Doxyfile config
  * We are using `.mjs` files for our backend, which aren't recognized by the doxygen parser by default. This line fixes this which just says that `.mjs` files should use the `Javascript` parser.
  ```
  EXTENSION_MAPPING = mjs=Javascript
  ```
  * The Main Page is set to this readme file.
  * The backend documentation is found by searching through the files on the doxygen website.
  * The game documentation is found by searching through the class list on the doxygen website.

---
## Future Development
* The Byte City team has put tons of hours into this game. We were given nothing
  but an open-ended request with simple constraints, and we have built the
  Byte City you can play today from scratch.
* We are extremely proud of what we were able to accomplish in around 3 months
  while taking other courses.  Balancing development through other classwork,
  exams, and the various organizations we are involved in was a challenge, but
  all of us have grown so much.
* With that being said, there were still multiple goals that we weren't able to
  complete in our project timeline.
* These include
    * Open World Multiplayer
        * This will require a dedicated multiplayer server, syncing player
          positions, animations, and more.
    * Friends List
        * Allowing users to add their friends by username.
    * Chat Rooms
        * Allowing users to have private messages, group chats, and even a
          global chat.
* There are also certain aspects of the game that aren't as polished as we
  would like them to be.
    * Peculiar Pipes leaves a lot to be desired, both content-wise and
      looks-wise.
    * Wacky Wires leaves a lot to be desired, both game-mechanic-wise and
      looks-wise.
    * Some of the dialogue can be a little clunky at times.
    * The movement system can be a little clunky at times.
