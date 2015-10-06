using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net.Sockets;

public class RotationDistributor : MonoBehaviour {


    private float speed = 10.0F;
    private int port = 8888;
    private GameObject GUI;
    private Text IPText;
    System.Net.Sockets.TcpClient clientSocket;

    public void Connection()
    {
        GUI.SetActive(false);
        string ip = IPText.text;

        Debug.Log("Connecting to Server [" + ip + ":" + port + "]");
        clientSocket.Connect(ip, port);
    }

    
    void Start ()
    {
        clientSocket = new System.Net.Sockets.TcpClient();
        GUI = GameObject.Find("GUI");
        IPText = GameObject.Find("IPText").GetComponent<Text>();
        IPText.text = "127.0.0.1";
    }
	

	void Update ()
    {
        Vector3 dir = Vector3.zero;
        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        dir *= Time.deltaTime;
        transform.Translate(dir * speed);

    }
}
