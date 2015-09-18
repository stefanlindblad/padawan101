using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovingText : MonoBehaviour {

	private Rigidbody rb;
	public float speed;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate ()
    {
        speed=5f;

        Vector3 movement = new Vector3 (0.0f, 1, 1);

        rb.AddForce(movement * speed);
    }
}
