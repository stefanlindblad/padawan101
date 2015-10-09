Networking 101
==============

The NetworkManager GameObject is responsible for activating a
MainEngine on every Client. This by default sets the game in the
NetworkSetup State.

Here you need to press either H for starting a host (the computer with
Oculus + LeapMotion), or C for starting a client. The IP-adress + port
can be set on the NetworkManager.


Host
----

On the host, after having started a Server and a LocalClient, the
MainEngine kicks us into the Intro State. This switches to the
IntroCam and displays for some time. The objects of the Intro state
can be partly manipulated by their own scrips, but also in MainEngine.

*TODO* - Add a skip intro button (press S?).

After the intro is done, we enter the Fight State where the actual
fighting goes on. Pressing R to reset resets the score. Pressing B
toggles the EnemyBall on and off.


Client
------

On the client, the MainEngine switches to the Spectating State, where
the MainCamera is replaced with a FreeLookCameraRig. We might need to
change this as we progress, but should work for now.


Prefabs & Scripts
-----------------

*VERY MUCH IN PROGRESS*

Some of the prefabs and scripts have been upgraded with some network
stuff in order to send the correct data over the wire. I have moved
some of them to NetworkedPrefabs.
