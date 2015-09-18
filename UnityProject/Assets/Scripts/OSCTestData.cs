using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityOSC;

public class OSCTestData : MonoBehaviour {

	// Use this for initialization
	void Start () {

		OSCHandler.Instance.Init();
	
	}
	
	// Update is called once per frame
	void Update () {

		GameObject ls = GameObject.Find("LightSaber");

		OSCHandler.Instance.UpdateLogs();

		Dictionary<string, ServerLog> servers;
		servers = OSCHandler.Instance.Servers;

		foreach(var value in servers)
		{	
			//Debug.Log(value.Key);

			float rotationX = 0.0f;
			float rotationY = 0.0f;
			float rotationZ = 0.0f;
			Debug.Log("Current Rotation: " + ls.transform.rotation.eulerAngles);

			foreach(var packet in value.Value.packets)
			{
				string adr = packet.Address;

				// Pitch - X Value
				if(adr.Contains("/pry/0"))
				{
					foreach(var data in packet.Data)
					{
						rotationX = GetAngle((float) data);
						Debug.Log("Pitch: " + rotationX);
					}
				}

				// Roll - Z Value
				else if(adr.Contains("/pry/1"))
				{
					foreach(var data in packet.Data)
					{
						rotationZ = GetAngle((float) data);
						Debug.Log("Roll: " + rotationZ);
					}
				}

				// Yaw - Y Value
				else if(adr.Contains("/pry/2"))
				{
					foreach(var data in packet.Data)
					{
						rotationY = GetAngle((float) data);
						Debug.Log("Yaw: " + rotationY);
					}
				}
			}

			ls.transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
			Debug.Log("New Rotation: " + ls.transform.rotation.eulerAngles);
		}	
	}


	float GetAngle(float value)
	{
		return value * 360.0f;
	}


}
