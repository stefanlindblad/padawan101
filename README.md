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
Press "S" to reset the game to zero points and to start again.
Press "A" to add more enemies if you want to have the game harder.
Press "K" to kill all enemies.
Press "I" to start the intro again if you wanna use it.


##How to start the game controller:

Open the UnityEngine_mobileSaber game from the repo and
built it onto an available and per USB connected Android Phone.
(iPhone built can work but iphone rotations are measured differently,
you have to press "U" then always when the game starts that the
game knows to use iPhone rotation values)
Start the App on the phone and connect to the client.
Both have to be in the same IP adress space.
For good performance a 5G Wifi Connection is recommended.


##How to calibrate the game controller:

If the rotation of the phone is not correct, you have to calibrate
it. Hold the phone correctly upward (try different ways to hold it,
there is only one version correct) and press "M" or "N" to rotate
the rotation init value until the position of the phone matches
with the position of the virtual lightsaber. Enjoy!


##Common problems:

Use different USB busses for Kinect and Oculus and USB3 for Kinect.
Certain computing power is necessary for all the hardware input/output.
We recommend a 2015 state of the art gaming computer.
Do not use Unity4 or the old Unity Plugin Solution. The latest game
is built to run with the Unity5 builtin Oculus Support.