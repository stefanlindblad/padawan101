using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using KinectJT = Windows.Kinect.JointType;

public class TestMotion : MonoBehaviour {
    private KinectManager _testObject;
    private bool normBody = false;
    private Vector3 zDeltaDistance;

    
	
	void Start () {
        GameObject KM = GameObject.FindGameObjectWithTag("KinectManager");
        _testObject = KM.GetComponent<KinectManager>();
	}
	
	// Update is called once per frame
	void Update () {
        Result result = _testObject.getBodyPartPos(KinectJT.HandRight);

        if (result.Success)
        {
            Vector3 posHand = result.AccessToken[0];

            if (!normBody)
            {
                normBody = true;
                zDeltaDistance = NormalizePlayerBodyPos();
            }

            
            this.transform.position = posHand + zDeltaDistance;
            //Debug.Log(result.AccessToken[0]);

        }
        else{normBody = false;}
	}


    private Vector3 NormalizePlayerBodyPos(){
        //Get spine
        Result result = _testObject.getBodyPos();
        if (result.Success)
        {
            Vector3 pos = result.AccessToken[0]; //Check mean distance
        }


        Vector3 distace = new Vector3(0.0f, 0.0f, 0.0f);
        return distace;
    }
}
