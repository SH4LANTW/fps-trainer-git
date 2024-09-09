# FPS AIM Trainer Unity - Multiplayer
## 2024/9/9 readme updated
This is a Unity Project Game that has the feature of an FPS Aim Trainer, but in Multiplayer.

The Multiplayer function was created using Unity, including these multiplayer solution SDK:
- Unity Netcode for GameObject
- Unity Relay Service
- Parrel Sync (For development)

## How to open the Project
- First download the whole project by clicking "Code -> Download ZIP".
- Unzip the Project ZIP.
- In Unity, Add Project from Disk and select the project just unziped.

### Testing in Unity Transport Mode (only Local Connection):
- In Project, select the `NetworkManager` GameObject, into the Component `Unity Transport`. Change the Protocol Type to `Unity Transport`.
- Play Runtime, select the `NetworkManager` GameObject, into the Component `Network Manager`, as Host player click `Start Host`,as Client player click `Start Client`.

### Testing in Relay Unity Transport Mode (online Connection via JoinCode, prerequisite: need to link project to Unity Project ID first. In Project Settings -> Services -> Unity Project ID):
- In Project, select the `NetworkManager` GameObject, into the Component `Unity Transport`. Change the Protocol Type to `Relay Unity Transport`,and in the Component `Network Manager`, toggle `Try Relay in the Editor`.
1. Play Runtime, Host Player click Host Button and share Join Code to Clients who intent to join. Client enter Join Code in the input text bar, and click Client Join.
2. or Play Runtime, select the `NetworkManager` GameObject, into the Component `Network Manager`, as Host player click `Start Host`,as Client player enter Join Code and click `Start Client`.

