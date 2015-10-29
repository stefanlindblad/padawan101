using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class KinectManager : MonoBehaviour
{
    public GameObject platform;
    public GameObject lightSaber;
    public bool useRightHand = true;
    public Vector3 OffsetVectorOfPlayer = new Vector3(0.0f, 1.0f, 1.5f); //Hardcoded :( Could be calibrated
    public float KinectBodyScale = 1.0f;

    private BodySourceManager _BodyManager;
    private Kinect.Body CurrentPlayer;
    public Dictionary<ulong, Kinect.Body> StoredBodies = new Dictionary<ulong, Kinect.Body>();

    public Material BoneMaterial;
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

    private Dictionary<Kinect.JointType, Vector3[]> smoothedJoints;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private Dictionary<int, Kinect.JointType> jointMap = new Dictionary<int, Kinect.JointType>() {    
        {0,Kinect.JointType.SpineBase},
        {1,Kinect.JointType.SpineMid},
        {2,Kinect.JointType.Neck},
        {3,Kinect.JointType.Head},
        {4,Kinect.JointType.ShoulderLeft},
        {5,Kinect.JointType.ElbowLeft},
        {6,Kinect.JointType.WristLeft},
        {7,Kinect.JointType.HandLeft},
        {8,Kinect.JointType.ShoulderRight},
        {9,Kinect.JointType.ElbowRight},
        {10,Kinect.JointType.WristRight},
        {11,Kinect.JointType.HandRight},
        {12,Kinect.JointType.HipLeft},
        {13,Kinect.JointType.KneeLeft},
        {14,Kinect.JointType.AnkleLeft},
        {15,Kinect.JointType.FootLeft},
        {16,Kinect.JointType.HipRight},
        {17,Kinect.JointType.KneeRight},
        {18,Kinect.JointType.AnkleRight},
        {19,Kinect.JointType.FootRight},
        {20,Kinect.JointType.SpineShoulder},
        {21,Kinect.JointType.HandTipLeft},
        {22,Kinect.JointType.ThumbLeft},
        {23,Kinect.JointType.HandTipRight},
        {24,Kinect.JointType.ThumbRight},
    };




    //----------------------------------------------------------------

    void Start()
    {
        // Get body from Kinect
        _BodyManager = this.GetComponent<BodySourceManager>();
        if (_BodyManager == null) { return; }
        /*positionKalman = new KalmanFilter();
        positionKalman.initialize(3, 3);
        positionKalman.setR(1.0f);*/
        smoothedJoints = new Dictionary<Kinect.JointType, Vector3[]>();
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            smoothedJoints[jt] = new Vector3[4];
        }
    }

    void Update()
    {

        //Get the data from the body
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null) { return; }




        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }




        // --Check tracked bodies
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
                StoredBodies.Remove(trackingId);
            }
        }

        // Then add tracked bodies
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                    // Store bodies
                    StoredBodies[body.TrackingId] = body;
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }

        //Update the current player
        updateCurrentPlayer(trackedIds);

        updateSmoothedJoints();
    }

    private void updateSmoothedJoints()
    {
        if (CurrentPlayer == null) return;
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Vector3 newJointPosition = GetVector3FromJoint(CurrentPlayer.Joints[jt]);

            for (int i = 3; i > 0; i--) {
                 smoothedJoints[jt][i] = smoothedJoints[jt][i-1];
            }
            smoothedJoints[jt][0] = newJointPosition;
            smoothedJoints[jt][0] = interp(smoothedJoints[jt]);
            //Debug.Log("jt:" + jt +" len:" + smoothedJoints[jt].Length);
        }
        
    }

    private Vector3 interp(Vector3[] values) 
    {
        Vector3 smooth1 = Vector3.Lerp(Vector3.Lerp(values[0], values[1], 0.5f), Vector3.Lerp(values[1], values[2], 0.5f), 0.5f);
        Vector3 smooth2 = Vector3.Lerp(Vector3.Lerp(values[1], values[2], 0.5f), Vector3.Lerp(values[2], values[3], 0.5f), 0.5f);
        return Vector3.Lerp(smooth1, smooth2, 0.5f);
    }

    // -----------------------------------------------------------------
    // ------------------  POSITIONAL TRACKING -------------------------
    // -----------------------------------------------------------------

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

    // Return every position in the body in a list of vec3
    public Result getBodyPos(bool interpolate = false)
    {
        Result result = new Result();

        List<Vector3> pos = new List<Vector3>();       
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            pos.Add(smoothedJoints[jt][0]);
        }

        result.Success = true;
        result.AccessToken = pos;

        return result;
    }

    // Return the position of jointTypeName
    public Result getBodyPartPos(Kinect.JointType jointTypeName, bool interpolate = false)
    {
        Result result = new Result();

        try
        {
            List<Vector3> pos = new List<Vector3>(1);
            pos.Add(GetVector3FromJoint(CurrentPlayer.Joints[jointTypeName]));

            result.Success = true;
            result.AccessToken = pos;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            result.Success = false;
        }

        return result;
    }

    public void updateCurrentPlayer(List<ulong> trackedIds)
    {
        if (StoredBodies.Count == 1) {

            CurrentPlayer = StoredBodies[trackedIds[0]];
        }
        else if (StoredBodies.Count > 1)
        {
            CurrentPlayer = StoredBodies[trackedIds[0]]; //Do something better here!
        }
        else
        {
            //Debug.Log("Stored Bodies not initilized");
        }

    }



    // -----------------------------------------------------------------
    // ----------------  GRAPHICAL REPRESENTATION ----------------------
    // -----------------------------------------------------------------

    // Initialize gameObject
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Changed it to a empty go that the mesh does not disturb. Can change it back.
            //GameObject jointObj = new GameObject();

            //LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            //lr.SetVertexCount(2);
            //lr.material = BoneMaterial;
            //lr.SetWidth(0.005f, 0.005f);

            jointObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        return body;
    }

    // Update/Refresh gameObject
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            //LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            //if (targetJoint.HasValue)
            //{
            //    lr.SetPosition(0, jointObj.localPosition);
            //    lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
            //    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            //}
            //else
            //{
            //    lr.enabled = false;
            //}
        }
    }

    // -----------------------------------------------------------------
    // ------------------  STATIC METHODS ------------------------------
    // -----------------------------------------------------------------

    // Get 3D pos of Kinect.Joint
    private Vector3 GetVector3FromJoint(Kinect.Joint joint, bool centerToPlatform = true, int faceTo = -1)
    {
        // faceTo = -1 gives the POV of the player (1st person) and =1 gives the mirror
        Vector3 kinectPos = new Vector3(joint.Position.X * KinectBodyScale, joint.Position.Y * KinectBodyScale, faceTo * joint.Position.Z * KinectBodyScale);

        if (centerToPlatform)
        {
            kinectPos += platform.transform.position + OffsetVectorOfPlayer;
        }
        return kinectPos;
    }


    // Color for linerenderer
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
}



// -----------------------------------------------------------------
// ------------------  OTHER CLASSES ------------------------------
// -----------------------------------------------------------------

// Return object when asking for position
public class Result
{
    public bool Success { get; set; }
    public List<Vector3> AccessToken { get; set; }
    public string ErrorMessage { get; set; }
}