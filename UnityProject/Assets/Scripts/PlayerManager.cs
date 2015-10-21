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
    public GameObject shotPrefab;

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
                                                  new Vector3 (-15f, 6f, 65f),
                                                  Quaternion.identity);
		this.enemyBall.SetActive (true);

		NetworkServer.Spawn (this.lightSaber);
		NetworkServer.Spawn (this.enemyBall);

	}
    [Command]
    public void CmdSpawnShot(Vector3 direction, Quaternion rotation)
    {
        var ls_collider = this.lightSaber.GetComponentInChildren<Collider>();
        GameObject go = GameObject.Instantiate(this.shotPrefab, direction, rotation) as GameObject;
        var sb = go.GetComponent<ShotBehavior>();
        sb.ls_collider = ls_collider;
        NetworkServer.Spawn(go);
        GameObject.Destroy(go, 5f);
    }
    public void setLightsaberRotation(Quaternion rotation) {
        if (this.lightSaber) {
            this.lightSaber.transform.rotation = rotation;
            Debug.Log("Lightsaber rotation"+rotation);
        }
    }
    [Command]
    public void CmdSpawnBalls()
    {
        this.enemyBall = (GameObject)Instantiate(enemyBallPrefab,
                                                  new Vector3(-15f, 6f, 65f),
                                                  Quaternion.identity);
        this.enemyBall.SetActive(true);
        
        NetworkServer.Spawn(this.enemyBall);
    }
    [Command]
    public void CmdKillAllBalls(){
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            GameObject.Destroy(enemy);
        }
    }
}
