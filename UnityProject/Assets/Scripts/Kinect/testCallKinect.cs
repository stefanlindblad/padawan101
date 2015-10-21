using UnityEngine;
using System.Collections;
using KinectJT = Windows.Kinect.JointType;

public class testCallKinect : MonoBehaviour {
    private KinectManager _testObject;

	// Update is called once per frame
	void Update () 
    {
        // Find the kinectManager object
        GameObject KM = GameObject.FindGameObjectWithTag("KinectManager");
        _testObject = KM.GetComponent<KinectManager>();


        //-----------------------
        // Get all positions from the Kinect Manager
        Result result1 = _testObject.getBodyPos();
        if(result1.Success)
        {
            //Debug.Log(result1.AccessToken);
        }
        else
        {
            //If u like to do something when we do not have a position from the kinect
        }


        //-----------------------
        // Get a sertain position from the Kinect Manager
        Result result2 = _testObject.getBodyPartPos(KinectJT.HandRight);
        if (result2.Success)
        {
            Debug.Log(result2.AccessToken[0]);
        }
        else
        {
            //If u like to do something when we do not have a position from the kinect
        }
	}
}



/////////////////////////////////////////////////////////////////////
//////////////// INPUTS FOR getBodyPartPos /////////////////////////
/*
        SpineBase                                =0,
        SpineMid                                 =1,
        Neck                                     =2,
        Head                                     =3,
        ShoulderLeft                             =4,
        ElbowLeft                                =5,
        WristLeft                                =6,
        HandLeft                                 =7,
        ShoulderRight                            =8,
        ElbowRight                               =9,
        WristRight                               =10,
        HandRight                                =11,
        HipLeft                                  =12,
        KneeLeft                                 =13,
        AnkleLeft                                =14,
        FootLeft                                 =15,
        HipRight                                 =16,
        KneeRight                                =17,
        AnkleRight                               =18,
        FootRight                                =19,
        SpineShoulder                            =20,
        HandTipLeft                              =21,
        ThumbLeft                                =22,
        HandTipRight                             =23,
        ThumbRight                               =24,
     */