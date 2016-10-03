using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Multiplayer : PunBehaviour {
    public GameObject player1Prefab;
	public GameObject player2Prefab;
	public GameObject lobbyScreen;
	public GameObject createRoomPanel;
    public GameObject roomPanel;
    public GameObject charPanel;
    public GameObject ListContent;
    public delegate void LobbyAction(Room room);
    public static event LobbyAction showMyCharSprite;

    GameObject player;
    GameObject selectedSkin;
    Sprite selectedMissile;
    
	bool isTimerStarted = false;

    private List<string> serverList;
    public Slider maxRoomSlider;
    public InputField roomNameInput;

	private HUDandUIManager manager;

	public delegate void JoinedRoomAction(bool isMaster);
	public static event JoinedRoomAction InitMinimap;
    public delegate void StartCounterAction();
    public static event StartCounterAction OnStartCounter;

    void Awake()
    {        
        serverList = new List<string>();
        manager = GameObject.Find("GameController").GetComponent<HUDandUIManager>();

    }

	void Start () {

        
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
    
    //void OnJoinedLobby()
    //{
    //    PopulateServerList();
    //}

    public void CreateCustomRoom()
    {
        RoomOptions options = new RoomOptions();
        options.maxPlayers = byte.Parse(maxRoomSlider.value.ToString());       
        PhotonNetwork.CreateRoom(roomNameInput.text, options, null);
    }

    public void ConnectRoom()
    {
        Text text = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>();
		Debug.Log(text.text);
		PhotonNetwork.JoinRoom(text.text);
    }


    void OnJoinedRoom() {

        Debug.Log("Ready for action!");
        showMyCharSprite(PhotonNetwork.room);
        
    }

    public void PlayTheGame() { 
        lobbyScreen.SetActive(false);
        manager.ShowUI();
		createRoomPanel.SetActive(false);   
        charPanel.SetActive(false);

        Debug.Log(PlayFabGameBridge.Instance.currentSkin.ItemId);
        selectedSkin = (GameObject)Resources.Load(PlayFabGameBridge.Instance.currentSkin.ItemId);
        selectedMissile = Resources.Load<Sprite>(PlayFabGameBridge.Instance.currentMissile.ItemId) as Sprite;
        selectedSkin.GetComponent<PlayerControls>().missileSkin = selectedMissile;

        player = PhotonNetwork.Instantiate(selectedSkin.name, new Vector2(0, 0), Quaternion.identity, 0);
        
		player.GetComponentInChildren<Camera>().enabled = true;
		Transform MainCamera = player.transform.FindChild("Main Camera");
		MainCamera.FindChild("backg0").GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().isKinematic = false;
        //player.GetComponent<PlayerControls>().enabled = true;

        photonView.RPC("RPCOnStartCounter", PhotonTargets.AllBuffered);
    }

	void Update(){
		
		if(PhotonNetwork.room.playerCount == 3 && !isTimerStarted){
			photonView.RPC("RPCOnStartCounter", PhotonTargets.AllBuffered);
			isTimerStarted = true;
		}
	}

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was selected");
    }

    public void PopulateServerList()
    {    
        RoomInfo[] hostData = PhotonNetwork.GetRoomList();

        Debug.Log(hostData.Length);

        if (hostData.Length > 0)
        {
            for (int i = 0; i < hostData.Length; i++)
            {

                if (!serverList.Contains(hostData[i].name)) 
                {
                    serverList.Add(hostData[i].name);
                    GameObject roomText = (GameObject)Instantiate(roomPanel);
                    roomText.GetComponentInChildren<Text>().text = hostData[i].name;
                    //roomList.Add(roomText);

                    roomText.transform.SetParent(ListContent.transform, false);
                    roomText.SetActive(true); 
                }               
            }
        }
    }

    [PunRPC]
    void RPCOnStartCounter()
    {
        OnStartCounter();
    }

    void OnEnable()
    {
        Timer.StartGame += EnableController;
    }

    void OnDisable()
    {
        Timer.StartGame -= EnableController;
    }

    void EnableController()
    {
        photonView.RPC("RPCEnableController", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    void RPCEnableController()
    {
        PlayerControls controller = player.GetComponent<PlayerControls>();
        controller.enabled = true;
    }
}
