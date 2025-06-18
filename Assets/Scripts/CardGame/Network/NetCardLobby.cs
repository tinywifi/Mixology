using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetCardLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] Text infos;

    string localPlayerNick;

    
    void Start()
    {
        Screen.SetResolution(450, 800, false);

        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
            localPlayerNick = "Player#" + Random.Range(1000, 10000);
        else
            localPlayerNick = PhotonNetwork.LocalPlayer.NickName;

        infos.text = "Mixology " + localPlayerNick;

    }

    
    void Update()
    {
        
    }

    public void JoinLocal()
    {
        SceneManager.LoadScene("CardGame-Board");
    }

    void RoomReady()
    {
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            infos.text = "Waiting for player...";
        }
        else
        {
            infos.text = "Starting game...";

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel("CardGame-Board");
        }
    }
    #region Network
    public override void OnConnectedToMaster()
    {
        infos.text = "Connected...";
        
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Hashtable props = new Hashtable
            {
                {"LOADED_LEVEL", false} 
            };

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable room = new Hashtable
            {
                {"DECK_ID", Random.Range(1,1000)}, 
                {"START_TIME", (float)0 } 
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(room);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        RoomReady();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomReady();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions opts = new RoomOptions() { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(localPlayerNick + "Room", opts);
    }

    #endregion

    #region UI
    public void OnLogin()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = localPlayerNick;
            PhotonNetwork.ConnectUsingSettings();

            foreach (Button b in FindObjectsOfType<Button>())
            {
                b.interactable = false;
            }
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    #endregion
}
