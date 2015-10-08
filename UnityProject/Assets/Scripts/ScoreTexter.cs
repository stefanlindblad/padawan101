using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTexter : MonoBehaviour {
    public float timeRemaining;
	private Text ScoreText;
    private Text TimeText;
    private Text HighScore;
	public int score;
    public int highScore;
    public int intTimeRemaining;
    private Vector3 winPosition = new Vector3(-51.0f, 0.0f, 46.9f);
    private Vector3 resetPosition = new Vector3(-51.0f, 1000.0f, 46.9f);

    // Use this for initialization
    void Start () {
	    score = 0;
        highScore = 0;
        timeRemaining = 10;
	    ScoreText = GetComponent<Text>();
        ScoreText.text = "Score: 0";
        TimeText = GameObject.Find("TimeText").GetComponent<Text>();
        TimeText.text = "Time left: 10";
        HighScore = GameObject.Find("HighScore").GetComponent<Text>();
        HighScore.text = "High score: 0";
	}
	void Update()
	{
        if (Input.GetKeyDown(KeyCode.R))
            ResetScore();
        //int n=0;
        //while (++n<10)
        //{
        //	AddScore(10);
        //}
        //if (score > 295)
        intTimeRemaining = System.Convert.ToInt16(timeRemaining);
        if(intTimeRemaining==0)
            Win();
        if (timeRemaining > 0)
            timeRemaining -= Time.deltaTime;
        TimeText.text = "Time left: " + intTimeRemaining;
        //TimeText.text = "Time left: " + timeRemaining;
    }
	public void AddScore(int newScoreValue)
	{
	score+= newScoreValue;
	UpdateScore();
	}
    public void ResetScore()
    {
        GameObject.Find("WinText").transform.position = resetPosition;
        score = 0;
        ScoreText.text = "Score: 0";
        timeRemaining = 10;
        HighScore.text = "High score: " + highScore;
    }

	// Update is called once per frame
	void UpdateScore () {
	ScoreText.text="Score: "+score;
    }
    public void Win()
    {
        if (score > highScore)
            highScore = score;
        HighScore.text = "High score: " + highScore;
        GameObject.Find("WinText").transform.position=winPosition;

    }
}
