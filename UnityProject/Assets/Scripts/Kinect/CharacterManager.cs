using UnityEngine;
using System.Collections;
using KinectJT = Windows.Kinect.JointType;

public class CharacterManager : MonoBehaviour {

    [System.Serializable]
    public class _BodyParts
    {
        public Transform root;
        public Transform head;
        public Transform neck;
        public Transform torso;
        public Transform rightShoulder;
        public Transform rightElbow;
        public Transform rightHand;
        public Transform rightHip;
        public Transform rightKnee;
        public Transform rightAnkle;
        public Transform rightFoot;
        public Transform leftShoulder;
        public Transform leftElbow;
        public Transform leftHand;
        public Transform leftHip;
        public Transform leftKnee;
        public Transform leftAnkle;
        public Transform leftFoot;
        public Transform leftThumb;
        public Transform rightThumb;
    }

    public _BodyParts MyBody;

    private KinectManager _testObject;

	// Use this for initialization
	void Start () {
        // INIT
        GameObject KM = GameObject.FindGameObjectWithTag("KinectManager");
        _testObject = KM.GetComponent<KinectManager>();
	}
	
	// Update is called once per frame
	void Update () {
	    //Get all positions of the kinect
        Result result = _testObject.getBodyPos();
        
        // Update rotations
        if (result.Success)
        {
            /*MyBody.root.transform.position = result.AccessToken[0];           //root
            MyBody.torso.transform.position = result.AccessToken[1];            //torso
            MyBody.neck.transform.position = result.AccessToken[2];             //neck
            MyBody.head.transform.position = result.AccessToken[3];             //head
            MyBody.leftShoulder.transform.position = result.AccessToken[4];     // Left sholder
            MyBody.leftElbow.transform.position = result.AccessToken[5];        // Left Elbow
            //MyBody.leftHand.transform.position = result.AccessToken[6];   

            MyBody.leftHand.transform.position = result.AccessToken[7];         //leftHand
            MyBody.rightShoulder.transform.position = result.AccessToken[8];    //rightSholder
            MyBody.rightElbow.transform.position = result.AccessToken[9];       //RightElbow
            //MyBody.leftHand.transform.position = result.AccessToken[10];    
            MyBody.rightHand.transform.position = result.AccessToken[11];       //rightHand
            MyBody.leftHip.transform.position = result.AccessToken[12];         //leftHip
            MyBody.leftKnee.transform.position = result.AccessToken[13];        //leftKnee
            MyBody.leftAnkle.transform.position = result.AccessToken[14];       //leftAnkle
            MyBody.leftFoot.transform.position = result.AccessToken[15];        //leftFoot

            MyBody.rightHip.transform.position = result.AccessToken[16];        //rightHip
            MyBody.rightKnee.transform.position = result.AccessToken[17];       //rightKnee
            MyBody.rightAnkle.transform.position = result.AccessToken[18];      //rightAnkle
            MyBody.rightFoot.transform.position = result.AccessToken[19];       //rightFoot

            //MyBody.leftHand.transform.position = result.AccessToken[20];      //leftHand
            //MyBody.HandTipLeft.transform.position = result.AccessToken[21];   //handTipLeft
            MyBody.leftThumb.transform.position = result.AccessToken[22];       //leftThumb
            //MyBody.handtipRight.transform.position = result.AccessToken[23];  //handTipRight
            MyBody.rightThumb.transform.position = result.AccessToken[24];*/

            //MyBody.leftShoulder.rotation = FindRotation(result.AccessToken[4], result.AccessToken[5], new Vector3(0,1,0));
            //MyBody.leftElbow.rotation = FindRotation(result.AccessToken[5], result.AccessToken[7], new Vector3(0,1,0));

            MyBody.rightShoulder.rotation = FindRotation(result.AccessToken[8], result.AccessToken[9], new Vector3(0,1,0));
            MyBody.rightElbow.rotation = FindRotation(result.AccessToken[9], result.AccessToken[11], new Vector3(0,1,0));
            //MyBody.rightElbow.Rotate(new Vector3(0.0f, 180.0f, 0.0f), Space.Self);
        }
            
	}

    private Quaternion FindRotation(Vector3 fromJoint, Vector3 toJoint, Vector3 wantedDirection)
    {
        toJoint.x = -toJoint.x;
        fromJoint.x = -fromJoint.x;

        toJoint.y = toJoint.y;
        fromJoint.y = fromJoint.y;

        toJoint.z = -toJoint.z;
        fromJoint.z = -fromJoint.z;
        

        Vector3 boneVector = toJoint - fromJoint;
        Quaternion restulAngle = Quaternion.FromToRotation(boneVector, wantedDirection);
        return restulAngle;
    }
}
