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
    private Vector3 initPosition = new Vector3(0.0f, 380.0f, 140.0f);//working values Y=368.4f & Z=157.9f


    void Awake ()
    {
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
    {
        float running = Time.time - startTime;


        if(running < maxTime)
        {
            Vector3 movement = new Vector3(0.0f, 0.46f, 0.7f); //if angle is 60 then (0 , 0.5 , 0.87) if angle is 50 then (0 , 0.46 , 1)
            rb.AddForce(movement * speed);
        }
    }

    public void Reset()
    {
        startTime = Time.time;
        rb.velocity = new Vector3(0, 0, 0);
        this.gameObject.transform.position = initPosition;
    }

    //    Here we should put some
    //    Text for Instruction what
    //the Player should do and
    //then go on with the game.

//    The galaxy   is   at war!
//We need all the help we
//can get.That  is  why you
//have to  learn the ways of
//the force at this early age.

//Focus on   blocking the
//shots rather  than hitting
//anything.We really need
//all  the help  we can  get.
}
