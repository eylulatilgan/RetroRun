using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LobbyManager : Photon.MonoBehaviour {

    public GameObject lobbyScreen;
    public GameObject createRoomPanel;
    public GameObject roomPanel;
    public GameObject charPanel;

    public GameObject charSpritePanel;
    public Transform charImageContentParent;
    public GameObject charImagePanel;

    public Sprite greenPlayer;
    public Sprite redPlayer;

    public List<Sprite> charSprites = new List<Sprite>();
    public List<GameObject> charImgPnlList = new List<GameObject>();

    private int ownOrder = 0;
    private GameObject charImgPnl;
    GameObject currentSkin;
    string currentSkinID;
    Sprite currentSkinSprite;

    List<GameObject> characterPanels;

    int index;

    void OnEnable()
    {
        Multiplayer.showMyCharSprite += showCharSprite;
    }

    void OnDisable()
    {
        Multiplayer.showMyCharSprite -= showCharSprite;
    }

    public void showCharSprite(Room room)
    {
        currentSkinID = PlayFabGameBridge.Instance.currentSkin.ItemId;
        currentSkin = (GameObject)Resources.Load(currentSkinID);
        currentSkinSprite = currentSkin.GetComponent<SpriteRenderer>().sprite;

        lobbyScreen.SetActive(false);

        charPanel.SetActive(true);
        charSpritePanel.SetActive(true);

        characterPanels = new List<GameObject>();
        
            charImgPnl = Instantiate(charImagePanel) as GameObject;
            charImgPnl.gameObject.name = "Character ";
            charImgPnl.transform.SetParent(charImageContentParent);
            characterPanels.Add(charImgPnl);
        


        foreach (var panel in characterPanels)
        {
            if(panel.transform.GetChild(0).GetComponent<Image>().sprite == null)
            {
                panel.transform.GetChild(0).GetComponent<Image>().sprite = currentSkinSprite;
                index = characterPanels.IndexOf(panel);
                return;                            
            }            
        }

        //send rpc
        
        this.photonView.RPC("showMyChar", PhotonTargets.AllBuffered, currentSkinID);

        charSprites.Add(greenPlayer);
        ownOrder = charSprites.Capacity - 1; 

    }

    [PunRPC]
    void showMyChar(string skinID)
    {
        Debug.Log("Heya imma new player desu!");
        Debug.Log(skinID);

        //GameObject otherSkin = (GameObject)Resources.Load(skinID);
        //Sprite otherSkinSprite = otherSkin.GetComponent<SpriteRenderer>().sprite;

        //foreach (var panel in characterPanels)
        //{
        //    if (panel.transform.GetChild(0).GetComponent<Image>().sprite == null)
        //    {
        //        panel.transform.GetChild(0).GetComponent<Image>().sprite = otherSkinSprite;
        //        return;
        //    }
        //}
    }
}
