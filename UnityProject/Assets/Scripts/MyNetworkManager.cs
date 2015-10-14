using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    public GameObject mainEngine;

     [Header("Spawnable Prefabs")]
    public List<GameObject> prefabs = new List<GameObject> ();

    private NetworkClient myClient;

    // Create a client and connect to the server port
    public void SetupClient ()
    {
        Debug.Log("SetupClient.");
        foreach (GameObject prefab in prefabs) {
            if (prefab != null)
                ClientScene.RegisterPrefab (prefab);
        }

        myClient = new NetworkClient ();

        myClient.RegisterHandler (MsgType.Connect, OnConnected);
        myClient.Connect (base.networkAddress, base.networkPort);
    }

    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnectedLocalClient);
        /*ClientScene.Ready(myClient.connection);*/
        ClientScene.AddPlayer(myClient.connection, 0);
    }

    public NetworkClient GetClient() {
        return myClient;
    }


    public void SetupHost()
    {
        Debug.Log("Started server.");
        NetworkServer.Listen(base.networkPort);
        SetupLocalClient();
    }

    public void OnConnectedLocalClient (NetworkMessage msg)
    {
        Debug.Log ("HostId: " + myClient.connection.hostId + " connected.");   
    }

    public void OnConnected (NetworkMessage msg)
    {
        Debug.Log ("HostId: " + myClient.connection.hostId + " connected.");
        ClientScene.Ready(myClient.connection);
    }

    // Use this for initialization
    void Start ()
    {
        if (mainEngine)
            mainEngine.SetActive(true);
        base.autoCreatePlayer = true;

        NetworkServer.RegisterHandler(MsgType.AddPlayer, OnAddPlayerMessage);
    }

    void OnAddPlayerMessage(NetworkMessage netMsg)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab,
                                                    new Vector3(0, 0, 0),
                                                    Quaternion.identity);
        player.tag = "Player";
        NetworkServer.AddPlayerForConnection(netMsg.conn, player, 0);

    }
}
