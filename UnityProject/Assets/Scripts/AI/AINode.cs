using UnityEngine;
using System.Collections;

public class AINode : MonoBehaviour
{
	private float shootAnimationTime;
	private float shootAnimationStart;
	private bool isShooting;
	private bool isMoving;
	private float startTime;
	private float mySpeed;
	private float travelDistance;
	private bool hastoShoot;
	private Vector3 destination;
	private Vector3 startPosition;

	void Start()
	{
		GameObject AIobj = GameObject.Find("AIFactory");
		if(AIobj != null)
		{
			AIFactory factory = AIobj.GetComponent<AIFactory>();
			factory.Register(this);
		}
		else
		{
			Debug.Log("Couldn't register AINode at AIFactory. Create a empty Object called 'AIFactory' and add the script.");
		}

		isMoving = false;
		isShooting = false;
		mySpeed = 0f;
		shootAnimationTime = 0f;
	}

	void Update()
	{
		if(isMoving)
		{
			float movedTime = (Time.time - startTime) * mySpeed;
        	float movedPart = movedTime / travelDistance;
        	transform.position = Vector3.Lerp(startPosition, destination, movedPart);
        	
        	if(transform.position == destination)
        	{
        		isMoving = false;
        		isShooting = true;
        		hastoShoot = true;
        		shootAnimationStart = Time.time;
        	}
        }
        if(isShooting)
        {
        	// Set here real player position
        	ShootAtPlayer(Vector3.zero);
        }
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void SetDestination(Vector3 dest, float speed, float shootTime)
	{
		startPosition = transform.position;
		startTime = Time.time;
		mySpeed = speed;
		shootAnimationTime = shootTime;
		destination = dest;
		travelDistance = Vector3.Distance(transform.position, dest);
		transform.LookAt(destination);
		isMoving = true;
	}

	public bool IsWorking()
	{
		return (isMoving || isShooting);
	}

	private void ShootAtPlayer(Vector3 pos)
	{
		float timeGone = Time.time - shootAnimationStart;
		float timeGoneNormalized = timeGone / shootAnimationTime;

		// first half, looking to player
		if( timeGoneNormalized <  0.5f )
		{
			Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos - transform.position), timeGoneNormalized * (1f/timeGoneNormalized) );
			if(!(float.IsNaN(rot.x + rot.y + rot.z + rot.x)))
				transform.rotation = rot;
		}

		// half time, lets shoot
		if( timeGoneNormalized >= 0.5f && hastoShoot)
		{
        	// Shoot a ray that disipears after 5 sec
        	var playerObj = GameObject.FindWithTag("Player");
        	var player = playerObj.GetComponent<PlayerManager>();
        	player.CmdSpawnShot(transform.position, transform.rotation);
        	hastoShoot = false;
		}
		// Trigger shoot event

		if( timeGone > shootAnimationTime )
			isShooting = false;
	}
}