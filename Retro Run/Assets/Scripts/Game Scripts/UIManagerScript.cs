using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour {

	public Animator startButton;
	public Animator settingsButton;
	public Animator settingsPanel;
	public Animator contentPanel;
	public Animator gearImage;
	public Animator aboutPanel;
	public Animator leaderboardPanel;
	public Animator menuPanel;
	public Animator marketPanel;

	public GameObject loadingScreen;

	private bool isLoading;

	void Start() 
	{
		isLoading = true;

		RectTransform transform = contentPanel.gameObject.transform as RectTransform;        
		Vector2 position = transform.anchoredPosition;
		position.y -= transform.rect.height;
		transform.anchoredPosition = position;
	}

	void Update()
	{
		if (isLoading && Input.GetKeyDown(KeyCode.Escape))
		{
            SceneManager.LoadScene("Login");
		}
	}

	public void OpenSettings() {
		
		settingsPanel.enabled = true;
		settingsPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		contentPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
	}

	public void CloseSettings() {

		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
	}

	public void OpenAbout() {
		
		aboutPanel.enabled = true;
		aboutPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
		contentPanel.SetBool("isHidden", true);
	}
	
	public void CloseAbout() {
		
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
	}
	
	public void OpenLeaderboard() {
		
		leaderboardPanel.enabled = true;
		leaderboardPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
		contentPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
	}

	public void CloseLeaderboard() {
		
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
	}

	public void ToggleMenu() {

		contentPanel.enabled = true;
		bool isHidden = contentPanel.GetBool("isHidden");
		contentPanel.SetBool("isHidden", !isHidden);

		gearImage.enabled = true;
		gearImage.SetBool("isHidden", !isHidden);
	}

	
	public void StartGame() {

        CatalogScript.catalogLoaded = false;
		SceneManager.LoadScene("Lobby&scene");   
     
	}

    public void StartSinglePlayerGame()
    {
        CatalogScript.catalogLoaded = false;
        SceneManager.LoadScene("SinglePlayerOffline");
    }

	public void GoToMarket() {

		//Application.LoadLevel(marketPlace);

		menuPanel.enabled = true;
		marketPanel.enabled = true;
		marketPanel.SetBool("isHidden", false);
		menuPanel.SetBool("isHidden", true);
		aboutPanel.SetBool("isHidden", true);
		settingsPanel.SetBool("isHidden", true);
		leaderboardPanel.SetBool("isHidden", true);
		gearImage.SetBool("isHidden", true);
	}
	
	public void GoToMenu() {
		
		//Application.LoadLevel(menuScene);
		
		menuPanel.enabled = true;
		marketPanel.enabled = true;
		marketPanel.SetBool("isHidden", true);
		menuPanel.SetBool("isHidden", false);
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		contentPanel.SetBool("isHidden", true);
	}
	
	void OnEnable()
	{
		MarketItemScript.InitializeDone += CloseLoading;
	}
	
	void OnDisable()
	{
		MarketItemScript.InitializeDone -= CloseLoading;
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

	public void LogOut() {
        CatalogScript.catalogLoaded = false;
        PlayerPrefs.DeleteKey("prevUsername");
        PlayerPrefs.DeleteKey("prevPassword");

        Debug.Log("PlayerPrefs deleted.");
        SceneManager.LoadScene("Login");
	}
}
