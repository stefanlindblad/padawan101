using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class TextFadeInOut : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float DisplayTime = 2;
    float elapsedTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > DisplayTime)
        {
            speed = 99999999999999999999f;

            Vector3 movement = new Vector3(0.0f, 1, 1);

            rb.AddForce(movement * speed);
        }
    }
}
