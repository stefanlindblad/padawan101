using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.VR;

public class MainEngine : MonoBehaviour
{


	public enum State
	{
		Intro,
		Fight,
		Loose,
		Win,
		NetworkSetup,
		Spectating
    };

    public enum Device
	{
		iPhone,
		Android
    };

	private State _gameState;
	private Device _usedDevice;
	private float _introTimePassed;
	private float _networkSetupTimePassed;

	private GameObject ls;
    private bool startFading = false;
    private bool fadeOutStarted = false;
    private bool introAlmostDone = false;
    private bool introIsDone = false;
    public float fadeTime = 0.5f;

    private MyNetworkManager networkManager;

    private FadeObjectInOut fadingText;
    private GameObject introBackground;
    private GameObject longIntro;
    private GameObject introText;
	private GameObject winText;
    private RotationServer rotServer;
	private Text scoreText;
	private Text timeText;
	private Text highScoreText;
    public GameObject[] enemies;
    // Fight state variables
    public float timeRemaining = 0f;
	public int score = 0;
	public int highScore = 0;
    private int ballsSpawned = 0;

	private bool hasSpawnedObjects = false;


	public PlayerManager player;
	public GameObject winTextPrefab;

	[Header("Cameras and rigs")]
	public GameObject introCam;
	public GameObject mainCam;
	public GameObject ovrCam;
	public GameObject specCam;
	private GameObject currentCam;
	private Vector3 introOVRCamPosition = new Vector3 (4.289584f, 413.0703f, 109.5418f);
	//rot X=15.72058
	private Vector3 fightOVRCamPosition = new Vector3 (0.0f, 0.0f, 0.0f);


	public GameObject freeLookCameraRig;


	[Header("Flags")]
	public bool
		useOVR = false;

	/* Constants */
	private const float INTRO_LENGTH = 40.0f; // seconds
	private const float NETWORK_SETUP_MAX_WAIT = 2000.0f; // seconds
	private const int HIT_SCORE = 10;

	public State GameState ()
	{
		return _gameState;
	}

	public Device UsedDevice ()
	{
		return _usedDevice;
	}

	void Awake ()
	{
        longIntro = GameObject.Find("LongIntro");
        fadingText = longIntro.GetComponent<FadeObjectInOut>();
        introBackground = GameObject.Find("IntroBackground");
        introBackground.SetActive(false);
		introText = GameObject.Find ("IntroText");
        introText.SetActive(false);
		winText = (GameObject)Instantiate (winTextPrefab, new Vector3 (0f, -1f, 33f), Quaternion.identity);
		winText.SetActive (false);
        scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		highScoreText = GameObject.Find ("HighScore").GetComponent<Text> ();
		timeText = GameObject.Find ("TimeText").GetComponent<Text> ();
        rotServer= GameObject.Find("RotationServer").GetComponent<RotationServer>();
        networkManager = GameObject.Find ("MyNetworkManager").GetComponent<MyNetworkManager> ();
        _usedDevice = Device.Android;

		_gameState = State.NetworkSetup;
		OnStateEntering ();
	}

	private void SwitchCamera (GameObject camera)
	{
		if (camera == currentCam) {
			return;
		}
		if (!camera) {
			Debug.Log ("Camera not found. (" + camera + ")");
			return;
		}

		GameObject[] cameras = GameObject.FindGameObjectsWithTag ("MainCamera");
		foreach (GameObject cam in cameras) {
			cam.SetActive (false);
		}
		camera.SetActive (true);
		currentCam = camera;
	}


	private void ChangeState (State state)
	{
		Debug.Log ("Switching from " + StateName (GameState ()) + " to " + StateName (state) + ".");
		OnStateExiting ();
		_gameState = state;
		OnStateEntering ();

	}

	void NetworkSetupUpdate ()
	{
		this._networkSetupTimePassed += Time.fixedDeltaTime;
		if (Input.GetKey (KeyCode.O)) {
			useOVR = true;
		}

		if (Input.GetKey (KeyCode.H)) {
			networkManager.SetupHost ();
			ChangeState (State.Intro);
		} else if (Input.GetKey (KeyCode.C) ||
			this._networkSetupTimePassed >
			MainEngine.NETWORK_SETUP_MAX_WAIT) {
			networkManager.SetupClient ();
			ChangeState (State.Spectating);
		}
	}
    void introUpdate()
    {
        this._introTimePassed += Time.fixedDeltaTime;
        if (this._introTimePassed > MainEngine.INTRO_LENGTH)
        {
            ChangeState(State.Fight);
        }
        else
        {
            if (startFading == false)
            {
                fadingText.FadeIn();
                startFading = true;
            }
            if (this._introTimePassed > 5.0f && fadeOutStarted == false)
            {
                introBackground.SetActive(false);
                fadingText.FadeOut();
                fadeOutStarted = true;
            }
            //else if (fadeOutStarted == true) { Debug.Log("Not anymore"); }
            if (this._introTimePassed > 4.0f && introIsDone == false)
            {
                introText.SetActive(true);
                MovingText text = introText.GetComponent<MovingText>();
                text.Reset();
                introIsDone = true;
            }
        }
    }
	void FightUpdate ()
	{
        if (Input.GetKeyDown (KeyCode.I))
			ChangeState (State.Intro);
        if (Input.GetKeyDown(KeyCode.A)) {
            spawnFirstBall();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            player.CmdKillAllBalls();
        }
        if (timeRemaining < 40 && ballsSpawned == 1) {
            player.CmdSpawnBalls();
            ballsSpawned = 2;
        }
        if (timeRemaining < 20 && ballsSpawned == 2) {
            player.CmdSpawnBalls();
            ballsSpawned = 3;
        }
		if (timeRemaining <= 0) {
            this.timeText.text = "Time left: 0.00";
			ChangeState (State.Win);
		} else {
			timeRemaining -= Time.deltaTime;
		}
        player.setLightsaberRotation(rotServer.GetRotation());
        //Debug.Log("Acceleration:" + rotServer.GetAcceleration());
        player.playAccelerationSound(rotServer.GetAcceleration());

		this.timeText.text = "Time left: " + String.Format ("{0:F2}", timeRemaining);
	}

	void WinUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.I))
			ChangeState (State.Intro);
	}

	public void AddScore ()
	{
		score += MainEngine.HIT_SCORE;
        if (score > highScore) {
            highScore = score;
            this.highScoreText.text = "Highscore: " + highScore;
        }
        
		this.scoreText.text = "Score: " + score;
	}
    void spawnFirstBall()
    {
        player.CmdSpawnBalls();
    }

	// Stuff done once if you enter a state
	void OnStateEntering ()
	{
		switch (GameState ()) {
		case State.Intro:
                player.CmdKillAllBalls();
                introBackground.SetActive(true);
                this._introTimePassed = 0.0f;
                if (useOVR)
                {
                    this.ovrCam.transform.position = introOVRCamPosition;
                }
                else
                {
                    SwitchCamera(this.introCam);
                }
                if (startFading == true)
                    startFading = false;
                if (fadeOutStarted == true)
                    fadeOutStarted = false;
                if (introAlmostDone == true)
                    introAlmostDone = false;
                if (introIsDone == true)
                    introIsDone = false;

                //if (introText) {
                //	introText.SetActive (true);
                //	MovingText text = introText.GetComponent<MovingText> ();
                //	text.Reset ();
                //}
                //Debug.Log("EnterIntro");
                
			break;

		case State.Fight:
                
                var playerObj = GameObject.FindWithTag ("Player");
			this.player = playerObj.GetComponent<PlayerManager> ();
			if (!hasSpawnedObjects) {
				player.CmdSpawnObjects ();
				hasSpawnedObjects = true;
            } /*else {
                player.CmdSpawnBalls();
            }*/
                player.CmdKillAllBalls();
                ballsSpawned = 0;
                player.CmdSpawnBalls();
                ballsSpawned = 1;
                if (useOVR) {
				this.ovrCam.transform.position = fightOVRCamPosition;
			}
			this.score = 0;
			this.timeRemaining = 60.0f;

			this.scoreText.text = "Score: 0";            
			this.timeText.text = "Time left: 10";
			this.highScoreText.text = "HighScore: " + this.highScore;
			if (useOVR) {
				SwitchCamera (this.ovrCam);
			} else {
				SwitchCamera (this.mainCam);
			}

			break;

		case State.Win:
                player.CmdKillAllBalls();
                this.winText.SetActive (true);
			break;

		case State.Loose:

                // What todo if the player looses.

			break;

		case State.NetworkSetup:
			this._networkSetupTimePassed = 0.0f;
			Debug.Log ("Press H to start Host, press C (or wait) to start Client.");
			break;
		case State.Spectating:
			freeLookCameraRig.SetActive (true);
			SwitchCamera (this.specCam);
			break;
		}
	}

	// Stuff done every update step in a state
	void OnStateRunning ()
	{

		if (Input.GetKeyDown (KeyCode.U))
		{
			_usedDevice = Device.iPhone;
			Debug.Log("Switched to iPhone Rotation");
		}

		if (Input.GetKeyDown (KeyCode.S))
		{
			ChangeState(State.Fight);
		}

		if (Input.GetKeyDown (KeyCode.R))
		{
			InputTracking.Recenter();
		}

		if (Input.GetKey (KeyCode.N))
		{
			rotServer.ChangeCalibration(-1);
		}

		if (Input.GetKey (KeyCode.M))
		{
			rotServer.ChangeCalibration(1);
		}


		switch (GameState ()) {
		case State.Intro:

                //this._introTimePassed += Time.fixedDeltaTime;
                //if (this._introTimePassed > MainEngine.INTRO_LENGTH)
                //	ChangeState (State.Fight);
                introUpdate();
                break;

		case State.Fight:
			FightUpdate ();
			break;

		case State.Win:
			WinUpdate ();               
			break;

		case State.Loose:

                // What todo if the player looses.

			break;

		case State.NetworkSetup:
			NetworkSetupUpdate ();
			break;

		case State.Spectating:
			break;
		}
	}

	// Stuff done once if you exit a state
	void OnStateExiting ()
	{
		switch (GameState ()) {
		case State.Intro:

			if (introText)
				introText.SetActive (false);
			if (useOVR)
				SwitchCamera (this.ovrCam);
			else
				SwitchCamera (this.mainCam);

			break;

		case State.Fight:
			break;

		case State.Win:
			this.winText.SetActive (false);
			break;

		case State.Loose:

                // What todo if the player looses.

			break;

		case State.NetworkSetup:
			break;

		case State.Spectating:
			freeLookCameraRig.SetActive (false);
			break;
		}
	}

	private string StateName (State s)
	{
		switch (s) {
		case State.Intro:
			return "Intro";

		case State.Fight:
			return "Fight";

		case State.Win:
			return "Win";

		case State.Loose:
			return "Loose";

		case State.NetworkSetup:
			return "NetworkSetup";

		case State.Spectating:
			return "Spectating";

		}
		return "";
	}

	private void LogState ()
	{
		Debug.Log (StateName (GameState ()));
	}

	void Update ()
	{
		OnStateRunning ();
	}

}
