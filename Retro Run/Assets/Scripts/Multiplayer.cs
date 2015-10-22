using UnityEngine;
using System.Collections;
using Photon;
public class Multiplayer : PunBehaviour {
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    GameObject player;

	// Use this for initialization
	void Start () {
        PhotonNetwork.ConnectUsingSettings("0.1");
	}

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    void OnJoinedLobby() {
        Debug.Log("Joined to Lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom("Room");
    }

    void OnJoinedRoom() {
        Debug.Log("Ready for action!");
        if (PhotonNetwork.isMasterClient) {
            player = PhotonNetwork.Instantiate(player1Prefab.name, new Vector2(0, 0), Quaternion.identity, 0);
        }
        else
        {
            player = PhotonNetwork.Instantiate(player2Prefab.name, new Vector2(0.6f, 0), Quaternion.identity, 0);            
        }       
        player.GetComponentInChildren<Camera>().enabled = true;
        player.GetComponent<Rigidbody2D>().isKinematic = false;
        player.GetComponent<PlayerControls>().enabled = true;
    }
}
