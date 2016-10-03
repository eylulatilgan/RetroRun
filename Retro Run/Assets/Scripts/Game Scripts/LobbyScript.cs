using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour {

	public Image panelPlayer1;
	public Image panelPlayer2;
    public Image imagePlayer1;
    public Image imagePlayer2;
    public Text namePlayer1;
    public Text namePlayer2;

	public string menu = "Menu";
	public string gameLevel = "MainOn";

    public Button readyButton;
	
	public GameObject loadingScreen;
	public Text loadingText;

    public delegate void CheckReadyButton(bool isReady);
    public static event CheckReadyButton On1Ready, On2Ready;

	public bool isPlayer1Ready = false;
	public bool isPlayer2Ready = false;
	private bool isLoading      = true;


    void OnEnable()
    {
        PhotonConnectionScript.Player1StatusReady += Assign1Ready;
        PhotonConnectionScript.Player2StatusReady += Assign2Ready;

        PhotonConnectionScript.Player1Info += SetPanelPlayer1;
        PhotonConnectionScript.Player2Info += SetPanelPlayer2;

        PhotonConnectionScript.ActivateReadyButton += ActivateReadyBtn;

		PhotonConnectionScript.OnLobbyJoin += CloseLoading;
		PhotonConnectionScript.OnConnectionStatus += ConnectionText;
    }

	void OnDisable()
	{
		PhotonConnectionScript.Player1StatusReady -= Assign1Ready;
		PhotonConnectionScript.Player2StatusReady -= Assign2Ready;
		
		PhotonConnectionScript.Player1Info -= SetPanelPlayer1;
		PhotonConnectionScript.Player2Info -= SetPanelPlayer2;
		
		PhotonConnectionScript.ActivateReadyButton -= ActivateReadyBtn;
		
		PhotonConnectionScript.OnLobbyJoin -= CloseLoading;
		PhotonConnectionScript.OnConnectionStatus -= ConnectionText;
	}

	void Update()
	{
		if(isPlayer1Ready)
		{
			panelPlayer1.color = Color.green;
            isPlayer1Ready = true;
		}
		else
		{
			panelPlayer1.color = Color.red;
		}

		if(isPlayer2Ready)
		{
			panelPlayer2.color = Color.green;
            isPlayer2Ready = true;
		}
		else
		{
			panelPlayer2.color = Color.red;
		}

        if (isPlayer1Ready && isPlayer2Ready)
        {
            CatalogScript.catalogLoaded = false;
            Application.LoadLevel(gameLevel);
		}

		if (isLoading && Input.GetKeyDown(KeyCode.Escape))
		{            
			Application.LoadLevel("Login");
		}

	}

    void SetPanelPlayer1(string skinID1, string username1)
    {
        imagePlayer1.sprite = Resources.Load<Sprite>("SkinImages/" + skinID1) as Sprite;
        imagePlayer1.color = new Color(1f, 1f, 1f, 1f);
        namePlayer1.text = username1;
    }

    void SetPanelPlayer2(string skinID2, string username2)
    {
        imagePlayer2.sprite = Resources.Load<Sprite>("SkinImages/" + skinID2) as Sprite; 
        imagePlayer2.color = new Color(1f, 1f, 1f, 1f);
        namePlayer2.text = username2;
    }

    void Assign1Ready(bool is1Ready)
    {
        isPlayer1Ready = is1Ready;
    }

    void Assign2Ready(bool is2Ready)
    {
        isPlayer2Ready = is2Ready;
    }

	public void PlayerReady()
	{
        if (PhotonNetwork.isMasterClient)
        {
            if (!isPlayer1Ready)
            {
                isPlayer1Ready = true;
                Debug.Log("Player1Ready = " + isPlayer1Ready);
                On1Ready(isPlayer1Ready);
            }
            else
            {
                isPlayer1Ready = false;
                Debug.Log("Player1Ready = " + isPlayer1Ready);
                On1Ready(isPlayer1Ready);
            }
        }
        else
        {
            if (!isPlayer2Ready)
            {
                isPlayer2Ready = true;
                Debug.Log("Player2Ready = " + isPlayer2Ready);
                On2Ready(isPlayer2Ready);
            }
            else
            {
                isPlayer2Ready = false;
                Debug.Log("Player2Ready = " + isPlayer2Ready);
                On2Ready(isPlayer2Ready);
            }
        }
    }

	public void GoToMenu()
	{
        if (PhotonNetwork.isMasterClient)
        {
            isPlayer1Ready = false;
            Debug.Log("Player1Ready = " + isPlayer1Ready);
            On1Ready(isPlayer1Ready);
        }
        else
        {
            isPlayer2Ready = false;
            Debug.Log("Player2Ready = " + isPlayer2Ready);
            On2Ready(isPlayer2Ready);
        }

        CatalogScript.catalogLoaded = false;
        Application.LoadLevel(menu);
	}

    void ActivateReadyBtn()
    {
        readyButton.interactable = true;
    }

	
	public void CloseLoading ()
	{
		loadingScreen.SetActive(false);
		isLoading = false;
	}
	
	public void ShowLoading ()
	{
		loadingScreen.SetActive(true);
		isLoading = true;
	}

	public void ConnectionText(string connectionStatus)
	{
		loadingText.text = connectionStatus;
	}

}
