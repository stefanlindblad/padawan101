using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {
    public AudioSource laserhit;

    private Collider ls_collider;
    private MainEngine mainEngine;

    void Awake()
    {
    //    mainEngine = GameObject.Find("__MainEngine").GetComponent<MainEngine>();
    }

    // Use this for initialization
    void Start ()
    {
        laserhit = GetComponent<AudioSource>();
        ls_collider = GameObject.Find("LightSaber").GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		transform.position += transform.forward * Time.deltaTime * 100f;
	}


    void OnTriggerEnter(Collider col)
    {
        if(ls_collider != null)
        {

        }
        if (col == ls_collider)
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
