using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{


    public Text fuseText;
    public int missile = 1;

    private float time;
    private float startTime = 5f;

    private bool isFinished = false;

    void OnEnable()
    {

        //GoldManager.OnTriggered += ShowScore;
        MissileBoxScript.OnDonate += AddFuse;
        PlayerControls.OnMissiled += DecrFuse;
    }

    void OnDisable()
    {

        //GoldManager.OnTriggered -= ShowScore;
        MissileBoxScript.OnDonate -= AddFuse;
        PlayerControls.OnMissiled -= DecrFuse;
    }

    //void ShowScore(int gold)
    //{
    //    goldtext.text = "Gold: " + gold;
    //}

    void AddFuse(GameObject go)
    {

        missile++;
        fuseText.text = "Missile: " + missile;
    }

    void DecrFuse(GameObject go)
    {

        missile--;
        fuseText.text = "Missile: " + missile;
    }

}
