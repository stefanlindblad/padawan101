using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class RotationDistributor : MonoBehaviour {

    //private float speed = 10.0F;
    private int port = 25005;
    private InputField IP;
    private GameObject button;
    private static bool connected = false;
    private Socket client;
    private Transform cubeTransform; 
    private static bool vibrate = false;

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
        IP.text = PlayerPrefs.GetString("IP_Address");
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
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

        CheckVibrate();

        double magnetTime = Input.compass.timestamp;
        Quaternion gyroAttitudeData = Input.gyro.attitude;
        Vector3 gyroRotationData = Input.gyro.rotationRateUnbiased;
        Vector3 accelData = Input.acceleration;
        Vector3 gravityData = Input.gyro.gravity.normalized;
        Vector3 magnetData = Input.compass.rawVector;



        cubeTransform.localRotation = Input.gyro.attitude;

        if(connected)
        {
            try
            {
                Send(client, "<BOF>" + magnetTime.ToString("000000000.000000000") + "," + gyroAttitudeData.x.ToString("000.000") + ","
                                     +  gyroAttitudeData.y.ToString("000.000") + "," + gyroAttitudeData.z.ToString("000.000") + ","
                                     + gyroAttitudeData.w.ToString("000.000") + "," + gyroRotationData.x.ToString("000.000") + "," 
                                     + gyroRotationData.y.ToString("000.000") + "," + gyroRotationData.z.ToString("000.000") + ","
                                     + accelData.x.ToString("000.000") + "," + accelData.y.ToString("000.000") + "," 
                                     + accelData.z.ToString("000.000") + "," + gravityData.x.ToString("000.000") + ","
                                     + gravityData.y.ToString("000.000") + "," + gravityData.z.ToString("000.000") + ","
                                     + magnetData.x.ToString("000.000") + "," + magnetData.y.ToString("000.000") + "," 
                                     + magnetData.z.ToString("000.000") + "<EOF>");
                sendDone.WaitOne();
            }
            catch (System.Exception e)
            {
                connected = false;
                Debug.Log(e.ToString());
            }
        }   
    }

    void OnApplicationQuit()
    {
        if(connected)
        {
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
                PlayerPrefs.SetString ("IP_Address", ip);
                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Receive(client);

            }
            catch (System.Exception e)
            {
                connected = false;
                Debug.Log(e.ToString());
            }
        }
        else
        {
            client.Close();
            connected = false;
        }

    }

    private bool SocketConnected(Socket s)
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
            connected = false;
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
                // All the data has arrived; put it in response.
                if (state.sb.Length > 1)
                {
                    String response = state.sb.ToString();

                    // add other actions here if necessary
                    if(response == "vibration")
                        vibrate = true;

                    state.sb.Length = 0;
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            else
            {
                client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,new AsyncCallback(ReceiveCallback), state);
                receiveDone.Set();
            }
        }
        catch (Exception e)
        {
            connected = false;
            Debug.Log(e.ToString());
        }
    }

    private void CheckVibrate()
    {
        if(vibrate)
        {
            Handheld.Vibrate();
            vibrate = false;
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
            Socket handler = (Socket) ar.AsyncState;
            // Complete sending the data to the remote device.
            handler.EndSend(ar);
            // Signal that all bytes have been sent.
            sendDone.Set();
        }
        catch (Exception e)
        {
            connected = false;
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
            connected = false;
            Debug.Log(e.ToString());
        }
    }
}