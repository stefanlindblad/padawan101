using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;

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
    }
	;

	private State _gameState;
	private float _introTimePassed;
	private float _networkSetupTimePassed;

	private GameObject ls;
    
	private MyNetworkManager networkManager;

	private GameObject introText;
	private GameObject winText;
	private Text scoreText;
	private Text timeText;
	private Text highScoreText;


	// Fight state variables
	public float timeRemaining = 0f;
	public int score = 0;
	public int highScore = 0;

	private bool hasSpawnedObjects = false;


	public PlayerManager player;
	public GameObject winTextPrefab;

	[Header("Cameras and rigs")]
	public GameObject
		introCam;
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
	private const float INTRO_LENGTH = 7.0f; // seconds
	private const float NETWORK_SETUP_MAX_WAIT = 20.0f; // seconds
	private const int HIT_SCORE = 10;

	public State GameState ()
	{
		return _gameState;
	}

	void Awake ()
	{

		introText = GameObject.Find ("IntroText");
		winText = (GameObject)Instantiate (winTextPrefab, new Vector3 (0f, -1f, 33f), Quaternion.identity);
		winText.SetActive (false);

		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		highScoreText = GameObject.Find ("HighScore").GetComponent<Text> ();
		timeText = GameObject.Find ("TimeText").GetComponent<Text> ();

		networkManager = GameObject.Find ("MyNetworkManager").GetComponent<MyNetworkManager> ();

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

	void FightUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.R))
			ChangeState (State.Fight);

		if (Input.GetKeyDown (KeyCode.I))
			ChangeState (State.Intro);


		if (timeRemaining <= 0) {
			ChangeState (State.Win);
		} else {
			timeRemaining -= Time.deltaTime;
		}

		this.timeText.text = "Time left: " + String.Format ("{0:F2}", timeRemaining);
	}

	void WinUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.R))
			ChangeState (State.Fight);

		if (Input.GetKeyDown (KeyCode.I))
			ChangeState (State.Intro);
	}

	public void AddScore ()
	{
		score += MainEngine.HIT_SCORE;
		this.scoreText.text = "Score: " + score;
	}


	// Stuff done once if you enter a state
	void OnStateEntering ()
	{
		switch (GameState ()) {
		case State.Intro:
			this._introTimePassed = 0.0f;
			if (useOVR) {
				this.ovrCam.transform.position = introOVRCamPosition;
			} else {
				SwitchCamera (this.introCam);
			}
                
			if (introText) {
				introText.SetActive (true);
				MovingText text = introText.GetComponent<MovingText> ();
				text.Reset ();
			}

			break;

		case State.Fight:
			var playerObj = GameObject.FindWithTag ("Player");
			this.player = playerObj.GetComponent<PlayerManager> ();
			if (!hasSpawnedObjects) {
				player.CmdSpawnObjects ();
				hasSpawnedObjects = true;
			}
			if (useOVR) {
				this.ovrCam.transform.position = fightOVRCamPosition;
			}
			this.score = 0;
			this.timeRemaining = 10.0f;

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
		switch (GameState ()) {
		case State.Intro:

			this._introTimePassed += Time.fixedDeltaTime;
			if (this._introTimePassed > MainEngine.INTRO_LENGTH)
				ChangeState (State.Fight);

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
