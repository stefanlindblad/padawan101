using UnityEngine;
using System.Collections;

public class MoveOnSphere : MonoBehaviour
{
	Vector3 nextPosition;
	private float timeSinceLastMove = 0.0f;

	// Grid size
	private const int XMAX = 10;
	private const int YMAX = 10;

	// Sphere constraints
	private const float radius = 1.5f;

	Vector3[][] spherePoints;


	void Start ()
	{
		spherePoints = new Vector3[XMAX][];

		Random.seed = 42;

		for (int i=0; i<YMAX; ++i) {
			spherePoints [i] = new Vector3[YMAX];
			for (int j=0; j<XMAX; j++) {
				spherePoints [i] [j] = randomSpherePosition ();
			}
		}
			
	}


	private Vector3 randomSpherePosition ()
	{
		float phi = Random.Range (Mathf.PI + (Mathf.PI / 4), (2.0f * Mathf.PI) - Mathf.PI / 4.0f);
		float theta = Random.Range (2.0f * Mathf.PI / 3.0f, 4.0f * Mathf.PI / 5.0f);
		float y = Mathf.Sin (theta);
		float x = Mathf.Cos (phi);
		float z = Mathf.Cos (theta) + Mathf.Sin (phi);
		return radius * (new Vector3 (x, y, z));
	}

	private void setNextTargetPosition ()
	{
		int i = Random.Range (0, XMAX);
		int j = Random.Range (0, YMAX);
		nextPosition = spherePoints [i] [j];
	}

	void Update ()
	{
		timeSinceLastMove += Time.deltaTime;

		if (timeSinceLastMove > 1.0f) {
			transform.position = nextPosition;
			setNextTargetPosition ();
		
			timeSinceLastMove = 0.0f;
		}
	} 
}