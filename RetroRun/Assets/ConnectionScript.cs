using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ConnectionScript : MonoBehaviour {

    private string serverName = "", maxPlayers = "2", port = "25566";
    public InputField serverNameField;
    public InputField maxPlayersField;
    public InputField portField;

    //public GameObject serverPanel;

    //public Transform serverListPanel;

    private Rect windowRect = new Rect(0,0,200,200);

    void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect, windowFunc, "");
    }

    public void OnClickCreateServer() 
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            serverName = serverNameField.text;
            maxPlayers = maxPlayersField.text;
            port = portField.text;

            try
            {
                Network.InitializeSecurity();
                Network.InitializeServer(int.Parse(maxPlayers), int.Parse(port), !Network.HavePublicAddress());
                MasterServer.RegisterHost("TestGame", serverName);
            }
            catch (System.Exception)
            {
                Debug.Log("Error");
                throw;
            }
        }        
    }

    public void OnClickDisconnect()
    {
        Network.Disconnect();
    }

    private void windowFunc(int id) 
    {
        if (GUILayout.Button("Refresh")) 
        {
            MasterServer.RequestHostList("TestGame");
        }

        GUILayout.BeginHorizontal();

        GUILayout.Box("Server Name");

        GUILayout.EndHorizontal();

        if (MasterServer.PollHostList().Length != 0) 
        {
            HostData[] data = MasterServer.PollHostList();

            foreach (HostData hd in data)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Box(hd.gameName);
                if (GUILayout.Button("Connect")) 
                {
                    Network.Connect(hd);
                }

                GUILayout.EndHorizontal();
            }

        }

        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }    
}
