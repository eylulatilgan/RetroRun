using UnityEngine;
using System.Collections;
using System;

public class SinglePlayerControls : MonoBehaviour {
    GameObject selectedSkin;
    Sprite selectedMissile;

    // Use this for initialization
    void Awake () {        
        selectedSkin = (GameObject)Resources.Load(PlayFabGameBridge.Instance.currentSkin.ItemId + "Single");
        Debug.Log(PlayFabGameBridge.Instance.currentSkin.ItemId+"Single");
        selectedMissile = Resources.Load<Sprite>(PlayFabGameBridge.Instance.currentMissile.ItemId) as Sprite;
        //selectedSkin.GetComponent<PlayerControls>().missileSkin = selectedMissile;

        Instantiate(selectedSkin, new Vector2(0, 0), Quaternion.identity);
    }
    
}
