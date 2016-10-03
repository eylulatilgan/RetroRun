using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
public class Missile
{
    public string missileID;
    public string missileName;
    public Sprite missileSprite;
    public bool isBought;
    public uint price;
}

public class Skin
{
    public string skinID;
    public string skinName;
    public Sprite skinSprite;    
    public bool isBought;
    public uint price;
}

public class PlayFabGameBridge : GenericSingleton<PlayFabGameBridge> {
    public int wonRaces;
    public int totalRaces;
    public int userBalance;
    public int startUserBalance;

    public Dictionary<string, Skin>    skins;
    public Dictionary<string, Missile> missiles;

    public ItemInstance currentSkin;
    public ItemInstance currentMissile;
	
	public Skin    defaultSkin    = new Skin    { skinID    = "S00", skinName    = "Green Skin",       price = 0, isBought = true};
    public Missile defaultMissile = new Missile { missileID = "M00", missileName = "Shuriken Missile", price = 0, isBought = true};

    public Dictionary<string, ItemInstance> boughtSkins;
    public Dictionary<string, ItemInstance> boughtMissiles;

}
