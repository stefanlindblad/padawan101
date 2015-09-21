using UnityEngine;
using System.Collections;

public class LeapMotionVolumeRenderer : MonoBehaviour {

    private const float BOX_RADIUS = 0.45f;
    private const float BOX_WIDTH = 0.965f;
    private const float BOX_DEPTH = 0.6671f;
    private Color c1 = Color.blue;
    private Color c2 = Color.blue;
    private int lengthOfLineRenderer = 5;

    private HandController _controller;
    private LineRenderer _renderer;

    void Start()
    {
        _controller = GetComponent<HandController>();
        _renderer = GetComponent<LineRenderer>();

        if (_controller && _renderer)
        {
            _renderer.SetColors(c1, c2);
            _renderer.SetWidth(0.01F, 0.01F);
            _renderer.SetVertexCount(lengthOfLineRenderer);
        }

    }

    void Update()
    {
        Vector3 origin = _controller.transform.TransformPoint(Vector3.zero);

        Vector3 local_top_left = new Vector3(-BOX_WIDTH, BOX_RADIUS, BOX_DEPTH);
        Vector3 top_left = _controller.transform.TransformPoint(BOX_RADIUS * local_top_left.normalized);

        Vector3 local_top_right = new Vector3(BOX_WIDTH, BOX_RADIUS, BOX_DEPTH);
        Vector3 top_right = _controller.transform.TransformPoint(BOX_RADIUS * local_top_right.normalized);

        Vector3 local_bottom_left = new Vector3(-BOX_WIDTH, BOX_RADIUS, -BOX_DEPTH);
        Vector3 bottom_left = _controller.transform.TransformPoint(BOX_RADIUS * local_bottom_left.normalized);

        Vector3 local_bottom_right = new Vector3(BOX_WIDTH, BOX_RADIUS, -BOX_DEPTH);
        Vector3 bottom_right = _controller.transform.TransformPoint(BOX_RADIUS * local_bottom_right.normalized);

        if(_controller && _renderer)
        {
            _renderer.SetPosition(0, bottom_right);
            _renderer.SetPosition(1, bottom_left);
            _renderer.SetPosition(2, top_left);
            _renderer.SetPosition(3, top_right);
            _renderer.SetPosition(4, bottom_right);

            //_renderer.SetPosition(0, origin);
            //_renderer.SetPosition(1, top_left);
            //_renderer.SetPosition(2, origin);
            //_renderer.SetPosition(3, top_right);
            //_renderer.SetPosition(4, origin);
            //_renderer.SetPosition(5, bottom_left);
            //_renderer.SetPosition(6, origin);
            //_renderer.SetPosition(7, bottom_right);
            //_renderer.SetPosition(8, bottom_left);
            //_renderer.SetPosition(9, top_left);
            //_renderer.SetPosition(10, top_right);
            //_renderer.SetPosition(11, bottom_right);
        }
    }
}
