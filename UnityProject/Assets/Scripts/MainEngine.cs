using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


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

    private State _gameState;
    private float _introTimePassed;

    private GameObject ls;
    private GameObject introText;
    private MyNetworkManager networkManager;

    public PlayerManager player;

    [Header("Cameras and rigs")]
    public GameObject introCam;
    public GameObject mainCam;
    public GameObject ovrCam;
    public GameObject specCam;

    public GameObject freeLookCameraRig;


    [Header("Flags")]
    public bool useOVR = false;

    /* Constants */
    private const float INTRO_LENGTH = 1.0f; // seconds

    public State GameState ()
    {
        return _gameState;
    }

    void Awake ()
    {
        introText = GameObject.Find ("IntroText");

        networkManager = GameObject.Find ("MyNetworkManager").GetComponent<MyNetworkManager> ();

        _gameState = State.NetworkSetup;
        OnStateEntering ();
    }

    private void SwitchCamera (GameObject camera)
    {
        if (!camera) {
            Debug.Log ("Camera not found. (" + camera + ")");
            return;
        }

        GameObject[] cameras = GameObject.FindGameObjectsWithTag ("MainCamera");
        foreach (GameObject cam in cameras) {
            cam.SetActive (false);
        }
        camera.SetActive (true);
    }


    private void ChangeState (State state)
    {
        Debug.Log ("Switching from " + StateName (GameState ()) + " to " + StateName (state) + ".");
        OnStateExiting ();
        _gameState = state;
        OnStateEntering ();

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

                // what todo while fighting.

                break;

            case State.Win:

                // What todo if the player wins.

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

    void NetworkSetupUpdate ()
    {
        if (Input.GetKey (KeyCode.H)) {
            networkManager.SetupHost ();
            ChangeState (State.Intro);
        } else if (Input.GetKey (KeyCode.C)) {
            networkManager.SetupClient ();
            ChangeState (State.Spectating);
        }
    }


    // Stuff done once if you enter a state
    void OnStateEntering ()
    {
        switch (GameState ()) {
            case State.Intro:
                this._introTimePassed = 0.0f;
                SwitchCamera (this.introCam);
                if (introText) {
                    introText.SetActive (true);
                    MovingText text = introText.GetComponent<MovingText> ();
                    text.Reset ();
                }

                break;

            case State.Fight:
                var playerObj = GameObject.FindWithTag("Player");
                this.player = playerObj.GetComponent<PlayerManager>();
                player.CmdSpawnObjects();
                break;

            case State.Win:

                // What todo if the player wins.

                break;

            case State.Loose:

                // What todo if the player looses.

                break;

            case State.NetworkSetup:
                Debug.Log ("Press H to start Host, press C to start Client.");
                break;
            case State.Spectating:
                freeLookCameraRig.SetActive (true);
                SwitchCamera (this.specCam);
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

                // what todo while fighting.
                break;

            case State.Win:

                // What todo if the player wins.

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
