using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;

public class MiniMapScript : PunBehaviour {

	public Transform startZone;
	public Transform finishZone;
	
	public Transform parent;
	public float slideWidth = 0.1f;

	
	public Image mmChar1;
	public Image mmChar2;
	
	public GameObject target1;
	public GameObject target2;

	private float mapEnd;
	private float mapStart;
	private float mapLenght;
	private float mapPlayer1;
	private float mapPos1;
	private float mapPlayer2;
	private float mapPos2;
	
	private bool is1Here;
	private bool is2Here;

	Vector2 slideAnchorMin1;
	Vector2 slideAnchorMax1;
	Vector2 slideAnchorMin2;
	Vector2 slideAnchorMax2;
		
	void OnEnable(){
		PhotonPlayerScript.InitMinimap += InitializeMiniMapChar;
	}
	
	void Start() {

		mapStart = startZone.position.x;
		mapEnd   = finishZone.position.x;

		mapLenght = mapEnd - mapStart;



		mmChar1.rectTransform.anchorMin = slideAnchorMin1;
		mmChar1.rectTransform.anchorMax = slideAnchorMax1;
		mmChar2.rectTransform.anchorMin = slideAnchorMin2;
		mmChar2.rectTransform.anchorMax = slideAnchorMax2;
		
		slideAnchorMin1.y = -0.2f;
		slideAnchorMax1.y = 1.2f;
		slideAnchorMin2.y = -0.2f;
		slideAnchorMax2.y = 1.2f;

	}

	void Update() {

		if(is1Here)
		{
			mapPlayer1 = target1.transform.position.x;
			mapPos1 = (mapPlayer1 - mapStart) / mapLenght;

			slideAnchorMin1.x = mapPos1 - slideWidth/2;
			slideAnchorMax1.x = mapPos1 + slideWidth/2;
			
			mmChar1.rectTransform.anchorMin = slideAnchorMin1;
			mmChar1.rectTransform.anchorMax = slideAnchorMax1;

		}
		if(is2Here)
		{
			mapPlayer2 = target2.transform.position.x;
			mapPos2 = (mapPlayer2 - mapStart) / mapLenght;

			slideAnchorMin2.x = mapPos2 - slideWidth/2;
			slideAnchorMax2.x = mapPos2 + slideWidth/2;
			
			mmChar2.rectTransform.anchorMin = slideAnchorMin2;
			mmChar2.rectTransform.anchorMax = slideAnchorMax2;
		}
	}

	void InitializeMiniMapChar(string imgID, GameObject go, bool isMaster) {


		if(isMaster)
		{
			target1 = go;
			mmChar1.sprite = Resources.Load<Sprite>("SkinImages/" + imgID) as Sprite;
			is1Here = true;
		}
		else
		{
			target2 = go;
			mmChar2.sprite = Resources.Load<Sprite>("SkinImages/" + imgID) as Sprite;
			is2Here = true;

		}
	}
}