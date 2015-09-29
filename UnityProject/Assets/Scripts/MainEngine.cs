using UnityEngine;
using System.Collections;

public class MainEngine : MonoBehaviour {



    public enum State
    {
        Intro,
        Fight,
        Loose,
        Win
    };

    private State _gameState;
    private int _points;
    private int _raysDefended;
    private int _life;
    private GameObject enemyBall;
    private GameObject introText;
    private GameObject introCam;
    private GameObject mainCam;
    private GameObject ovrCam;

    public bool useOVR = false;

    public State GameState()
    {
        return _gameState;
    }

    void Awake ()
    {
        enemyBall = GameObject.Find("EnemyBall");
        introText = GameObject.Find("IntroText");
        introCam = GameObject.Find("IntroCamera");
        mainCam = GameObject.Find("MainCamera_Normal");
        ovrCam = GameObject.Find("MainCamera_OVR");
        ResetGame();
        _gameState = State.Intro;
        OnStateEntering();

	}

    private void SwitchCamera(string name)
    {
        /*if(name == "Intro")
        {
            if (introCam)
                introCam.SetActive(true);
            if (mainCam)
                mainCam.SetActive(false);
            if (ovrCam)
                ovrCam.SetActive(false);
        }
        else if (name == "Main")
        {
            if (introCam)
                introCam.SetActive(false);
            if (mainCam)
                mainCam.SetActive(true);
            if (ovrCam)
                ovrCam.SetActive(false);
        }
        else*/ if (name == "OVR")
        {
            if (introCam)
                introCam.SetActive(false);
            if (mainCam)
                mainCam.SetActive(false);
            if (ovrCam)
                ovrCam.SetActive(true);
        }
    }


    private void SwitchBall(bool onOff)
    {
        if (enemyBall)
            enemyBall.SetActive(onOff);
    }


    private void ChangeState(State state)
    {
        OnStateExiting();
        _gameState = state;
        OnStateEntering();

    }

    // Stuff done every update step in a state
    void OnStateRunning()
    {
        switch (GameState())
        {
            case State.Intro:

                // What todo as intro;
                // choose intro camera as main camera...

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
        }
    }

    // Stuff done once if you enter a state
    void OnStateEntering()
    {
        switch (GameState())
        {
            case State.Intro:

                SwitchCamera("Intro");
                SwitchBall(false);
                if (introText)
                {
                    introText.SetActive(true);
                    MovingText text = introText.GetComponent<MovingText>();
                    text.Reset();
                }

                break;

            case State.Fight:

                SwitchBall(true);
                Debug.Log("Got into Fight State");

                break;

            case State.Win:

                // What todo if the player wins.

                break;

            case State.Loose:

                // What todo if the player looses.

                break;
        }
    }

    // Stuff done once if you exit a state
    void OnStateExiting()
    {
        switch (GameState())
        {
            case State.Intro:

                if (introText)
                    introText.SetActive(false);
                if (useOVR)
                    SwitchCamera("OVR");
                else
                    SwitchCamera("Main");

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
        }
    }

    public void IntroIsOver()
    {

        Debug.Log("Intro is over.");
        this.ChangeState(State.Fight);

    }

    void Update ()
    {
        OnStateRunning();
        if (Input.GetKeyDown(KeyCode.R))
            ResetGame();
    }

    public int GetLife()
    {
        return _life;
    }

    public int GetPoints()
    {
        return _points;
    }

    public int GetRaysDefended()
    {
        return _raysDefended;
    }

    public void HitIncoming()
    {

    }

    public void GetHit(int impact = 1)
    {
        _life = _life - impact;
    }

    public void ScorePoint(int points)
    {
        _points = _points + points;
    }

    public void DefendedRay()
    {
        _raysDefended++;
    }

    private void ResetGame()
    {
        GameObject.Find("ScoreText").GetComponent<ScoreTexter>().ResetScore();
        _points = 0;
        _raysDefended = 0;
        _life = 20;
        SwitchBall(false);
        this.ChangeState(State.Intro);    

    }
}
