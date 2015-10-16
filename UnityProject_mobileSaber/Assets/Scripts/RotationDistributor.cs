using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;

// State object for receiving data from remote device.
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 256;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();

}

public class RotationDistributor : MonoBehaviour {

    public string standardIP = "192.168.0.14";

    //private float speed = 10.0F;
    private int port = 25005;
    private InputField IP;
    private GameObject button;
    private static bool connected = false;
    private Socket client;
    private Transform cubeTransform; 
    private static Quaternion cubeRotation;
    private Quaternion quatMult;

    // ManualResetEvent instances signal completion.
    private static ManualResetEvent connectDone = new ManualResetEvent(false);
    private static ManualResetEvent sendDone = new ManualResetEvent(false);
    private static ManualResetEvent receiveDone = new ManualResetEvent(false);

    void Start ()
    {
        connected = false;
        button = GameObject.Find("Button");
        cubeTransform = GameObject.Find("Phone").GetComponent<Transform>();
        IP = GameObject.Find("InputField").GetComponent<InputField>();
        IP.text = standardIP;
        Input.gyro.updateInterval = 0.01F;
        Input.gyro.enabled = true;

        #if UNITY_IPHONE
            quatMult = new Quaternion(0f, 0f, 0.7071f, 0.7071f);
        #endif
        #if UNITY_ANDROID
            quatMult = new Quaternion(0f, 0f, 0.7071f, -0.7071f);
        #endif
        #if UNITY_EDITOR
            quatMult = Quaternion.identity;
        #endif
    }

    void Update ()
    {
        if(connected)
        {
            button.GetComponentInChildren<Text>().text = "Disconnect";
        }
        else
        {
            button.GetComponentInChildren<Text>().text = "Connect";
        }

        /* Rotation from acceleration first test
        Vector3 dir = Vector3.zero;
        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;
        if (dir.sqrMagnitude > 1)
            dir.Normalize();
        dir *= Time.deltaTime;
        Vector3 trans = dir * speed;
        this.transform.rotation = Quaternion.Euler(trans); */
        Quaternion rotation;

         #if UNITY_IPHONE
            rotation = Input.gyro.attitude;
        #endif
        #if UNITY_ANDROID
            rotation = Quaternion(Input.gyro.attitude.w, Input.gyro.attitude.x, Input.gyro.attitude.y, Input.gyro.attitude.z);
        #endif
        #if UNITY_EDITOR
            rotation = Quaternion.identity;
        #endif

        cubeTransform.localRotation = rotation * quatMult;

        if(connected)
        {
            try
            {
                Send(client, "<BOF>" + rotation.ToString() + "<EOF>");
                sendDone.WaitOne();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    void OnApplicationQuit()
    {
        if(connected)
        {
            //client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }

    public void Connection()
    {
        if(!connected)
        {
            string ip = IP.text;
            Debug.Log("[CLIENT] Connecting to Server [" + ip + ":" + port + "]"); 

            try
            {
                IPAddress ipAdress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAdress, port);

                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Receive(client);

            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        else
        {
            //client.Shutdown(SocketShutdown.Both);
            client.Close();
            connected = false;
        }

    }

     bool SocketConnected(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if (part1 && part2)
            return false;
        else
            return true;
    }

    private static void Receive(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void ReceiveCallback( IAsyncResult ar )
    {
        if(!connected)
        {
            connected = true;
            Debug.Log("[CLIENT] Connected to Server successfully.");
        }

        else
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject) ar.AsyncState;
                Socket client = state.workSocket;
    
                    // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
    
                        // Get the rest of the data.
                        client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    private static void Send(Socket client, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
    }

    private static void SendCallback(IAsyncResult ar) {
        try
        {
            //Retrieve the socket from the state object.
            //Socket client = (Socket) ar.AsyncState;

            // Complete sending the data to the remote device.
            //int bytesSent = client.EndSend(ar);
            //Debug.Log("Sent " + bytesSent + " bytes to server.");

            // Signal that all bytes have been sent.
            sendDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket) ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Debug.Log("[CLIENT] Socket connected to " + client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}