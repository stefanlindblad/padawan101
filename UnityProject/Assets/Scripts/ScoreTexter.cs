using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTexter : MonoBehaviour {

	private Text ScoreText;
	public int score;

	// Use this for initialization
	void Start () {
	score=0;

	ScoreText = GetComponent<Text>();
        ScoreText.text = "Score: 0";

	}
	void Update()
	{
		//int n=0;
		//while (++n<10)
		//{
		//	AddScore(10);
		//}
	}
	public void AddScore(int newScoreValue)
	{
	score+= newScoreValue;
	UpdateScore();
	}
	// Update is called once per frame
	void UpdateScore () {
	ScoreText.text="Score: "+score;
	}
}
