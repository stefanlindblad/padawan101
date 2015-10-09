using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour
{
    public AudioSource laserhit;

    private Collider ls_collider;

    // Use this for initialization
    void Start ()
    {
        laserhit = GetComponent<AudioSource> ();
        var ls = GameObject.Find ("NetworkedLightSaber");
        if (ls)
            ls_collider = ls.GetComponent<Collider> ();
    }

    // Update is called once per frame
    void Update ()
    {
        transform.position += transform.forward * Time.deltaTime * 100f;
    }


    void OnTriggerEnter (Collider col)
    {
        if (ls_collider != null) {

        }
        if (col == ls_collider) {
            // Add score
            GameObject.Find ("ScoreText").GetComponent<ScoreTexter> ().AddScore (10);
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
