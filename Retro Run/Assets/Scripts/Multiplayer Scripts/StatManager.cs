using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class StatManager : MonoBehaviour {

    void Start()
    {
        if (PlayFabData.AuthKey != null) LoadUserData();
        else PlayFabData.LoggedIn += LoadUserData;
    }

    void OnEnable()
    {
        FinishScript.OnFinished += updateStat;
    }

    void OnDisable()
    {
        FinishScript.OnFinished -= updateStat;
    }

    void updateStat(GameObject go)
    {
        if (gameObject == go)
        {
            UpdatePlayerData();            
        }        

	} 
    private void LoadUserData(string authKey = null)
    {
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, SetPlayerData, OnPlayFabError);
    }

    private void SetPlayerData(GetUserDataResult result)
    {

        Debug.Log("Player stats loaded");
        if (result.Data.ContainsKey("WonRaces"))
        {
            PlayFabGameBridge.Instance.wonRaces = int.Parse(result.Data["WonRaces"].Value);
        }

        if (result.Data.ContainsKey("TotalRaces"))
        {
            PlayFabGameBridge.Instance.totalRaces = int.Parse(result.Data["TotalRaces"].Value);
        }

        else
        {
            PlayFabGameBridge.Instance.wonRaces = 0;
            PlayFabGameBridge.Instance.totalRaces = 0;
        }        
    }
    private void UpdatePlayerData()
    {
        UpdateUserDataRequest req = new UpdateUserDataRequest();
        //AddUserVirtualCurrencyRequest currReq = new AddUserVirtualCurrencyRequest();        
        //currReq.Amount = PlayFabGameBridge.Instance.userBalance-PlayFabGameBridge.Instance.startUserBalance;

        Dictionary<string, string> playerData = new Dictionary<string, string>();
        playerData.Add("TotalRaces", PlayFabGameBridge.Instance.totalRaces.ToString());
        playerData.Add("WonRaces", PlayFabGameBridge.Instance.wonRaces.ToString());
        req.Data = playerData;
        req.Permission = UserDataPermission.Public;

        
        PlayFabClientAPI.UpdateUserData(req, OnUpdateSuccess, OnPlayFabError);
        //PlayFabClientAPI.curr(currReq, OnUpdateSuccess, OnPlayFabError);

        Dictionary<string, int> stats = new Dictionary<string, int>();
        stats.Add("WonRaces", PlayFabGameBridge.Instance.wonRaces);
        storeStats(stats);

    }

    public void storeStats(Dictionary<string, int> stats)
    {
        PlayFab.ClientModels.UpdateUserStatisticsRequest request = new PlayFab.ClientModels.UpdateUserStatisticsRequest();
        request.UserStatistics = stats;
        if (PlayFabData.AuthKey != null)
            PlayFabClientAPI.UpdateUserStatistics(request, StatsUpdated, OnPlayFabError);
    }

    private void StatsUpdated(UpdateUserStatisticsResult result)
    {
         Debug.Log("StatsUpdated");    
    }

    private void OnUpdateSuccess(UpdateUserDataResult result)
    {
        Debug.Log("OnUpdateSuccess");
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }
}
