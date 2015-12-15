# padawan101
VR game using a hardware controller to enable players training sword fighting with a lightsaber.


##How to set up the game:

The following hardware and software is necessary:
- Oculus Rift DK2
- Oculus SDK 0.8 (newer could work but is untested)
- Microsoft Kinect One (newer, square version)
- Microsoft Kinect One SDK
- Computer with Windows 8
- Unity 5.2
- This github sourcecode (latest version in master)


##How to start the game client:

Run the Unity Editor and load the scene "main_FINAL.unity".
When the game is started on a PC the first time there will
show up some errors in the log. These should normally be
ignored since they should be gone after cash rebuild etc.

Run the Game in the Editor by pressing "Play".
Press "H" to define the game client as host of the game.
Press "R" to reset the game to zero points and to start again.
Press "A" to add more enemies if you want to have the game harder.


##How to start the game controller:

Open the UnityEngine_mobileSaber game from the repo and
built it onto an available and per USB connected Android Phone.
(iPhone built can work but iphone rotations are measured differently,
you have to press "I" then always when the game starts that the
game knows to use iPhone rotation values)
Start the App on the phone and connet to the client.
Both have to be in the same IP adress space.
For good performance a 5G Wifi Connection is recommended.

