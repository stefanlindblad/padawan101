using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]

public class MoveOnSphere : MonoBehaviour
{
	// Variables
	public float smoothTime = 0.3f;
	public Transform camera;
	private Vector3 velocity = Vector3.zero;

	Vector3 nextPosition;
	private float timeSinceLastMove = 0.0f;

	// Line renderer
	LineRenderer lineRenderer;


	// Grid size
	private const int XMAX = 10;
	private const int YMAX = 10;

	// Sphere constraints
	private const float radius = 1.5f;

	Vector3[][] spherePoints;


	void Start ()
	{
		// Prepare AI for the ball
		spherePoints = new Vector3[XMAX][];

		Random.seed = 42;

		for (int i=0; i<YMAX; ++i) {
			spherePoints [i] = new Vector3[YMAX];
			for (int j=0; j<XMAX; j++) {
				spherePoints [i] [j] = randomSpherePosition ();
			}
		}
		setNextTargetPosition ();


		// Prepare laser ray
		lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.SetWidth (1.0f,1.0f);
		Debug.Log (lineRenderer);
	}


	private Vector3 randomSpherePosition ()
	{
		float phi = Random.Range (Mathf.PI + (Mathf.PI / 4), (2.0f * Mathf.PI) - Mathf.PI / 4.0f);// + Mathf.PI;
		float theta = Random.Range (2.0f * Mathf.PI / 3.0f, 4.0f * Mathf.PI / 5.0f);
		float y = Mathf.Sin (theta);
		float x = Mathf.Cos (phi);
		float z = -(Mathf.Cos (theta) + Mathf.Sin (phi));
		return radius * (new Vector3 (x, y, z));
	}




	// ---------------------------------------------------------------------//



	private void setNextTargetPosition ()
	{
		int i = Random.Range (0, XMAX);
		int j = Random.Range (0, YMAX);
		nextPosition = spherePoints [i] [j];
	}

	private void shootLaserRay()
	{

	}

	void Update ()
	{
		transform.position = Vector3.SmoothDamp(transform.position, nextPosition, ref velocity, smoothTime);

		timeSinceLastMove += Time.deltaTime;
		if (transform.position == nextPosition) {
			shootLaserRay();
			setNextTargetPosition ();
		}
	} 
}