using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTexter : MonoBehaviour {

    private Text ScoreText;
    public int score;
    private Vector3 winPosition = new Vector3(-51.0f, 0.0f, 46.9f);
    private Vector3 resetPosition = new Vector3(-51.0f, 1000.0f, 46.9f);

    // Use this for initialization
    void Start () {
        score=0;

        ScoreText = GetComponent<Text>();
        ScoreText.text = "Score: 0";

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ResetScore();
        //int n=0;
        //while (++n<10)
        //{
        //  AddScore(10);
        //}
        if (score > 295)
            Win();
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
    }

    // Update is called once per frame
    void UpdateScore () {
        ScoreText.text="Score: "+score;
    }
    public void Win()
    {
        GameObject.Find("WinText").transform.position=winPosition;
    }
}
