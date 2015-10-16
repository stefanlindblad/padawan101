using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

public class RotationServer : MonoBehaviour
{
    public string ip = "192.168.0.14";
    private int port = 25005;
    private Transform cubeTransform; 
    private static Quaternion cubeRotation;
    private Socket listener;
    private static RotationServer server;

    void Start()
    {
        cubeTransform = GameObject.Find("Phone").GetComponent<Transform>();
        cubeRotation = Quaternion.identity;
        StartListening();
        server = this;
    }

    void Update()
    {
        cubeTransform.localRotation = cubeRotation;
    }

    // Thread signal.
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public void StartListening()
    {

        IPAddress ipAdress = IPAddress.Parse(ip);
        IPEndPoint localEndPoint = new IPEndPoint(ipAdress, port);

        // Create a TCP/IP socket.
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(2);

                // Start an asynchronous socket to listen for connections.
                Debug.Log("[SERVER] Waiting for a connection...");
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener );
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.
        allDone.Set();

        // Get the socket that handles the client request.
        Socket listener = (Socket) ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        // Create the state object.
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        Send(handler, "Hello!");

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
                    getQuaternionFromString(content);
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
            Debug.Log(e.ToString());
            server.closeConnection();
        }
    }

    private void closeConnection()
    {
        if(listener != null)
        {
            listener.Close();
            listener = null;
        }
        StartListening();
    }

    private static bool getQuaternionFromString(string str)
    {
        string[] temp1 = str.Split('>');
        string[] temp2 = temp1[1].Split('<');

        string vector = temp2[0];

        if(vector.StartsWith("(") && vector.EndsWith(")"))
        {
            string[] temp = vector.Substring(1,vector.Length-2).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            float z = float.Parse(temp[2]);
            float w = float.Parse(temp[3]);
            Quaternion inRot = new Quaternion(x,y,z,w);
            //Vector3 euRot = inRot.eulerAngles;
            //cubeRotation = Quaternion.Euler(euRot.x, euRot.z, euRot.y);
            cubeRotation = inRot;
            return true;
        }
        else
            return false;
    }

    private static void Send(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            //Socket handler = (Socket) ar.AsyncState;

            // Complete sending the data to the remote device.
            //int bytesSent = handler.EndSend(ar);
            //Debug.Log("Sent " + bytesSent + " bytes to client.");
            Debug.Log("[SERVER] Sent Initialization Sequence to Client.");
        }
        catch (Exception e) 
        {
            Debug.Log(e.ToString());
            server.closeConnection();
        }
    }
}