using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public delegate void StartAction();
    public static event StartAction StartGame;

    private bool startCountdownWhenTimeIsSynced;
    private const string StartTimeKey = "st";
    public double StartTime;
    public bool startCountdown = false;
    bool timerStarted;

    public Text timeText;
    public Text startTimeText;

    double elapsedTime;
    double remainingTime;
    void OnEnable()
    {
        Multiplayer.OnStartCounter += StartCounter;
    }

    void OnDisable()
    {
        Multiplayer.OnStartCounter -= StartCounter;
    }

    public void StartCounter()
    {
        startCountdown = true;
        this.StartRoundNow();
    }

    private void StartRoundNow()
    {

        if (PhotonNetwork.time < 0.0001f)
        {
            startCountdownWhenTimeIsSynced = true;
            return;
        }
        startCountdownWhenTimeIsSynced = false;

        ExitGames.Client.Photon.Hashtable startTimeProp = new Hashtable();
        startTimeProp[StartTimeKey] = PhotonNetwork.time;
        PhotonNetwork.room.SetCustomProperties(startTimeProp);

    }

    public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartTimeKey))
        {
            StartTime = (double)propertiesThatChanged[StartTimeKey];
        }
    }

    void Update()
    {
        if (startCountdown)
        {
            elapsedTime = (double)(PhotonNetwork.time - StartTime);
            
           
            if (startTimeText.enabled == true)
            {                
                remainingTime = 5.00f - elapsedTime;
                startTimeText.text = string.Format("Starts in {0:0.000}", remainingTime);
                
                if (remainingTime < 0)
                {
                    startTimeText.enabled = false;
                    timeText.enabled = true;
                    timerStarted = true;
                    StartGame();
                }
            }                               
        }

        timeText.text = string.Format("Time: {0:0.00}", elapsedTime);
               
        if (timerStarted)
        {
            this.StartRoundNow();
            timerStarted = false;
        }
       
    }

}
