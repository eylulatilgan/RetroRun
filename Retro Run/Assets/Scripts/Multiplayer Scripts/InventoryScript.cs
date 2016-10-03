using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab;
public class InventoryScript : GenericSingleton<InventoryScript>
{    
    public static List<ItemInstance> inventory;
    public static bool InventoryLoaded = false;

    public delegate void UpdateAction();
    public static event UpdateAction UpdateGoldAmmount;	
	public static event UpdateAction PurchaseDone;
        
    protected override void Initialize()
    {
        PlayFabGameBridge.Instance.boughtSkins = new Dictionary<string, ItemInstance>();
        PlayFabGameBridge.Instance.boughtMissiles = new Dictionary<string, ItemInstance>();
        
    }

    public void CreateInventory()
    {
        if 
            (PlayFabData.AuthKey != null) UpdateInventory();
        else
            PlayFabData.LoggedIn += UpdateInventory;        
    }

    public void UpdateInventory(string authKey = null)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnUpdateUserInventory, OnPlayFabError);        
    }
    

    public void OnUpdateUserInventory(GetUserInventoryResult result) 
    {      
        inventory = result.Inventory;

        if (inventory.Count == 0)
        {
            Debug.Log("GrantItemAtInventory");
            GrantItems();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            ItemInstance itemInstance = inventory[i];

            if (itemInstance.ItemClass == "Skin" && !PlayFabGameBridge.Instance.boughtSkins.ContainsKey(itemInstance.ItemId))
            {
                PlayFabGameBridge.Instance.boughtSkins.Add(itemInstance.ItemId, itemInstance);
            }
            else if (itemInstance.ItemClass == "Missile" && !PlayFabGameBridge.Instance.boughtMissiles.ContainsKey(itemInstance.ItemId))
            {
                PlayFabGameBridge.Instance.boughtMissiles.Add(itemInstance.ItemId, itemInstance);
            }
        }

        if (inventory.Count == (PlayFabGameBridge.Instance.boughtSkins.Count + PlayFabGameBridge.Instance.boughtMissiles.Count))
        {
            InventoryLoaded = true;	
			PurchaseDone();
        }

        if (InventoryLoaded)
        {
            InitializationEventManager.OnNextInitialization();
        }  

    }
    
    void GrantItems()
    {
        string defaultSkinID = PlayFabGameBridge.Instance.defaultSkin.skinID;
        string defaultMissileID = PlayFabGameBridge.Instance.defaultMissile.missileID;
        int defaultSkinPrice = (int)PlayFabGameBridge.Instance.defaultSkin.price;
        int defaultMissilePrice = (int)PlayFabGameBridge.Instance.defaultMissile.price;

        PurchaseItemRequest skinRequest = new PurchaseItemRequest();
        skinRequest.CatalogVersion = "RetroRun";
        skinRequest.VirtualCurrency = "GC";
        skinRequest.Price = defaultSkinPrice;
        skinRequest.ItemId = defaultSkinID;

        PlayFabClientAPI.PurchaseItem(skinRequest, InventoryScript.Instance.OnPurchase, OnPlayFabError);

        PurchaseItemRequest missileRequest = new PurchaseItemRequest();
        missileRequest.CatalogVersion = "RetroRun";
        missileRequest.VirtualCurrency = "GC";
        missileRequest.Price = defaultMissilePrice;
        missileRequest.ItemId = defaultMissileID;

        PlayFabClientAPI.PurchaseItem(missileRequest, InventoryScript.Instance.OnPurchase, OnPlayFabError);
        
    }
        
    public void OnPurchase(PurchaseItemResult result)
    {
        PlayFabGameBridge.Instance.userBalance -= (int)result.Items[0].UnitPrice;
        UpdateGoldAmmount();
        InventoryScript.Instance.UpdateInventory();        
    }
    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }

}
