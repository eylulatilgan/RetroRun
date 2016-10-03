using UnityEngine;
using System.Collections;

public class DummyMultiplayer : Photon.PunBehaviour {
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    
}
