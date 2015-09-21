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

    public State GameState()
    {
        return _gameState;
    }

    void Awake ()
    {
        ResetGame();
        _gameState = State.Intro;
        OnStateEntering();
	}
	


    void SwitchBall(bool onOff)
    {
        //Todo what it possible to turn the ball on or off
        // off should be that he is either not visible or remaining position
        // on should mean he is moving and shooting
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

                // What todo as intro;

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

    // Stuff done once if you exit a state
    void OnStateExiting()
    {
        switch (GameState())
        {
            case State.Intro:

                // What todo as intro;

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

    void Update ()
    {
        OnStateRunning();
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
        _points = 0;
        _raysDefended = 0;
        _life = 20;
    }
}
