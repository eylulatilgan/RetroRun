using UnityEngine;
using System.Collections;

public class MissileBoxScript : MonoBehaviour {


	private bool isTriggered = false;
	public Sprite triggeredBox;

	private Collider2D player;

	public delegate void DonateAction(GameObject go);
	public static event DonateAction OnDonate; 

	SpriteRenderer sprRend;

	void Awake()
	{
		sprRend = GetComponent<SpriteRenderer>(); 
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(!isTriggered && other.tag == "Player") {

			player = other; 
			OnDonate(player.gameObject);

			isTriggered = true;
			sprRend.sprite = triggeredBox;
		}
	}

}
