using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
public class PlayerDataManagerScript : GenericSingleton<PlayerDataManagerScript> {
    public delegate void UpdateAction();
    public static event UpdateAction UpdateGrantedItems;

    private bool isRetrived = false;

    public void StartRetriving()
    {
        isRetrived = true;
    }

    protected override void Initialize()
    {
        if (PlayFabData.AuthKey != null) LoadUserData();
        else PlayFabData.LoggedIn += LoadUserData;
    }

    private void LoadUserData(string authKey = null)
    {
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, SetPlayerData, OnPlayFabError);
    }

    void SetPlayerData(GetUserDataResult result)
    {
        if (result.Data.Count == 0)
        {
            PlayFabGameBridge.Instance.totalRaces = 0;
            PlayFabGameBridge.Instance.wonRaces = 0;
            PlayFabGameBridge.Instance.currentSkin = PlayFabGameBridge.Instance.boughtSkins["S00"];
            PlayFabGameBridge.Instance.currentMissile = PlayFabGameBridge.Instance.boughtMissiles["M00"];

            UpdatePlayerData(PlayFabGameBridge.Instance.currentSkin, PlayFabGameBridge.Instance.currentMissile);
        }
        else
        {
            PlayFabGameBridge.Instance.currentSkin = PlayFabGameBridge.Instance.boughtSkins[result.Data["Skin"].Value];
            PlayFabGameBridge.Instance.currentMissile = PlayFabGameBridge.Instance.boughtMissiles[result.Data["Missile"].Value];
            PlayFabGameBridge.Instance.totalRaces = int.Parse(result.Data["TotalRaces"].Value);
            PlayFabGameBridge.Instance.wonRaces = int.Parse(result.Data["WonRaces"].Value);
        }        

    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {        
        Debug.Log("Player data successfully retrived");
    }

    void UpdatePlayerData(ItemInstance skin, ItemInstance missile)
    {
        UpdateUserDataRequest request         = new UpdateUserDataRequest();
        Dictionary<string, string> playerData = new Dictionary<string, string>();

        playerData.Add(skin.ItemClass, skin.ItemId);
        playerData.Add(missile.ItemClass, missile.ItemId);
        playerData.Add("TotalRaces", PlayFabGameBridge.Instance.totalRaces.ToString());
        playerData.Add("WonRaces", PlayFabGameBridge.Instance.wonRaces.ToString());
        
        request.Data = playerData;
        request.Permission = UserDataPermission.Public;

        PlayFabClientAPI.UpdateUserData(request, OnAddDataSuccess, OnPlayFabError);
    }

    private void OnAddDataSuccess(UpdateUserDataResult result)
    {
        UpdateGrantedItems();
        Debug.Log("Granted items are succesfully sent to PlayFab");
    }


    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }
	
}
