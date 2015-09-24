using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {
    public AudioSource laserhit;
	// Use this for initialization
	void Start () {
        laserhit = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * 100f;
	}
    void OnTriggerEnter(Collider col)
    {
        if (col == GameObject.Find("LightSaber").GetComponent<Collider>())
        {
            // Add score
            GameObject.Find("ScoreText").GetComponent<ScoreTexter>().AddScore(10);
            laserhit.Play();
            // Simple bounce
            Vector3 colNormal = col.transform.forward; //Create the laser sword with every normal "forward" in local coordinates
            Vector3 laserDirection = this.transform.forward;
            this.transform.forward = Vector3.Reflect(laserDirection, colNormal);
        }
    }

    void OnCollisionEnter (Collision col)
    {
        Destroy(this.gameObject);
    }
}
