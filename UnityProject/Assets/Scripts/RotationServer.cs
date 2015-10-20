using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

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

public class RotationServer : MonoBehaviour
{
    private int port = 25005;
    private Transform cubeTransform; 
    private static Quaternion cubeRotation;
    private static Socket connectSocket;
    private static RotationServer server;
    private static string IP;
    private static Vector3 gyroData;
    private static Vector3 accelData;
    private static Vector3 magnetData;
    private static bool connected;

    private float lastVibrate = 0;

    void Start()
    {
        IP = Network.player.ipAddress;
        cubeTransform = GameObject.Find("Phone").GetComponent<Transform>();
        cubeRotation = Quaternion.identity;
        connected = false;
        StartListening();
        server = this;
        lastVibrate = Time.time;
        gyroData = Vector3.zero;
        accelData = Vector3.zero;
        magnetData = Vector3.zero;

    }

    void Update()
    {
        cubeTransform.localRotation = cubeRotation;

        // Test Vibration every 2sec, remove later
        if(Time.time - lastVibrate > 2)
        {
            VibratePhone();
            lastVibrate = Time.time;
        }

    }

    // Thread signal.
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public void StartListening()
    {

        IPAddress ipAdress = IPAddress.Parse(IP);
        IPEndPoint localEndPoint = new IPEndPoint(ipAdress, port);

        // Create a TCP/IP socket.
        connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            connectSocket.Bind(localEndPoint);
            connectSocket.Listen(2);

                // Start an asynchronous socket to listen for connections.
                Debug.Log("[SERVER] Waiting for a connection...");
                connectSocket.BeginAccept(new AsyncCallback(AcceptCallback), connectSocket );
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            connected = false;
        }
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.
        allDone.Set();

        // Get the socket that handles the client request.
        connectSocket = (Socket) ar.AsyncState;
        connectSocket = connectSocket.EndAccept(ar);

        // Create the state object.
        StateObject state = new StateObject();
        state.workSocket = connectSocket;
        connectSocket.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        Send(connectSocket, "Hello!");
        Debug.Log("[SERVER] Sent INIT sequence to client.");
        connected = true;

    }

    public static void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject) ar.AsyncState;
        Socket handler = state.workSocket;

        try
        {
            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // One block of data has been read from the client. Display it on the console.
                    //Debug.Log("[SERVER] Read " + content.Length  + " bytes from socket. Data : " + content + "\n");
                    getDataFromString(content);
                    bytesRead = 0;
                    state.sb.Length = 0;
                }

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            // we received Zero, client disconnected
            else if(bytesRead == 0)
            {
                server.closeConnection();
            }
        }
        catch(Exception e)
        {
            connected = false;
            Debug.Log(e.ToString());
            server.closeConnection();
        }
    }

    private void closeConnection()
    {
        if(connectSocket != null)
        {
            connectSocket.Close();
            connectSocket = null;
        }
        StartListening();
    }

    private static void getDataFromString(string str)
    {
        string[] temp1 = str.Split('>');
        string[] temp2 = temp1[1].Split('<');
        string vectors = temp2[0];
        string[] datas = vectors.Split(new Char[] {')', '('} );
        float x, y, z;
        if(datas.Length >= 5)
        {
            if(datas[1].Length > 0)
            {
                string[] gyroTemp = datas[1].Split(',');
                x = float.Parse(gyroTemp[0]);
                y = float.Parse(gyroTemp[1]);
                z = float.Parse(gyroTemp[2]);
                gyroData = new Vector3(x,y,z);
                Debug.Log("Gyro: " + gyroData);
            }
    
            if(datas[3].Length > 0)
            {
                string[] accelTemp = datas[3].Split(',');
                x = float.Parse(accelTemp[0]);
                y = float.Parse(accelTemp[1]);
                z = float.Parse(accelTemp[2]);
                accelData = new Vector3(x,y,z);
                Debug.Log("Acceleration: " + accelData);
            }
    
            if(datas[5].Length > 0)
            {
                string[] magnetTemp = datas[5].Split(',');
                x = float.Parse(magnetTemp[0]);
                y = float.Parse(magnetTemp[1]);
                z = float.Parse(magnetTemp[2]);
                magnetData = new Vector3(x,y,z);
                Debug.Log("Magnetometer: " + magnetData);
            }

        }

        // Do sensor fusion here

        Quaternion rotation = Quaternion.identity;
        cubeRotation = rotation;
    }

    private static void Send(Socket handler, String data)
    {
        try
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }
        catch (Exception e)
        {
            connected = false;
            Debug.Log(e.ToString());
        }
    }

    public void VibratePhone()
    {
        if(connected)
        {
            Send(connectSocket, "vibration");
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket) ar.AsyncState;
            // Complete sending the data to the remote device.
            handler.EndSend(ar);
        }
        catch (Exception e) 
        {
            connected = false;
            Debug.Log(e.ToString());
            server.closeConnection();
        }
    }
}