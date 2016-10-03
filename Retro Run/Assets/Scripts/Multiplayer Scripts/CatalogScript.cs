using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;
using PlayFab.Serialization.JsonFx;
public class CatalogScript : GenericSingleton<CatalogScript>
{
    public List<CatalogItem> items;
    public delegate void MarketAction();
    public static event MarketAction InitMarketItems;

    public static bool catalogLoaded;
    protected override void Initialize()
    {
        catalogLoaded = false;
        PlayFabGameBridge.Instance.skins = new Dictionary<string, Skin>();
        PlayFabGameBridge.Instance.missiles = new Dictionary<string, Missile>();
    }

    private void LoadMarketMenu(string authKey = null)
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion         = PlayFabData.CatalogVersion;
        PlayFabClientAPI.GetCatalogItems(request, ConstructCatalog, OnPlayFabError);        
    }

    public void CreateCatalog()
    {
        if (PlayFabData.AuthKey != null)
            LoadMarketMenu();
        else
            PlayFabData.LoggedIn += LoadMarketMenu;
    }

    private void ConstructCatalog(GetCatalogItemsResult result) 
    {

        items = result.Catalog;

        //adding default skin and missile        
        PlayFabGameBridge.Instance.skins.Add(PlayFabGameBridge.Instance.defaultSkin.skinID, PlayFabGameBridge.Instance.defaultSkin);
        PlayFabGameBridge.Instance.missiles.Add(PlayFabGameBridge.Instance.defaultMissile.missileID, PlayFabGameBridge.Instance.defaultMissile);

        //Now, inserting items to dictionaries
        for (int i = 0; i < items.Count; i++)
        {           
            Dictionary<string, uint> priceList = items[i].VirtualCurrencyPrices;

            if (items[i].ItemClass.StartsWith("Skin") && !PlayFabGameBridge.Instance.skins.ContainsKey(items[i].ItemId))
            {               
                //Creating a new skin object
                Skin newSkin = new Skin { skinID   = items[i].ItemId, 
                                          skinName = items[i].DisplayName,  
                                          price    = priceList["GC"]};


                PlayFabGameBridge.Instance.skins.Add(items[i].ItemId, newSkin);             
            }

            if (items[i].ItemClass.StartsWith("Missile") && !PlayFabGameBridge.Instance.missiles.ContainsKey(items[i].ItemId))
            {
                Missile newMissile = new Missile { missileID   = items[i].ItemId,
                                                   missileName = items[i].DisplayName,  
                                                   price       = priceList["GC"]};
                PlayFabGameBridge.Instance.missiles.Add(items[i].ItemId, newMissile);
            }

        }

        if (!catalogLoaded)
        {
            Debug.Log("CatalogLoaded" + catalogLoaded);
            InitMarketItems(); 
        }

         catalogLoaded = true;        
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }
}
