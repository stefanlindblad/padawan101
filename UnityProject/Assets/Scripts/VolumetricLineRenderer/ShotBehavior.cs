using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShotBehavior : NetworkBehaviour
{
	public AudioSource laserhit;

	public Collider ls_collider;

	public MainEngine mainEngine;

	// Use this for initialization
	void Start ()
	{
		laserhit = GetComponent<AudioSource> ();
		
		var meobj = GameObject.Find ("__MainEngine");

		if (meobj)
			mainEngine = meobj.GetComponent<MainEngine> ();

	}

	// Update is called once per frame
	void Update ()
	{
		transform.position += transform.forward * Time.deltaTime * 100f;
        var ls = GameObject.Find("NetworkedLightSaber(Clone)");
        if (ls)
            ls_collider = ls.GetComponent<Collider>();
    }


	void OnTriggerEnter (Collider col)
	{
		if (ls_collider != null) {

		}
		if (col == ls_collider) {
			mainEngine.AddScore ();
			laserhit.Play ();
			// Simple bounce

			//Create the laser sword with every normal "forward" in local coordinates
			Vector3 colNormal = col.transform.forward;
			Vector3 laserDirection = this.transform.forward;
			this.transform.forward = Vector3.Reflect (laserDirection, colNormal);
		}
	}

	void OnCollisionEnter (Collision col)
	{
		Destroy (this.gameObject);
	}
}
