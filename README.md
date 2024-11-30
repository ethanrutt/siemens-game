# ByteCity
## Siemens Manufacturing x Texas A&M Computer Science
### CSCE 482 - 932

### Table of Contents
- [Team Members](#team-members)
- [Background](#background)
- [Requirements](#requirements)
- [Proposal](#proposal)
- [Tech Stack](#tech-stack)
- [Frontend Installation](#frontend-installation)
- [Backend Installation](#backend-installation)
- [Frontend Testing](#frontend-testing)
- [Backend Testing](#backend-testing)
- [Android Deployment](#android-deployment)
- [iOS Deployment](#ios-deployment)
- [Future Development](#future-development)

### Team Members
* Rohan Ali
* Naveed Haq
* Ethan Rutt
* Rishi Santhanam

### Background
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

### Requirements
* Siemens wanted the game to be able to run on mobile devices, both iOS and
  Android.
* They also wanted to be able to sync in-game currency with what workers
  complete in the factory.
* They also wanted a fun experience that workers would enjoy playing. This was
  very open-ended.

### Proposal
* We proposed an open world RPG game inspired from Club Penguin.
* It involves a story, achievements, mini-games, an open world to explore, and
  more.

### Tech Stack
#### Frontend
* Since this game is mainly developed for Android and iOS, we decided to use
  the [Unity Engine](https://unity.com/) to develop our game with
  [C#](https://dotnet.microsoft.com/en-us/languages/csharp) as the scripting
  language
* We are using `Unity Editor Version 2022.3.45f1`
#### Backend
* We are using [nodejs](https://nodejs.org/en/), along with AWS Lambda and an
  **naveed fix this part plz**

#### Frontend Installation
* Installing Unity is pretty straight forward. Go to
  `https://unity.com/download` or click [here](https://unity.com/download) and
  download `Unity Hub`.
* This will be the main area you launch your Unity Engine from.
* Once Unity Hub is installed, you'll want to make sure to download Unity
  Editor Version `2022.3.45f1` which is the version that we have developed
  ByteCity with
* After this is installed, you'll want to clone this repository
* Using https
```bash
git clone https://github.com/ethanrutt/siemens-game.git
```
* Using ssh
```bash
git clone git@github.com:ethanrutt/siemens-game.git
```
* Once this is complete, you'll want to 
* Open up Unity Hub
* Click `Add` in the top right
* Click `add project from disk`
* Find this repository that you just cloned
* Click add/open
* Finally, click on the new entry in the projects window of unity hub which
  should say `siemens-game`
* The first time you open up Unity, it will need to read all of the packages we
  have installed from the `Packages/manifest.json` and
  `Packages/package-lock.json`, as well as compile all of our scripts.
* Depending on the speed of your computer, this could take up to 10 minutes.
* Once this is finished, you should be ready to develop, build, and run
  ByteCity!

#### Backend Installation
* **naveed fix this part plz**

### Frontend Testing
* We have a combination of github actions, combined with
  [game-ci](https://game.ci/) to automatically test whenever we push code.
* Inside of Unity itself, there are edit mode tests and play mode tests
  possible in [Unity Test Runner](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/index.html)
* Both are essential to testing functionality
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

### Android Deployment
* There should already be Android Build Support installed from our packages,
  but if there isn't then you can follow these steps.
* Open Unity Hub
    * Click on `Installs` on the left
    * Find the Unity Version used with this project `2022.3.45f1`
    * Click the gear on the far right of the Unity Version
    * Click `Add modules`
    * Click `Android Build Support`
    * Click `Install`
* After you have installed Android Build Support, open up the Unity Editor by
  clicking on `siemens-game` in Unity Hub
* Go to File -> Build Settings
* Click `Android`
* Click `Switch Platform`
* Click `Player Settings` in the bottom left
* Make sure the `Company Name` is set to `Siemens`
* Make sure the `Product Name` is set to `Byte City`
* Set the version to whatever version you are releasing
* Click Build
* Once the project has successfully been built, you should have a `.apk` file
* This is how you will install the game onto an Android Device.
* To distribute this `.apk` file, you can upload it to a file sharing service,
  or use a usb to directly transfer it to a device. 
* Once it's on the device, simply running it should install the game as an app.
* You might have to scan the file before installing it. This is normal
* You also might have to allow third-party apps to be installed
    * For the most part, this will just pop up as a prompt whenever you install
      the `.apk`
    * There is also an option in settings app to allow third-party app
      installations

### iOS Deployment
* **rishi fix this plz**

### Future Development
* The ByteCity team has put tons of hours into this game. We were given nothing
  but an open-ended request with simple constraints, and we have built the
  ByteCity you can play today from scratch. 
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
      looks-wise
    * Wacky Wires leaves a lot to be desired, both game-mechanic-wise and
      looks-wise
    * Some of the dialogue can be a little clunky at times
    * The movement system can be a little clunky at times
