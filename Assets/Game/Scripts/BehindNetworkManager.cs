using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class BehindNetworkManager : NetworkManager
{
    public void Start()
    {
        networkAddress = File.ReadAllText("address.txt");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }
}
    