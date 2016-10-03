using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour {
    
    //public AudioClip goldSound;
    private Text goldtext;

    private int gold = 0;

    void Start()
    {
        goldtext = GameObject.Find("txtGold").GetComponent<Text>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {/*
		if (other.tag == "Gold")
        {
            gold++;

            goldtext.text = "Gold: " + gold; 

			PlayFabGameBridge.Instance.userBalance += 1;

            //AudioSource.PlayClipAtPoint(goldSound, transform.position);
        }
        Debug.Log(PlayFabGameBridge.Instance.userBalance);
        */
    }		

	void OnEnable()
	{
		GoldScript.OnGoldCollect += AddGolder;
	}
	void OnDisable()
	{
		GoldScript.OnGoldCollect -= AddGolder;
	}

	void AddGolder(GameObject go) 
	{
		if (gameObject == go)
		{
			gold++;

			goldtext.text = "Gold: " + gold; 

			PlayFabGameBridge.Instance.userBalance += 1;
		}

	}
}
