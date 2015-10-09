using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkManager : NetworkBehaviour
{
    public GameObject mainEngine;

    [Header("Spawnable Prefabs")]
    public List<GameObject> prefabs = new List<GameObject> ();

    [Header("Network information")]
    public int portNumber = 4444;
    public string ipAdress = "127.0.0.1";

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
        myClient.Connect (ipAdress, portNumber);
    }

    public NetworkClient GetClient() {
        return myClient;
    }

    public void SetupHost()
    {
        Debug.Log("Started server.");
        NetworkServer.Listen(portNumber);

        SetupClient();

    }

    public void OnConnected (NetworkMessage msg)
    {
        Debug.Log (msg.ToString ());
    }

    // Use this for initialization
    void Start ()
    {
        if (mainEngine)
            mainEngine.SetActive(true);
    }

    // Update is called once per frame
    void Update ()
    {

    }
}
