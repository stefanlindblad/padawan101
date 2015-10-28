using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIFactory : MonoBehaviour
{
	public float minX = -100f;
	public float minY = -100f;
	public float minZ = -100f;
	public float maxX = 100f;
	public float maxY = 100f;
	public float maxZ = 100f;
	public float minDistance = 20f;
	public float maxDistance = 50f;
	public float shootAnimationTime = 3f;
	public float minSpeed = 15f;
	public float maxSpeed = 30f;
	public float shootSpread = 5f;

	private List<AINode> controlledObjects;


	void Awake ()
	{
		controlledObjects = new List<AINode>();
	}
	
	void Update ()
	{
		foreach(AINode node in controlledObjects)
		{
			if(node.IsWorking() == false)
			{
				float speed = Random.Range(minSpeed, maxSpeed);
				node.SetDestination( NewPosition(node.GetPosition(), minDistance, maxDistance), RandomSpread(), speed, shootAnimationTime );
			}
		}
	}

	public void Register(AINode obj)
	{
		controlledObjects.Add(obj);
	}

	private Vector3 NewPosition(Vector3 oldPos, float minDist, float maxDist)
	{
		Vector3 newPos = Vector3.zero;
		while(true)
		{
			newPos = RandomMovement();
			if( ((newPos - oldPos).magnitude > minDist) && ((newPos - oldPos).magnitude < maxDist) )
			{
				return newPos;
			}
		}
	}

	private Vector3 RandomSpread()
		{
		return new Vector3(Random.Range(-shootSpread, shootSpread), Random.Range(-shootSpread, shootSpread), Random.Range(-shootSpread, shootSpread));
	}

	private Vector3 RandomMovement()
	{
		return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
	}
}
