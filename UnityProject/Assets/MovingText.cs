using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovingText : MonoBehaviour
{
	public float speed;
    public float maxTime;

    private float startTime;
    private MainEngine mainEngine;
    private Rigidbody rb;
    private bool introOver;
    private Vector3 initPosition = new Vector3(0.0f, 368.4f, 358.4f);


    void Awake ()
    {
        introOver = false;
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
        mainEngine = GameObject.Find("__MainEngine").GetComponent<MainEngine>();
    }

    void FixedUpdate ()
    {
        float running = Time.time - startTime;


        if(running < maxTime)
        {
            Vector3 movement = new Vector3(0.0f, 1, 1);
            rb.AddForce(movement * speed);
        }
        else if(!introOver)
        {
            mainEngine.IntroIsOver();
            introOver = true;
        }
    }

    public void Reset()
    {
        startTime = Time.time;
        introOver = false;
        rb.velocity = new Vector3(0, 0, 0);
        this.gameObject.transform.position = initPosition;
    }
}
