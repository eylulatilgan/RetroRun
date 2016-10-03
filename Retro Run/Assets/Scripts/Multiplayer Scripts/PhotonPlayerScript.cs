using UnityEngine;
using System.Collections;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PhotonPlayerScript : PunBehaviour {

	public Image minimapChar;

	public Transform startZone;

    public delegate void StartCounterAction();
	public static event StartCounterAction OnStartCounter;
	public delegate void JoinedRoomAction(string minimapSkin, GameObject go, bool isMaster);
	public static event JoinedRoomAction InitMinimap;

	public GameObject selectedSkin;
	private Sprite selectedMissile;
	
	private GameObject playerPrefab;
    
    private void GetSelectedItems()
	{
		Debug.Log("Getting player items...");
        Debug.Log(PlayFabGameBridge.Instance.currentSkin.ItemId);
        playerPrefab = Resources.Load(PlayFabGameBridge.Instance.currentSkin.ItemId) as GameObject;
        Debug.Log(PlayFabGameBridge.Instance.currentMissile.ItemId);
        selectedMissile = Resources.Load<Sprite>(PlayFabGameBridge.Instance.currentMissile.ItemId) as Sprite;
	}

    void Start()
    {

        photonView.RPC("RPCOnStartCounter", PhotonTargets.AllBuffered);

        GetSelectedItems();
        selectedSkin = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(startZone.position.x, startZone.position.y, 0f), Quaternion.identity, 0);
        selectedSkin.GetComponent<PlayerControls>().missileSkin = selectedMissile;
        CameraControllerOnline cameraController = selectedSkin.GetComponent<CameraControllerOnline>();
        cameraController.enabled = true;
        selectedSkin.GetComponent<Rigidbody2D>().isKinematic = false;
		if (PhotonNetwork.isMasterClient)
		{
			InitMinimap(PlayFabGameBridge.Instance.currentSkin.ItemId, selectedSkin, true);
		}
		else
		{
			InitMinimap(PlayFabGameBridge.Instance.currentSkin.ItemId, selectedSkin, false);
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
		PlayerControls controller = selectedSkin.GetComponent<PlayerControls>();
        controller.enabled = true;
    }

}






