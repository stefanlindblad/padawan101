using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerManager : NetworkBehaviour {

    private bool _isLocalPlayer;
    private GameObject lightSaber;
    private GameObject enemyBall;

    [SyncVar]
    private int _points;
    [SyncVar]
    private int _raysDefended;
    [SyncVar]
    private int _life;

    private MyNetworkManager networkManager;

    [Header("Host prefabs")]
    public GameObject lightSaberPrefab;
    public GameObject enemyBallPrefab;

    void Start()
    {
        networkManager = GameObject.Find ("MyNetworkManager").GetComponent<MyNetworkManager> ();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown (KeyCode.R))
            ResetGame ();
        if (Input.GetKeyDown (KeyCode.B)) {
            CmdToggleEnemyBall();
        }
    }

    [Command]
    public void CmdToggleEnemyBall()
    {
        this.enemyBall.SetActive (!this.enemyBall.activeSelf);
    }

    [Command]
    public void CmdSpawnObjects()
    {
        this.lightSaber = (GameObject)Instantiate (lightSaberPrefab,
                                                   new Vector3 (0, 0, 0),
                                                   Quaternion.identity);

        this.enemyBall = (GameObject)Instantiate (enemyBallPrefab,
                                                  new Vector3 (0, 0, 0),
                                                  Quaternion.identity);
        this.enemyBall.SetActive(true);

        NetworkServer.Spawn(this.lightSaber);
        NetworkServer.Spawn(this.enemyBall);

    }

    public int GetLife ()
    {
        return _life;
    }

    public int GetPoints ()
    {
        return _points;
    }

    public int GetRaysDefended ()
    {
        return _raysDefended;
    }

    public void GetHit (int impact = 1)
    {
        _life = _life - impact;
    }

    public void ScorePoint (int points)
    {
        _points = _points + points;
    }

    public void DefendedRay ()
    {
        _raysDefended++;
    }


    private bool hasMadeAJoke = false;

    private void ResetGame ()
    {
        try {
            GameObject.Find ("ScoreText").GetComponent<ScoreTexter> ().ResetScore ();
        } catch (NullReferenceException e) {
            if (!hasMadeAJoke) {
                Debug.Log ("I will be fixed someday and then everything will be alright. " +
                           "Pigs will have learnt to fly and hell has since long frozen over.");
                hasMadeAJoke = true;
            }
        }
        _points = 0;
        _raysDefended = 0;
        _life = 20;
    }

}
