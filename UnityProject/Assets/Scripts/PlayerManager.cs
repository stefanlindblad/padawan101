using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerManager : NetworkBehaviour
{

	private bool _isLocalPlayer;
	private GameObject lightSaber;
	private GameObject enemyBall;


	private MyNetworkManager networkManager;

	[Header("Host prefabs")]
	public GameObject
		lightSaberPrefab;
	public GameObject enemyBallPrefab;

	void Start ()
	{
		networkManager = GameObject.Find ("MyNetworkManager").GetComponent<MyNetworkManager> ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.B)) {
			CmdToggleEnemyBall ();
		}
	}

	[Command]
	public void CmdToggleEnemyBall ()
	{
		this.enemyBall.SetActive (!this.enemyBall.activeSelf);
	}

	[Command]
	public void CmdSpawnObjects ()
	{
		this.lightSaber = (GameObject)Instantiate (lightSaberPrefab,
                                                   new Vector3 (0, 0, 0),
                                                   Quaternion.identity);

		this.enemyBall = (GameObject)Instantiate (enemyBallPrefab,
                                                  new Vector3 (0, 0, 0),
                                                  Quaternion.identity);
		this.enemyBall.SetActive (true);

		NetworkServer.Spawn (this.lightSaber);
		NetworkServer.Spawn (this.enemyBall);

	}

}
