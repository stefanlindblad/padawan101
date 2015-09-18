using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	// VARIABLES
	private float x;
	private float y;
	private float zoom;
	private float x2;
	private float y2;
	private float zoom2;
	private float maxSpeed = 0.25f;
	public Vector3 origin = new Vector3 (0, 0, 0);

	// Use this for initialization
	void Start () {
		x = 0;
		y = 0;
		zoom = 0;
		x2 = 0;
		y2 = 0;
		zoom2 = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//bool UIB = this.GetComponent<InGameMenu> ().UIButtons;

		// For keyboard
		// Get user input with acceleration and deacceleration
		if (Input.GetAxis ("Horizontal") == 0) {
			if (x > 0.0f) {x -= 0.05f;} 
			else if (x < 0.0f){x += 0.05f;}
			if (x < 0.01f && x > -0.01f){x=0.0f;}
		} else {
			x += Input.GetAxis ("Horizontal") * 0.02f;
		}

		if (Input.GetAxis ("Vertical") == 0) {
			if (y < 0.02f && y > -0.02f){y=0.0f;}
			if (y > 0.0f) {y -= 0.02f;} 
			else if (y < 0.0f){y += 0.02f;}

		} else {
			y += Input.GetAxis ("Vertical") * 0.02f;
		}

		// Zoom (+&-)
		if (Input.GetKey (KeyCode.KeypadPlus)) {
			zoom = 0.2f;
		} else if (Input.GetKey (KeyCode.KeypadMinus)) {
			zoom = -0.2f;
		} else {
			zoom = 0.0f;
		}


		x = Mathf.Max (x, -maxSpeed);
		x = Mathf.Min (x, maxSpeed);
		y = Mathf.Max (y, -maxSpeed);
		y = Mathf.Min (y, maxSpeed);

		float angle = -x*2;

		MoveCamera (true, 0, y, angle, zoom);

	}

	//public Vector2 GetCurrentXY(){
	//	return new Vector2 (x, y);
	//}

	public void ChangeCamera (Vector2 XYZ){
		x2 += XYZ.x * 0.02f;
		y2 += XYZ.y * 0.02f;
		zoom2 += XYZ.x * 0.00f;
		
		x2 = Mathf.Max (x2, -maxSpeed);
		x2 = Mathf.Min (x2, maxSpeed);
		y2 = Mathf.Max (y2, -maxSpeed);
		y2 = Mathf.Min (y2, maxSpeed);
		
		float angle = -x2*2;
		
		MoveCamera (false, 0, y2, angle, zoom2);
	}
	private void MoveCamera(bool keys, float xlen, float ylen, float angle, float z) {


		//bool UIB = this.GetComponent<InGameMenu> ().UIButtons;



		// APPLY TRANSFORM
		transform.position += new Vector3 (xlen,ylen,0.0f);
		transform.position = Vector3.MoveTowards (transform.position, origin, z);
		origin.y = transform.position.y;
		transform.RotateAround (origin, Vector3.up, angle);
	}
}


