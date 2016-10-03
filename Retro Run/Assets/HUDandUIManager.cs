using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDandUIManager : MonoBehaviour
{

	public Text goldText;
    public Text missileText;

	public int missile = 1;

	public GameObject createRoomPanel;
	public InputField roomNameInput;
	public Slider maxRoomSlider;
	public Text roomSliderText;


    void OnEnable()
    {
		PlayerControls.OnCollectGold 	+= ShowScore;
		PlayerControls.OnCollectMissile += ShowMissile;
		MissileBoxScript.OnDonate += AddFuse;
		PlayerControls.OnMissiled += DecrFuse;
    }

    void OnDisable()
	{
		PlayerControls.OnCollectGold 	-= ShowScore;
		PlayerControls.OnCollectMissile -= ShowMissile;
		MissileBoxScript.OnDonate -= AddFuse;
		PlayerControls.OnMissiled -= DecrFuse;
    }

    void ShowScore(int gold)
    {
		goldText.text = "Gold: " + gold;
	}

	void ShowMissile(int missile)
    {
        missileText.text = "Missile: " + missile;
    }

	public void ReturnToMenu() 
	{
		Application.LoadLevel("Menu");
	}

	public void ShowCreateRoomPanel()
	{
		createRoomPanel.SetActive(true);
	}

	public void HideCreateRoomPanel()
	{
		createRoomPanel.SetActive(false);
	}

	public void ChangeSliderNumder()
	{
		roomSliderText.text = maxRoomSlider.value.ToString();
	}    

	public void ShowUI()
	{
		goldText.gameObject.SetActive(true);
		missileText.gameObject.SetActive(true);
	}

	public void HideUI()
	{
		goldText.gameObject.SetActive(false);
		missileText.gameObject.SetActive(false);
	}

	void AddFuse(GameObject go)
	{

		missile++;
		missileText.text = "Missile: " + missile;
	}

	void DecrFuse(GameObject go)
	{

		missile--;
		missileText.text = "Missile: " + missile;
	}
}
