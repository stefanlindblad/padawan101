﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Leap;

public class LightSaberController : NetworkBehaviour
{
    public string LeapMotionController = "";
    public bool UseRightHand = true;

    private HandController controller;
    private Transform t;
    private bool validSettings = false;



    void Start ()
    {
        validSettings = false;
        GameObject cObject = GameObject.Find(LeapMotionController);
        controller = cObject.GetComponent<HandController>();
        t = this.gameObject.transform;

        if (controller && t)
            validSettings = true;
    }

    void Update ()
    {

        t.Rotate(Vector3.forward * 4.0f * Time.deltaTime);
        t.Rotate(Vector3.right * 4.0f * Time.deltaTime);

        if (validSettings)
        {
            HandModel model = null;

            foreach (HandModel hm in controller.GetAllGraphicsHands())
            {
                Hand h = hm.GetLeapHand();


                // We have a valid right hand
                if (h.IsValid && h.IsRight && UseRightHand)
                    model = hm;

                // we have a valid left hand
                else if (h.IsValid && !h.IsRight && !UseRightHand)
                    model = hm;
            }


            if(model != null)
            {
                t.position = model.GetPalmPosition();
                t.rotation = model.GetPalmRotation();
            }

        }
    }
}
