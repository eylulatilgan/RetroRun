using UnityEngine;
using System.Collections;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PhotonConnectionScript : PunBehaviour {

    public delegate void PlayerStatus(bool isPlayerReady);
    public static event PlayerStatus Player1StatusReady, Player2StatusReady;

	public delegate void ActivateButton();
	public static event ActivateButton ActivateReadyButton;
	public static event ActivateButton OnLobbyJoin;

    public delegate void PlayerInfo(string skinID, string username);
    public static event PlayerInfo Player1Info, Player2Info;

	public delegate void LoadStatus(string connectionMessage);
	public static event LoadStatus OnConnectionStatus;

	private static string skinID1, skinID2 ;
	private string username1, username2;
    private bool prevStatus1 = false;
    private bool prevStatus2 = false;
    private string roomName = "";
    private uint roomNumber = 0;
	private string connectionStatus;
	/*
	void Awake() 
	{
		DontDestroyOnLoad(skinID1);
		DontDestroyOnLoad(skinID2);
	}
	*/
	void Start()
    {
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
        if (AuthManager.connectedToPlayFab)
        {
            GetPhotonAuthenticationToken();
        }

		connectionStatus = PhotonNetwork.connectionStateDetailed.ToString();
    }

	void Update()
	{
		if(connectionStatus != PhotonNetwork.connectionStateDetailed.ToString())
		{
			OnConnectionStatus(PhotonNetwork.connectionStateDetailed.ToString());
			connectionStatus = PhotonNetwork.connectionStateDetailed.ToString();
		}
	}

    void GetPhotonAuthenticationToken()
    {

        GetPhotonAuthenticationTokenRequest req = new GetPhotonAuthenticationTokenRequest();
        req.PhotonApplicationId = "244e35d0-57ff-4e29-b492-62a470851a50";//"2ebff324-aec4-4b3f-8621-4ead47c7758c"; 

        PlayFabClientAPI.GetPhotonAuthenticationToken(req, OnPhotonAuthenticationSuccess, OnPlayfabError);

    }

    void OnPhotonAuthenticationSuccess(GetPhotonAuthenticationTokenResult result)
    {

        Debug.Log(result.PhotonCustomAuthenticationToken);
        Debug.Log(AuthManager.playfabId);
        ConnectToMasterServer(AuthManager.playfabId, result.PhotonCustomAuthenticationToken);

    }

    void OnPlayfabError(PlayFabError error)
    {

        Debug.Log(error.Error.ToString());
        Debug.Log(error.ErrorMessage);
        Debug.Log(error.ErrorDetails);

    }

    private void ConnectToMasterServer(string id, string ticket)
    {
        //Debug.Log("connect to master server");
        //if (PhotonNetwork.AuthValues != null)
        //{

        //}
        //else
        //{

        //    PhotonNetwork.AuthValues = new AuthenticationValues()
        //    {

        //        AuthType = CustomAuthenticationType.Custom

        //    };

        //    PhotonNetwork.AuthValues.AddAuthParameter(id, ticket);

        //}

        AuthenticationValues authVals = new AuthenticationValues(AuthManager.playfabId);
        authVals.AuthType = CustomAuthenticationType.Custom;
        authVals.AddAuthParameter("username", AuthManager.playfabId);
        authVals.AddAuthParameter("token", ticket);

        PhotonNetwork.ConnectUsingSettings("1.0");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Connected to Master");

        JoinLobby();
    }

    public void JoinLobby()
    {
        Debug.Log("JoinLobby");
        PhotonNetwork.JoinLobby();
    }

    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMessage)
    {
        Debug.Log("Photon Join Room Failed");

        foreach (object o in codeAndMessage)
        {
            Debug.Log(o);
        }

        CreateRoom();

    }

    private void CreateRoom()
    {
        roomName = "RetroRunRoom";
        Debug.Log(roomName + " has been created");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
		OnLobbyJoin();
        if (!PhotonNetwork.isMasterClient)
        {

            ActivateReadyButton();
            skinID2 = PlayFabGameBridge.Instance.currentSkin.ItemId;
            username2 = LoginScript.getUsername();
            Player2Info(skinID2, username2);

            Debug.Log("Player2 sent his skinID and username");
            photonView.RPC("RPCOtherClientSkin", PhotonTargets.OthersBuffered, skinID2, username2);
        }
        else
        {
            skinID1 = PlayFabGameBridge.Instance.currentSkin.ItemId;
            username1 = LoginScript.getUsername();
            Player1Info(skinID1, username1);
            
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        ActivateReadyButton();

        Debug.Log("Player1 sent his skinID and username");
        photonView.RPC("RPCMasterClientSkin", PhotonTargets.OthersBuffered, skinID1, username1);
    }

    void OnEnable()
    {
        LobbyScript.On1Ready += MasterClientReady;
        LobbyScript.On2Ready += OtherClientReady;
    }

    void MasterClientReady(bool isReady)
    {
        if (prevStatus1 != isReady)
        {
            photonView.RPC("RPCMasterClientStatus", PhotonTargets.OthersBuffered, isReady);
            prevStatus1 = isReady;
        }
    }

    void OtherClientReady(bool isReady)
    {
        if (prevStatus2 != isReady)
        {
            photonView.RPC("RPCOtherClientStatus", PhotonTargets.OthersBuffered, isReady);
            prevStatus2 = isReady;
        }
    }

    [PunRPC]
    void RPCMasterClientStatus(bool isReady)
    {
        Player1StatusReady(isReady);
    }

    [PunRPC]
    void RPCOtherClientStatus(bool isReady)
    {
        Player2StatusReady(isReady);
    }

    [PunRPC]
    void RPCMasterClientSkin(string skinID, string username)
    {
        Player1Info(skinID, username);
    }

    [PunRPC]
    void RPCOtherClientSkin(string skinID, string username)
    {
        Player2Info(skinID, username);
    }

	public static string GetSkinID1()
	{
		return skinID1;
	}

	public static string GetSkinID2()
	{
		return skinID2;
	}

}
