using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using PlayFab.ClientModels;
using PlayFab;
public class MarketManagerScript : MonoBehaviour {

	public Text playerNameText;
    public Text goldAmmountText;
    public Text purchaseErrorText;

	public GameObject skinPanel;
	public GameObject missilePanel;
	public GameObject otherPanel;
	public GameObject errorPanel;
	public GameObject loadingScreen;
	
	public Scrollbar scrollbarSkin;
	public Scrollbar scrollbarMissile;

    public static CatalogItem selectedSkin;
    public static CatalogItem selectedMissile;
	
	private bool isLoading;
    private static bool isAllExecuted;

    private static int ExecutionOrder;

    public delegate void MenuAction();
    public static event MenuAction showMenuAgain;
	void Start() {
		
		isLoading = false;
        PlayFabClientAPI.GetUserCombinedInfo(new GetUserCombinedInfoRequest(), OnGetUserCurrency, OnPlayFabError);
        playerNameText.text = LoginScript.getUsername();

        if (isAllExecuted)
        {
            showMenuAgain();
            isAllExecuted = false;
        }
        else
        {            
            ExecutionOrder = 0;
            InitializationNext();           
        }
        
	}

    private void OnGetUserCurrency(GetUserCombinedInfoResult result)
    {
        PlayFabGameBridge.Instance.userBalance = result.VirtualCurrency["GC"];
        PlayFabGameBridge.Instance.startUserBalance = result.VirtualCurrency["GC"];
        goldAmmountText.text = PlayFabGameBridge.Instance.userBalance.ToString("0000") + " G";
       
    } 

	public void ShowSkinPanel() {
		
		missilePanel.SetActive(false);
		otherPanel  .SetActive(false);
		skinPanel   .SetActive(true );
		scrollbarSkin.value = 0;
	}

	public void ShowMissilePanel() {
		
		skinPanel   .SetActive(false);
		otherPanel  .SetActive(false);
		missilePanel.SetActive(true );
		scrollbarMissile.value = 0;
	}

	public void ShowOtherPanel() {
		
		skinPanel   .SetActive(false);
		missilePanel.SetActive(false);
		otherPanel  .SetActive(true );
	}
    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }

    void OnEnable() 
    {
        InitializationEventManager.OnNextInitialization += InitializationNext;
        PurchaseScript.OnError += GiveError;       
		InventoryScript.UpdateGoldAmmount += UpdateGoldAmmount;
		PurchaseScript.OnPurchaseProcess += ShowLoading;
		InventoryScript.PurchaseDone += CloseLoading;
    }

    void OnDisable()
    {
        PurchaseScript.OnError -= GiveError;
        InventoryScript.UpdateGoldAmmount -= UpdateGoldAmmount;
		InitializationEventManager.OnNextInitialization -= InitializationNext;
		PurchaseScript.OnPurchaseProcess -= ShowLoading;
		InventoryScript.PurchaseDone -= CloseLoading;
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

    void InitializationNext()
    {
        switch (ExecutionOrder)
        {
            case 0:
                InventoryScript.Instance.CreateInventory();
                break;
            case 1:
                CatalogScript.Instance.CreateCatalog();
                break;            
            case 2:
                PlayerDataManagerScript.Instance.StartRetriving();
                isAllExecuted = true;
                break;
            default:
                break;
        }

        ExecutionOrder++;
    }

    void GiveError(string error)
    {
        errorPanel.SetActive(true);
        purchaseErrorText.text = error;
    }


    void UpdateGoldAmmount()
    {
        goldAmmountText.text = "Gold\n " + PlayFabGameBridge.Instance.userBalance.ToString("0000");
    }

    public void CloseErrorPanel()
    {
        errorPanel.SetActive(false);
    }

}
